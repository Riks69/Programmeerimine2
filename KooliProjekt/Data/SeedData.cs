using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace KooliProjekt.Data
{
    public class SeedData
    {
        public static void GenerateCategories(ApplicationDbContext context)
        {
            // Check if the Categories table is empty
            if (context.Cars.Any())
            {
                return; // If data exists, don't do anything
            }

            // Create categories (without manually setting Id, assuming the database generates them automatically)
            var Cars = new[]
            {
                new Car { Type = "Sedaan", RegistrationNumber = "ABC 142", HourlyRate = 30, KmRate = 30, IsAvaliable = true},
                new Car { Type = "Universaal", RegistrationNumber = "ABD 142", HourlyRate = 35, KmRate = 35, IsAvaliable = true },
                new Car { Type = "Matkabuss", RegistrationNumber = "ABV 142", HourlyRate = 60, KmRate = 60, IsAvaliable = true },
                new Car { Type = "Sportauto", RegistrationNumber = "ABB 142", HourlyRate = 100, KmRate = 100, IsAvaliable = true},
                new Car { Type = "Kupee", RegistrationNumber = "ABN 142", HourlyRate = 40, KmRate = 40, IsAvaliable = true},
                new Car { Type = "Kaubik", RegistrationNumber = "ABG 142", HourlyRate = 55, KmRate = 55, IsAvaliable = true},
                new Car { Type = "Veoauto", RegistrationNumber = "ABW 142", HourlyRate = 95, KmRate = 95, IsAvaliable = true},
                new Car { Type = "Kabriolett", RegistrationNumber = "ABE 142", HourlyRate = 65, KmRate = 65, IsAvaliable = true},
                new Car { Type = "Maastur", RegistrationNumber = "ABR 142", HourlyRate = 45, KmRate = 45, IsAvaliable = true},
                new Car { Type = "Linnamaastur", RegistrationNumber = "ABT 142", HourlyRate = 50, KmRate = 50, IsAvaliable = true}
            };

            // Add categories to the database context
            context.Cars.AddRange(Cars);

            // Save changes to the database
            context.SaveChanges();
        }

        public static void GenerateCustomers(ApplicationDbContext context)
        {
            // Check if the Categories table is empty
            if (context.Customers.Any())
            {
                return; // If data exists, don't do anything
            }

            var Customers = new List<Customer>
 {
        new Customer { Id = 101, Name = "Urmas Karumets", Email = "Urmaskarumets@gmail.com", IsRegistered = true },
        new Customer { Id = 102, Name = "Andreas Õunmann", Email = "AndreasÕunmann@gmail.com",IsRegistered = true },
        new Customer { Id = 103, Name = "Andri Lombard", Email = "AndriLombard@gmail.com", IsRegistered = true },
        new Customer { Id = 104, Name = "Marco Soosaar", Email = "MarcoSoosaar@gmail.com", IsRegistered = true },
        new Customer { Id = 105, Name = "Karl Kalender", Email = "KarlKalender@gmail.com", IsRegistered = true },
        new Customer { Id = 106, Name = "Üllar Suvi", Email = "ÜllarSuvi@gmail.com", IsRegistered = true },
        new Customer { Id = 107, Name = "Peeter Tamm", Email = "PeeterTamm@gmail.com", IsRegistered = true },
        new Customer { Id = 108, Name = "Jürgen Kass", Email = "JürgenKass@gmail.com", IsRegistered = true },
        new Customer { Id = 109, Name = "Jacob Loop", Email = "JacobLoop@gmail.com", IsRegistered = true },
        new Customer { Id = 110, Name = "Vambola Koljat", Email = "VambolaKoljat@gmail.com", IsRegistered = true }

    };

            context.Customers.AddRange(Customers);

            context.SaveChanges();
        }
        public static void GenerateBookings(ApplicationDbContext context)
        {
            // Check if the Categories table is empty
            if (context.Bookings.Any())
            {
                return; // If data exists, don't do anything
            }

            var Booking = new List<Booking>
            {
                new Booking { Id = 201, UserId = 1, StartTime = new DateTime(2024, 11, 25, 9, 0, 0), EndTime = new DateTime(2024, 11, 28, 9, 0, 0), DistanceKm = 200, IsCompleted = true },
                new Booking { Id = 202, UserId = 2, StartTime = new DateTime(2024, 2, 6, 9, 0, 0), EndTime = new DateTime(2024, 2, 10, 9, 0, 0), DistanceKm = 195, IsCompleted = true },
                new Booking { Id = 203, UserId = 3, StartTime = new DateTime(2024, 12, 13, 8, 0, 0), EndTime = new DateTime(2024, 12, 17, 8, 0, 0), DistanceKm = 124, IsCompleted = true },
                new Booking { Id = 204, UserId = 4, StartTime = new DateTime(2024, 1, 5, 12, 0, 0), EndTime = new DateTime(2024, 1, 7, 12, 0, 0), DistanceKm = 153, IsCompleted = true },
                new Booking { Id = 205, UserId = 5, StartTime = new DateTime(2024, 6, 15, 9, 0, 0), EndTime = new DateTime(2024, 6, 22, 9, 0, 0), DistanceKm = 123, IsCompleted = true },
                new Booking { Id = 206, UserId = 6, StartTime = new DateTime(2024, 7, 23, 10, 0, 0), EndTime = new DateTime(2024, 7, 24, 10, 0, 0), DistanceKm = 652, IsCompleted = true },
                new Booking { Id = 207, UserId = 7, StartTime = new DateTime(2024, 3, 18, 18, 0, 0), EndTime = new DateTime(2024, 3, 20, 18, 0, 0), DistanceKm = 127, IsCompleted = true },
                new Booking { Id = 208, UserId = 8, StartTime = new DateTime(2024, 4, 3, 12, 0, 0), EndTime = new DateTime(2024, 4, 8, 12, 0, 0), DistanceKm = 173, IsCompleted = true },
                new Booking { Id = 209, UserId = 9, StartTime = new DateTime(2024, 8, 17, 18, 0, 0), EndTime = new DateTime(2024, 8, 21, 18, 0, 0), DistanceKm = 92, IsCompleted = true },
                new Booking { Id = 210, UserId = 10, StartTime = new DateTime(2024, 10, 27, 11, 0, 0), EndTime = new DateTime(2024, 10, 30, 11, 0, 0), DistanceKm = 182, IsCompleted = true }

            };

            context.Bookings.AddRange(Booking);

            context.SaveChanges();
        }
    }
}