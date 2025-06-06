namespace KooliProjekt.PublicAPI.Api
{
    public class Result
    {
        public string? Error { get; set; }
        public Dictionary<string, List<string>> Errors { get; set; } = new();
        public bool HasError => !string.IsNullOrEmpty(Error) || Errors.Count > 0;
    }

   
}
