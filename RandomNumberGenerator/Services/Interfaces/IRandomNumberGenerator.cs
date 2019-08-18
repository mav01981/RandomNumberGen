using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IRandomNumberGenerator
    {
        Task<IEnumerable<int[]>> Create(Settings settings);
    }
}
