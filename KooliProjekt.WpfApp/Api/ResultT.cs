using KooliProjekt.WpfApp.Api;
using KooliProjekt.WpfApp.ApiAdd;

namespace KooliProjekt.WpfApp.ApiAdd
{
    public class Result<T>
{
    public bool IsSuccess { get; set; }
    public string Error { get; set; }
    public T Value { get; set; }

    public bool HasError => !IsSuccess;

    public static Result<T> Success(T value) => new Result<T> { IsSuccess = true, Value = value };
    public static Result<T> Failure(string error) => new Result<T> { IsSuccess = false, Error = error };
}
}