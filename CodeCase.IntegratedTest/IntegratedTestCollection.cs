using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCase.IntegratedTest;

[CollectionDefinition("IntegratedTest")]
public class IntegratedTestCollection : ICollectionFixture<MessageBrokerFixture>
{
    // Essa classe não precisa conter nenhum código
    // Ela é usada apenas para agrupar a fixture à coleção
}
