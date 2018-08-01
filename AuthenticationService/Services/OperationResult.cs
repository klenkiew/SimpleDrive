using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Services
{
    public class OperationResult
    {
        public bool IsValid { get; }
        public IReadOnlyCollection<OperationError> Errors { get; }

        protected OperationResult(bool isValid, IReadOnlyCollection<OperationError> errors)
        {
            Ensure.NotNull(errors, nameof(errors));
            
            if (isValid && errors.Any())
                throw new ArgumentException("A valid result cannot contain any errors.");

            this.IsValid = isValid;
            this.Errors = errors;
        }

        public static OperationResult Valid()
        {
            return new OperationResult(true, ArraySegment<OperationError>.Empty);
        }
        
        public static OperationResult Invalid(IReadOnlyCollection<OperationError> errors)
        {
            return new OperationResult(false, errors);
        }
        
        public static OperationResult Invalid(params OperationError[] errors)
        {
            return new OperationResult(false, errors);
        }
    }

    public class OperationResult<T> : OperationResult
    {
        public T Result { get; }
        
        protected OperationResult(T result, bool isValid, IReadOnlyCollection<OperationError> errors) 
            : base(isValid, errors)
        {
            this.Result = result;
        }

        public OperationResult(OperationResult result) : base(result.IsValid, result.Errors)
        {
        }

        public OperationResult(T @object, OperationResult result) : base(result.IsValid, result.Errors)
        {
            Result = @object;
        }

        public static OperationResult<T> Valid(T result)
        {
            return new OperationResult<T>(result, true, ArraySegment<OperationError>.Empty);
        }

        public new static OperationResult<T> Invalid(IReadOnlyCollection<OperationError> errors)
        {
            return new OperationResult<T>(default(T), false, errors);
        }

        public new static OperationResult<T> Invalid(params OperationError[] errors)
        {
            return new OperationResult<T>(default(T), false, errors);
        }
    }
    
    public class OperationError
    {
        public enum ErrorType {General, NotFound, ProcessingError}
        
        public string Description { get; }
        public ErrorType Type { get; }
        
        public OperationError(string description)
        {
            Description = description;
            Type = ErrorType.General;
        }

        public OperationError(string description, ErrorType type)
        {
            Description = description;
            Type = type;
        }

        public static implicit operator string(OperationError error)
        {
            return error.Description;
        }
        
        public static implicit operator OperationError(string error)
        {
            return new OperationError(error);
        }
    }

    public static class ResultExtensions
    {
        public static OperationResult<T> Cast<T>(this OperationResult result)
        {
            return new OperationResult<T>(result);
        }
        
        public static OperationResult<T> WithObject<T>(this OperationResult result, T @object)
        {
            return new OperationResult<T>(@object, result);
        }
        
        public static OperationResult ToOperationResult(this IdentityResult result)
        {
            return result.Succeeded ? OperationResult.Valid() 
                : OperationResult.Invalid(result.Errors
                    .Select<IdentityError, OperationError>(error => error.Description).ToList());
        }

        public static OperationResult ToOperationResult(this SignInResult result)
        {
            if (result.Succeeded)
                return OperationResult.Valid();
            
            var details = "invalid e-mail or password.";
            if (result.IsLockedOut)
                details = "the account is locked out.";
            else if (result.RequiresTwoFactor)
                details = "two factor authentication is required.";
            else if (result.IsNotAllowed)
                details = "invalid e-mail or password.";
            
            return OperationResult.Invalid("Login failed: " + details);
        }
    }
}