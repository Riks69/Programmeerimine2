using KooliProjekt.Data;

namespace KooliProjekt.Services
{
    public interface ICarService
    {
        Task<PagedResult<Car>> List(int page, int pageSize);
        Task<Car> Get(int id);
        Task Save(Car list);
        Task Delete(int id);
    }
}