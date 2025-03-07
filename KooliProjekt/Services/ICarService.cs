using KooliProjekt.Data;
using KooliProjekt.Search; // Make sure this is included

namespace KooliProjekt.Services
{
    public interface ICarService
    {
        Task<PagedResult<Car>> List(int page, int pageSize, CarSearch search = null);
        Task<Car> Get(int? Id);
        Task Save(Car car);
        Task Delete(int? Id);
        Task<bool> Includes(int Id);
    }
}