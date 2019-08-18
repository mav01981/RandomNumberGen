using Models;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class RandomNumberGenerator : IRandomNumberGenerator
    {
        public async Task<IEnumerable<int[]>> Create(Settings settings)
        {
            var collections = new List<int[]>();

            for (int record = 0; record < settings.OutputRecords; record++)
            {
                var random = new Random();

                int[] result = new int[settings.Quantity];

                for (int i = 0; i < settings.Quantity; i++)
                {
                    result[i] = random.Next(settings.FromValue, settings.ToValue);
                }

                collections.Add(result);
            }

            return collections;
        }
    }
}
