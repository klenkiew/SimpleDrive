using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Common.Validation
{
    public class ValidationMessage
    {
        private string Message { get; }
        
        private List<ValidationError> Errors { get; }
        
        public ValidationMessage(ModelStateDictionary validationState)
        {
            Message = "Validation failed";
            Errors = validationState.SelectMany(modelState => modelState.Value.Errors
                    .Select(error => new ValidationError(modelState.Key, error.ErrorMessage)))
                .ToList();
        }
    }
}