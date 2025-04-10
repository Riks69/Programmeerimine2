using System.ComponentModel.DataAnnotations;

namespace KooliProjekt.Data
{
    public class Car
    {
        public int Id { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string RegistrationNumber { get; set; }
        [Required]
        public double HourlyRate { get; set; }
        [Required]
        public double KmRate { get; set; }
        [Required]
        public Boolean IsAvaliable { get; set; }
    }
}