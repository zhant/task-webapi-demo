using Cnvs.Demo.TaskManagement.WebApi.Middleware;
using FluentValidation.Results;

namespace Cnvs.Demo.TaskManagement.WebApi.Extensions;

public static class ValidationExtensions
{
    public static List<Error> Parse(this IEnumerable<ValidationFailure> validationFailures)
    {
        return validationFailures.Select((Func<ValidationFailure, Error>)(x => new Error
        {
            Code = x.ErrorCode,
            Title = x.PropertyName,
            Detail = x.ErrorMessage
        })).ToList();
    }

    public static ValidationResult Parse(this IEnumerable<Error> errors)
    {
        return new ValidationResult(errors.Select((Func<Error, ValidationFailure>)(error => new ValidationFailure(error.Title, error.Detail)))
            .ToList());
    }
}