using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Services;

using System;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    private static IConfiguration configuration;

    static async Task Main(string[] args)
    {

        configuration = new ConfigurationBuilder()
       //.AddJsonFile("appsettings.json", true, true)
       .Build();

        var serviceProvider = new ServiceCollection()
            .AddLogging(configure => configure.AddConsole())
             .Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Trace)
                                .AddSingleton<IRandomNumberGenerator, RandomNumberGenerator>()
                                .AddSingleton<IFileService, FileService>()
                                .BuildServiceProvider();

        var logger = serviceProvider.GetService<ILogger<Program>>();

        var service = serviceProvider.GetService<IRandomNumberGenerator>();
        var fileService = serviceProvider.GetService<IFileService>();

        bool run = true;

        while (run)
        {
            await Menu(service, fileService);

            Console.WriteLine("");
            Console.WriteLine("Exit Y/N?");

            string input = Console.ReadLine();

            if (input.ToLower() == "y")
            {
                run = false;
            }

            Console.Clear();
        }
    }

    private static async Task Menu(IRandomNumberGenerator randomNumber, IFileService fileService)
    {
        Console.WriteLine("Random Number Generator [2019]");
        Console.WriteLine("");

        var settingsFound = await LoadSettings(fileService);

        if (settingsFound == null)
        {
            NumericalMenu("Enter Number of Random numbers required", out int numberOfRandomNumbers);

            NumericalMenu("Enter minimum Number for range", out int minRange);

            NumericalMenu("Enter maximum Number for range", out int maxRange);

            NumericalMenu("Enter number of records", out int range);

            settingsFound = new Models.Settings()
            {
                Name = Guid.NewGuid().ToString(),
                Quantity = numberOfRandomNumbers,
                FromValue = minRange,
                ToValue = maxRange,
                OutputRecords = range
            };
        }

        fileService.Save(settingsFound);

        var outputs = await randomNumber.Create(settingsFound);

        foreach (var output in outputs)
        {
            Console.WriteLine($"{ string.Join(", ", output)}");
        }
    }

    private async static Task<Models.Settings> LoadSettings(IFileService fileService)
    {
        bool runAgain = true;

        while (runAgain)
        {
            var settings = await fileService.GetAll<Models.Settings>();

            if (settings.Any())
            {
                for (int i = 1; i <= settings.Count; i++)
                {
                    Console.WriteLine($"{i}:{settings[i - 1].Name}|Number Length:{settings[i - 1].Quantity}" +
                        $"|Range:{settings[i - 1].FromValue}-{settings[i - 1].ToValue}" +
                        $"|Iterations:{settings[i - 1].OutputRecords}");
                }
                Console.WriteLine($"{settings.Count + 1}:Create new recoed");

                Console.WriteLine("");
                Console.WriteLine("Please select option.? ");
                string input = Console.ReadLine();

                int.TryParse(input, out int option);

                if (option == 0)
                {
                    runAgain = true;
                }

                if (option == settings.Count + 1)
                {
                    return null;
                }

                return settings[option - 1];

            }
        }

        return null;
    }

    private static void NumericalMenu(string message, out int inputValue)
    {
        bool runAgain = true;
        inputValue = 0;

        while (runAgain)
        {
            Console.WriteLine($"{message} ?");

            string input = Console.ReadLine();

            int.TryParse(input, out inputValue);

            if (inputValue == 0)
            {
                runAgain = true;
            }
            else
            {
                runAgain = false;
            }
        }
    }
}
