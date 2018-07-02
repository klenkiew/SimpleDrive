using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FileService.Validation
{
    public class ValidationMessage
    {
        public string Message { get; }
        
        public List<ValidationError> Errors { get; }
        
        public ValidationMessage(ModelStateDictionary validationState)
        {
            Message = "Validation failed";
            Errors = validationState.SelectMany(modelState => modelState.Value.Errors
                    .Select(error => new ValidationError(modelState.Key, error.ErrorMessage)))
                .ToList();
        }
    }
}