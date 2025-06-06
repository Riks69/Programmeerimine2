using System.ComponentModel.DataAnnotations;

namespace KooliProjekt.PublicAPI.Api
{
    public class Customer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nimi on kohustuslik")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parool on kohustuslik")]
        public string Password { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Kehtetu e-posti aadress")]
        public string Email { get; set; } = string.Empty;

        public bool IsRegistered { get; set; }
    }
}
