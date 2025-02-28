using KooliProjekt.Data;
using KooliProjekt.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Services
{
    public class CarService : ICarService
    {
        private readonly IUnitOfWork _uof;

        public CarService(IUnitOfWork uof)
        {
            _uof = uof;
        }

        public async Task Delete(int Id)
        {
            await _uof.CarRepository.Delete(Id);
        }

        public async Task<Car> Get(int Id)
        {
            return await _uof.CarRepository.Get((int)Id);
        }

        public async Task<PagedResult<Car>> List(int page, int pageSize)
        {
            return await _uof.CarRepository.List(page, pageSize);
        }

        public async Task Save(Car car)
        {
            await _uof.CarRepository.Save(car);
        }
    }
}