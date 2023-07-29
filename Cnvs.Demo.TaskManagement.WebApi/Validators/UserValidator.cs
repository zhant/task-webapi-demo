using Cnvs.Demo.TaskManagement.Domain;
using FluentValidation;
using User = Cnvs.Demo.TaskManagement.Dto.User;

namespace Cnvs.Demo.TaskManagement.WebApi.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(user => user).NotNull().WithMessage("User is required");
        RuleFor(user => user.Name).NotEmpty().WithMessage("User Name is required");
        RuleFor(user => user.Name).NotEqual(NullUser.Instance.Name)
            .WithMessage("This User Name is reserved and should not be used");
    }
}