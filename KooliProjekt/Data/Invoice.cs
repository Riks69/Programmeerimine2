using System.ComponentModel.DataAnnotations;

namespace KooliProjekt.Data
{
    public class Invoice
    {
        public int Id { get; set; }
        [Required]
        public int BookingId { get; set; }
        [Required]
        public double Amount { get; set; }
        [Required]
        public string Description { get; set; }
        public bool IsPaid { get; set; }
    }
}
