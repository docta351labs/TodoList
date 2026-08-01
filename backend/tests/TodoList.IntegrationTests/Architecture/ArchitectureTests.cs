using FluentAssertions;
using MediatR;
using NetArchTest.Rules;
using TodoList.Application.Features.TodoLists.Commands.CreateTodoList;
using TodoList.Domain.Common;

namespace TodoList.IntegrationTests.Architecture;

public class ArchitectureTests
{
    private static readonly System.Reflection.Assembly DomainAssembly = typeof(AggregateRoot).Assembly;
    private static readonly System.Reflection.Assembly ApplicationAssembly = typeof(CreateTodoListCommand).Assembly;

    [Fact]
    public void Domain_ShouldNotReference_Infrastructure()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn("TodoList.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue("Domain layer must not depend on Infrastructure");
    }

    [Fact]
    public void Domain_ShouldNotReference_Application()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn("TodoList.Application")
            .GetResult();

        result.IsSuccessful.Should().BeTrue("Domain layer must not depend on Application");
    }

    [Fact]
    public void Application_ShouldNotReference_Infrastructure()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOn("TodoList.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue("Application layer must not depend on Infrastructure");
    }

    [Fact]
    public void Application_ShouldNotReference_Api()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOn("TodoList.Api")
            .GetResult();

        result.IsSuccessful.Should().BeTrue("Application layer must not depend on Api");
    }

    [Fact]
    public void AllCommandHandlers_ShouldImplement_IRequestHandlerInterface()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("CommandHandler")
            .Should()
            .ImplementInterface(typeof(IRequestHandler<,>))
            .Or()
            .ImplementInterface(typeof(IRequestHandler<>))
            .GetResult();

        result.IsSuccessful.Should().BeTrue("All command handlers must implement MediatR IRequestHandler interface");
    }

    [Fact]
    public void AllQueryHandlers_ShouldImplement_IRequestHandlerInterface()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("QueryHandler")
            .Should()
            .ImplementInterface(typeof(IRequestHandler<,>))
            .Or()
            .ImplementInterface(typeof(IRequestHandler<>))
            .GetResult();

        result.IsSuccessful.Should().BeTrue("All query handlers must implement MediatR IRequestHandler interface");
    }
}
