using KooliProjekt.Data;
using KooliProjekt.Search;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Services
{
    public class CarService : ICarService
    {
        private readonly ApplicationDbContext _context;

        public CarService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Car>> List(int page, int pageSize, CarSearch search = null)
        {
            var query = _context.Cars.AsQueryable();

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

        public async Task<Car> Get(int id)
        {
            return await _context.Cars.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task Save(Car list)
        {
            if (list.Id == 0)
            {
                _context.Add(list);
            }
            else
            {
                _context.Update(list);
            }

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }
        }
    }
}
