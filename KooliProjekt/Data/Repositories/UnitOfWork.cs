namespace KooliProjekt.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context,
        ICustomerRepository customerRepository,
        IInvoiceRepository invoiceRepository,
        ICarRepository carRepository,
        IBookingRepository bookingRepository)

        {
            _context = context;

            CustomerRepository = customerRepository;
            InvoiceRepository = invoiceRepository;
            CarRepository = carRepository;
            BookingRepository = bookingRepository;
        }

        public ICustomerRepository CustomerRepository { get; }
        public IInvoiceRepository InvoiceRepository { get; }
        public ICarRepository CarRepository { get; }
        public IBookingRepository BookingRepository { get; }


    }
}