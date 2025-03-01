using Microsoft.EntityFrameworkCore;
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
            // Check if the Customers table is empty
            if (context.Customers.Any())
            {
                return; // If data exists, don't do anything
            }

            var customers = new List<Customer>
    {
        new Customer { Name = "Urmas Karumets", Email = "Urmaskarumets@gmail.com", IsRegistered = true, Password = "1234" },
        new Customer { Name = "Andreas Õunmann", Email = "AndreasÕunmann@gmail.com", IsRegistered = true, Password = "1243"},
        new Customer { Name = "Andri Lombard", Email = "AndriLombard@gmail.com", IsRegistered = true, Password = "4321" },
        new Customer { Name = "Marco Soosaar", Email = "MarcoSoosaar@gmail.com", IsRegistered = true, Password = "4213" },
        new Customer { Name = "Karl Kalender", Email = "KarlKalender@gmail.com", IsRegistered = true, Password = "2341" },
        new Customer { Name = "Üllar Suvi", Email = "ÜllarSuvi@gmail.com", IsRegistered = true, Password = "2134" },
        new Customer { Name = "Peeter Tamm", Email = "PeeterTamm@gmail.com", IsRegistered = true, Password = "2431" },
        new Customer { Name = "Jürgen Kass", Email = "JürgenKass@gmail.com", IsRegistered = true, Password = "3214" },
        new Customer { Name = "Jacob Loop", Email = "JacobLoop@gmail.com", IsRegistered = true, Password = "3143" },
        new Customer { Name = "Vambola Koljat", Email = "VambolaKoljat@gmail.com", IsRegistered = true, Password = "4124" }
    };


            context.Customers.AddRange(customers);

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
                new Booking { UserId = 1, StartTime = new DateTime(2024, 11, 25, 9, 0, 0), EndTime = new DateTime(2024, 11, 28, 9, 0, 0), DistanceKm = 200, IsCompleted = true },
                new Booking { UserId = 2, StartTime = new DateTime(2024, 2, 6, 9, 0, 0), EndTime = new DateTime(2024, 2, 10, 9, 0, 0), DistanceKm = 195, IsCompleted = true },
                new Booking { UserId = 3, StartTime = new DateTime(2024, 12, 13, 8, 0, 0), EndTime = new DateTime(2024, 12, 17, 8, 0, 0), DistanceKm = 124, IsCompleted = true },
                new Booking { UserId = 4, StartTime = new DateTime(2024, 1, 5, 12, 0, 0), EndTime = new DateTime(2024, 1, 7, 12, 0, 0), DistanceKm = 153, IsCompleted = true },
                new Booking { UserId = 5, StartTime = new DateTime(2024, 6, 15, 9, 0, 0), EndTime = new DateTime(2024, 6, 22, 9, 0, 0), DistanceKm = 123, IsCompleted = true },
                new Booking { UserId = 6, StartTime = new DateTime(2024, 7, 23, 10, 0, 0), EndTime = new DateTime(2024, 7, 24, 10, 0, 0), DistanceKm = 652, IsCompleted = true },
                new Booking { UserId = 7, StartTime = new DateTime(2024, 3, 18, 18, 0, 0), EndTime = new DateTime(2024, 3, 20, 18, 0, 0), DistanceKm = 127, IsCompleted = true },
                new Booking { UserId = 8, StartTime = new DateTime(2024, 4, 3, 12, 0, 0), EndTime = new DateTime(2024, 4, 8, 12, 0, 0), DistanceKm = 173, IsCompleted = true },
                new Booking { UserId = 9, StartTime = new DateTime(2024, 8, 17, 18, 0, 0), EndTime = new DateTime(2024, 8, 21, 18, 0, 0), DistanceKm = 92, IsCompleted = true },
                new Booking { UserId = 10, StartTime = new DateTime(2024, 10, 27, 11, 0, 0), EndTime = new DateTime(2024, 10, 30, 11, 0, 0), DistanceKm = 182, IsCompleted = true }

            };

            context.Bookings.AddRange(Booking);

            context.SaveChanges();
        }
        public static void GenerateInvoices(ApplicationDbContext context)
        {
            // Check if the Categories table is empty
            if (context.Invoices.Any())
            {
                return; // If data exists, don't do anything
            }

            var Invoice = new List<Invoice>
            {
                new Invoice { BookingId = 1, Amount = 35, Description = "a", IsPaid = true },
                new Invoice { BookingId = 2, Amount = 36, Description = "b", IsPaid = true },
                new Invoice { BookingId = 3, Amount = 37, Description = "c", IsPaid = true },
                new Invoice { BookingId = 4, Amount = 38, Description = "d", IsPaid = true },
                new Invoice { BookingId = 5, Amount = 39, Description = "e", IsPaid = true },
                new Invoice { BookingId = 6, Amount = 40, Description = "f", IsPaid = true },
                new Invoice { BookingId = 7, Amount = 41, Description = "g", IsPaid = true },
                new Invoice { BookingId = 8, Amount = 42, Description = "h", IsPaid = true },
                new Invoice { BookingId = 9, Amount = 43, Description = "i", IsPaid = true },
                new Invoice { BookingId = 10, Amount = 44, Description = "j", IsPaid = true }

            };

            context.Invoices.AddRange(Invoice);

            context.SaveChanges();
        }
    }
}