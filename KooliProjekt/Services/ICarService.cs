using KooliProjekt.Data;
using KooliProjekt.Search;

namespace KooliProjekt.Services
{
    public interface ICarService
    {
        Task<PagedResult<Car>> List(int page, int pageSize, CarSearch search);
        Task<Car> Get(int id);
        Task Save(Car list);
        Task Delete(int id);
    }
}