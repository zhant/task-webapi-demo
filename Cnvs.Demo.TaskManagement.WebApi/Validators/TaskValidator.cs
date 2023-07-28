using FluentValidation;
using Task = Cnvs.Demo.TaskManagement.Dto.Task;

namespace Cnvs.Demo.TaskManagement.WebApi.Validators;

public class TaskValidator : AbstractValidator<Task>
{
    public TaskValidator()
    {
        RuleFor(task => task).NotNull().WithMessage("Task is required");
        RuleFor(task => task.Description).NotEmpty().WithMessage("Task Description is required");
    }
}