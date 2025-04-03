using KooliProjekt.Data;
using KooliProjekt.Search;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace KooliProjekt.Services
{
    public class CarService : ICarService
    {
        private readonly ApplicationDbContext _context;

        public CarService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Delete(int? Id)
        {
            var car = await _context.Cars.FindAsync(Id);
            if (car != null)
            {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Car> Get(int? Id)
        {
            return await _context.Cars.FindAsync(Id);
        }

        public async Task<bool> Includes(int Id)
        {
            return await _context.Cars.AnyAsync(c => c.Id == Id);
        }

        public async Task<PagedResult<Car>> List(int page, int pageSize, CarSearch search = null)
        {
            var query = _context.Cars.AsQueryable();

            // Kui otsingukriteerium on olemas ja Keyword pole tühi
            if (search != null && !string.IsNullOrEmpty(search.Keyword))
            {
                query = query.Where(h =>
                    EF.Functions.Like(h.Type.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.RegistrationNumber.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.HourlyRate.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.KmRate.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.IsAvaliable.ToString(), $"%{search.Keyword}%"));
            }

            return await query.GetPagedAsync(page, pageSize);
        }

        // Uus Search meetod, kus arvestatakse IsAvaliable väärtuse kontrolliga
        public async Task<List<Car>> Search(CarSearch search)
        {
            IQueryable<Car> query = _context.Cars;

            // Filtreeri Keyword järgi
            if (search != null && !string.IsNullOrEmpty(search.Keyword))
            {
                query = query.Where(h =>
                    EF.Functions.Like(h.Type.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.RegistrationNumber.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.HourlyRate.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.KmRate.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.IsAvaliable.ToString(), $"%{search.Keyword}%"));
            }

            // Filtreeri Availability järgi, kui see on määratud
            if (search?.IsAvaliable != null)
            {
                query = query.Where(h => h.IsAvaliable == search.IsAvaliable);
            }

            return await query.ToListAsync(); // Tagastab kõik vastavad autod
        }

        public async Task Save(Car car)
        {
            if (car.Id == 0)
            {
                _context.Cars.Add(car);
            }
            else
            {
                var existingCars = await _context.Cars.FindAsync(car.Id);

                if (existingCars != null)
                {
                    // Kui olemas, siis uuenda
                    _context.Entry(existingCars).State = EntityState.Modified;
                }
                else
                {
                    _context.Cars.Add(car);
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
