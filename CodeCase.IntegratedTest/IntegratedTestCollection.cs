namespace CodeCase.IntegratedTest;

[CollectionDefinition("IntegratedTest")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>, ICollectionFixture<MessageBrokerFixture>, ICollectionFixture<CacheFixture>
{
    // Essa classe não precisa conter nenhum código
    // Ela é usada apenas para agrupar a fixture à coleção
}
