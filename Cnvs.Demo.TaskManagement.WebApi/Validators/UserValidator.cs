using Cnvs.Demo.TaskManagement.Dto;
using FluentValidation;

namespace Cnvs.Demo.TaskManagement.WebApi.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(user => user).NotNull().WithMessage("User is required");
        RuleFor(user => user.Name).NotEmpty().WithMessage("User Name is required");
    }
}