namespace KooliProjekt.WinFormsApp.Api
{
    public interface IApiClient
    {
        Task<Result<List<Customer>>> List();
        Task Save(Customer list);
        Task Delete(int id);
    }
}