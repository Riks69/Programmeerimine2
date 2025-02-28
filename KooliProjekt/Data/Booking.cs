using System.ComponentModel.DataAnnotations;

namespace KooliProjekt.Data
{
    public class Booking : Entity
    {
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int CarId { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public double DistanceKm { get; set; }
        public Boolean IsCompleted { get; set; }
    }
}
