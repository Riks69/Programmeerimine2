using KooliProjekt.Data;
using KooliProjekt.Search;

namespace KooliProjekt.Models
{
    public class CarIndexModel

    {
        public CarSearch Search { get; set; }
        public PagedResult<Car> Data { get; set; }
    }
}
