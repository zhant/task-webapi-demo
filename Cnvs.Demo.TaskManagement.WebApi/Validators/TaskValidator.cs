using FluentValidation;
using Task = Cnvs.Demo.TaskManagement.Dto.Task;

namespace Cnvs.Demo.TaskManagement.WebApi.Validators;

public class TaskValidator : AbstractValidator<Task>
{
    public TaskValidator()
    {
        RuleFor(x => x.Description).NotEmpty();
    }
}