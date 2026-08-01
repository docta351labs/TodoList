using Xunit;

namespace TodoList.IntegrationTests.Fixtures;

[CollectionDefinition("IntegrationTests", DisableParallelization = true)]
public class IntegrationTestCollection : ICollectionFixture<TodoListApiFactory>
{
}
