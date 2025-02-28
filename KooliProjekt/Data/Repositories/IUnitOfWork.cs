namespace KooliProjekt.Data.Repositories
{
    public interface IUnitOfWork
    {
        ICustomerRepository CustomerRepository { get; }
        IInvoiceRepository InvoiceRepository { get; }
        ICarRepository CarRepository { get; }
        IBookingRepository BookingRepository { get; }

    }
}