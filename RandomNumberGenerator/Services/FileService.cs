using Models;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Services
{
    public class FileService : IFileService
    {
        private DirectoryInfo _fileInfo;
        public FileService()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);

            _fileInfo = new FileInfo(location.AbsolutePath).Directory;

            if (!File.Exists($@"{_fileInfo.FullName}\settings"))
            {
                Directory.CreateDirectory($@"{_fileInfo.FullName}\settings");
            }
        }

        public async Task<T> Get<T>(string fileName)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText($"{_fileInfo.FullName}/{fileName}"));
        }

        public async Task<IList<T>> GetAll<T>()
        {
            IList<T> collection = new List<T>();

            var ext = new List<string> { ".json" };
            var data = Directory.GetFiles($@"{_fileInfo.FullName}\settings", "*.*", SearchOption.TopDirectoryOnly)
                 .Where(s => ext.Contains(Path.GetExtension(s)))
            .OrderByDescending(d => new FileInfo(d).CreationTime);

            foreach (var file in data)
            {
                collection.Add(JsonConvert.DeserializeObject<T>(File.ReadAllText($"{file}")));
            }

            return collection;
        }

        public void Save(Settings settings)
        {
            File.WriteAllText($"{_fileInfo.FullName}/settings/{settings.Name}.json", JsonConvert.SerializeObject(settings));
        }
    }
}
