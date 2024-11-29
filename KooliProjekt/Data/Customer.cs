using System.ComponentModel.DataAnnotations;

namespace KooliProjekt.Data
{
    public class Customer
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [StringLength(50)]
        public string Password { get; set; }
        [Required]
        [StringLength(50)]
        public string Email { get; set; }
        [Required]
        public Boolean IsRegistered { get; set; }
    }
}
