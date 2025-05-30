using System.ComponentModel.DataAnnotations;
namespace KooliProjekt.WinFormsApp.Api
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public Boolean IsRegistered { get; set; }

    }
}
