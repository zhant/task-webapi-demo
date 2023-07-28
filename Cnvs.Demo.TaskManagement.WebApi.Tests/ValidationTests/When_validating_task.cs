using Cnvs.Demo.TaskManagement.WebApi.Validators;
using FakeItEasy;
using FluentAssertions;
using FluentValidation.TestHelper;
using Task = Cnvs.Demo.TaskManagement.Dto.Task;

namespace Cnvs.Demo.TaskManagement.WebApi.Tests.ValidationTests;

public class When_validating_task
{
    private readonly TaskValidator _validator;

    // ReSharper disable once ConvertConstructorToMemberInitializers
    public When_validating_task()
    {
        _validator = new TaskValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Description_Is_Empty()
    {
        var task = A.Fake<Task>();
        task.Description = "";

        var result = _validator.TestValidate(task);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(failure => failure.PropertyName == "Description");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Description_Is_Not_Empty()
    {
        var task = A.Fake<Task>();
        task.Description = "Valid description";

        var result = _validator.TestValidate(task);

        result.IsValid.Should().BeTrue();
    }
}