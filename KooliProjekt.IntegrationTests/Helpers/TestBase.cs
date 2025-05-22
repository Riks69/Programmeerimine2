using KooliProjekt.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace KooliProjekt.IntegrationTests.Helpers
{
    public abstract class TestBase : IDisposable
    {
        public WebApplicationFactory<FakeStartup> Factory { get; }

        // Pääs `ApplicationDbContext`-ile
        protected ApplicationDbContext _context => Factory.Services.GetService<ApplicationDbContext>();

        public TestBase()
        {
            Factory = new TestApplicationFactory<FakeStartup>();
        }

        public void Dispose()
        {
            // Kustutame mälupõhise andmebaasi pärast iga testi lõppu
            var dbContext = _context;
            dbContext.Database.EnsureDeleted();  // Kustutame mälupõhise andmebaasi
        }
    }
}
