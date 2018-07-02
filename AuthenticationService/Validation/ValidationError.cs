using Newtonsoft.Json;

namespace FileService.Validation
{
    public class ValidationError
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; }
        public string Error { get; }

        public ValidationError(string field, string error)
        {
            Field = string.IsNullOrEmpty(field) ? null : field;
            Error = error;
        }
    }
}