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
                new Car {Type = "Sportauto", RegistrationNumber = "ABB 142", HourlyRate = 100, KmRate = 100, IsAvaliable = true},
                new Car {Type = "Kupee", RegistrationNumber = "ABN 142", HourlyRate = 40, KmRate = 40, IsAvaliable = true},
                new Car {Type = "Kaubik", RegistrationNumber = "ABG 142", HourlyRate = 55, KmRate = 55, IsAvaliable = true},
                new Car {Type = "Veoauto", RegistrationNumber = "ABW 142", HourlyRate = 95, KmRate = 95, IsAvaliable = true},
                new Car {Type = "Kabriolett", RegistrationNumber = "ABE 142", HourlyRate = 65, KmRate = 65, IsAvaliable = true},
                new Car {Type = "Maastur", RegistrationNumber = "ABR 142", HourlyRate = 45, KmRate = 45, IsAvaliable = true},
                new Car {Type = "Linnamaastur", RegistrationNumber = "ABT 142", HourlyRate = 50, KmRate = 50, IsAvaliable = true}
            };

            // Add categories to the database context
            context.Cars.AddRange(Cars);

            // Save changes to the database
            context.SaveChanges();
        }
    }
}
