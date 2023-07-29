using Cnvs.Demo.TaskManagement.Domain;
using Cnvs.Demo.TaskManagement.WebApi.Validators;
using FakeItEasy;
using FluentAssertions;
using FluentValidation.TestHelper;
using User = Cnvs.Demo.TaskManagement.Dto.User;

namespace Cnvs.Demo.TaskManagement.WebApi.Tests.ValidationTests;

public class When_validating_user
{
    private readonly UserValidator _validator;

    // ReSharper disable once ConvertConstructorToMemberInitializers
    public When_validating_user()
    {
        _validator = new UserValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var user = A.Fake<User>();
        user.Name = "";

        var result = _validator.TestValidate(user);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(failure => failure.PropertyName == "Name");
    }
    
    [Fact]
    public void Should_Have_Error_When_Name_Is_Reserved()
    {
        var user = A.Fake<User>();
        user.Name = NullUser.Instance.Name;

        var result = _validator.TestValidate(user);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(failure => failure.PropertyName == "Name");
    }
    
    [Fact]
    public void Should_Not_Have_Error_When_Valid_User()
    {
        var user = A.Fake<User>();
        user.Name = "ValidName";

        var result = _validator.TestValidate(user);

        result.IsValid.Should().BeTrue();
    }
}
