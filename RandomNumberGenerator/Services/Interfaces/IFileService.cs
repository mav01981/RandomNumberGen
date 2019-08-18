using Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{

    public interface IFileService
    {
        Task<IList<T>> GetAll<T>();
        Task<T> Get<T>(string fileName);
        void Save(Settings settings);
    }
}
