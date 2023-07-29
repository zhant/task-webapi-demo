using Cnvs.Demo.TaskManagement.Domain;
using FakeItEasy;
using FluentAssertions;

namespace Cnvs.Demo.TaskManagement.Tests;

public class When_getting_random_user
{
    private readonly UserRandomizer _userRandomizer;

    public When_getting_random_user()
    {
        _userRandomizer = new UserRandomizer();
    }

    [Fact]
    public void Should_Return_RandomUser_When_Users_Available()
    {
        var users = A.CollectionOfDummy<User>(5);

        var user = _userRandomizer.GetRandomUser(users);

        user.Should().NotBe(NullUser.Instance);
    }

    [Fact]
    public void Should_Return_NullUser_When_No_Users_Available()
    {
        IEnumerable<User> users = new List<User>();

        var user = _userRandomizer.GetRandomUser(users);

        user.Should().Be(NullUser.Instance);
    }
}