namespace KooliProjekt.PublicAPI.Api
{
    public class Result<T> : Result
    {
        public T Value { get; set; }
    }
}