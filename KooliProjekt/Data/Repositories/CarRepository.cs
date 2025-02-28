
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Data.Repositories
{
    public class CarRepository : BaseRepository<Car>, ICarRepository
    {
        public CarRepository(ApplicationDbContext context) : base(context)
        {
        }

    }
}