using KooliProjekt.Data;
using KooliProjekt.Search;
using System.Threading.Tasks;

namespace KooliProjekt.Services
{
    public interface ICarService
    {
        Task<PagedResult<Car>> List(int page, int pageSize, CarSearch search = null);
        Task<List<Car>> Search(CarSearch search);
        Task<Car> Get(int? Id);
        Task Save(Car car);
        Task Delete(int? Id);
        Task<bool> Includes(int Id);
    }
}
