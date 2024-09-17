/*
TODO: List 0/10
1.[ ] injeção de dependência em projeto de teste integrado
2.[ ] docker compose para criação de banco de dados relacional (MsSQL)
3.[ ] implementação de Entity Framework Core
4.[ ] classes com funcionalidades Controllers, Repository (CRUD)
5.[ ] testContainers para simular ambiente de teste do MsSQL
6.[ ] docker compose para criação de fila de mensagens (RabbitMQ)
7.[X] testContainers para simular ambiente de teste do RabbitMQ
8.[ ] 
9.[ ] 
0.[ ] 
 */
using Case.TestContainers.Api.Controllers;
using CodeCase.Domain.Interfaces;
using CodeCase.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace CodeCase.IntegratedTest;

public class UnitTest1 : TestBase
{
    public UnitTest1(MessageBrokerFixture messageBrokerFixture) : base(messageBrokerFixture)
    {
    }

    [Fact]
    public void Test1()
    {
        var hardSkillRepository = serviceProvider.GetRequiredService<IHardSkillRepository>();
        var handleHardSkillService = serviceProvider.GetRequiredService<IHandleHardSkillService>();
        HardSkillController controller = new (hardSkillRepository, handleHardSkillService);

        controller.SendHardSkill("Hello World!");

        Assert.True(true);
    }
}
