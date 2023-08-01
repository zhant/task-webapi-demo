using FluentValidation;

namespace Cnvs.Demo.TaskManagement.WebApi.Validators;

public class TaskToUpdateValidator : AbstractValidator<Dto.TaskToUpdate>
{
    public TaskToUpdateValidator()
    {
        RuleFor(task => task).NotNull().WithMessage("Task is required");
        RuleFor(task => task.Id).NotEmpty().WithMessage("Task Id is required");
        RuleFor(task => task.Description).NotEmpty().WithMessage("Task Description is required");
    }
}