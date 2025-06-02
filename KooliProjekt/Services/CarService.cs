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


        // Uus Search meetod
        public async Task<List<Car>> Search(CarSearch search)
        {
            IQueryable<Car> query = _context.Cars;

            // Filtreeri Keyword järgi (see peaks olema esimene, et mitte mõjutada teisi filtreid)
            if (search != null && !string.IsNullOrEmpty(search.Keyword))
            {
                query = query.Where(h =>
                    EF.Functions.Like(h.Type.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.RegistrationNumber.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.HourlyRate.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.KmRate.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.IsAvaliable.ToString(), $"%{search.Keyword}%"));
            }

            // Filtreeri Done (lõpetatud) järgi, kui see on määratud
            if (search?.Done.HasValue == true)
            {
                query = query.Where(h => h.IsAvaliable == search.Done);
            }

            return await query.ToListAsync(); // Tagastab kõik vastavad broneeringud
        }



        public async Task Save(Car car)
        {
            if (car.Id == 0)
            {
                // Kui ID on 0, siis lisame uue broneeringu
                _context.Cars.Add(car);
            }
            else
            {
                // Kui ID on olemas, siis otsime broneeringut ID järgi
                var existingCar = await _context.Cars.FindAsync(car.Id);

                if (existingCar == null)
                {
                    // Kui broneeringut ei leita, siis ei tee midagi
                    return;
                }

                // Kui broneering on olemas, siis uuendame andmeid
                existingCar.Type = car.Type;
                existingCar.RegistrationNumber = car.RegistrationNumber;
                existingCar.HourlyRate = car.HourlyRate;
                existingCar.KmRate = car.KmRate;
                existingCar.IsAvaliable = car.IsAvaliable;
            }

            // Tagame, et muudatused salvestatakse ja ID määratakse õigesti
            await _context.SaveChangesAsync();
        }
    }
}
