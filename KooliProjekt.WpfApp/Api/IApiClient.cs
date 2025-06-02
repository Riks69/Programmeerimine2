using KooliProjekt.WpfApp.ApiAdd;

namespace KooliProjekt.WpfApp.Api
{
    public interface IApiClient
    {
        Task<Result<List<Customer>>> List();
        Task<Result> Save(Customer list);
        Task<Result> Delete(int id);
    }
}
