using System.Collections.Generic;

namespace KooliProjekt.PublicAPI.Api
{
    public class Result
    {
        public Dictionary<string, List<string>> Errors { get; set; }

        public Result()
        {
            Errors = new Dictionary<string, List<string>>();
        }
        public bool HasError
        {
            get
            {
                return Errors.Count > 0;
            }
        }

        public void AddError(string propertyName, string errorMessage)
        {
            if (Errors.ContainsKey(propertyName))
            {
                Errors.Add(propertyName, new List<string>());
            }

            Errors[propertyName].Add(errorMessage);
        }
    }
}