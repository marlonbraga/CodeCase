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
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CodeCase.IntegratedTest;

public class HardSkillTest : TestBase
{
    public HardSkillTest(MessageBrokerFixture messageBrokerFixture) : base(messageBrokerFixture)
    {
    }

    [Fact]
    public async Task SendHardSkillAsync()
    {
        #region Arrange
        var channel = serviceProvider.GetRequiredService<IModel>();
        var hardSkillRepository = serviceProvider.GetRequiredService<IHardSkillRepository>();
        var handleHardSkillService = serviceProvider.GetRequiredService<IHandleHardSkillService>();
        HardSkillController controller = new (hardSkillRepository, handleHardSkillService);

        var taskCompletionSource = new TaskCompletionSource<string>();

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            taskCompletionSource.SetResult(message);
        };
        channel.BasicConsume(queue: "hello", autoAck: true, consumer: consumer);
        #endregion

        // Act
        controller.SendHardSkill("TestMsg");

        #region Assert
        int waitTime = 2000;
        var timeout = Task.Delay(waitTime);
        var receivedTask = taskCompletionSource.Task;

        var completedTask = await Task.WhenAny(receivedTask, timeout);
        if (completedTask == timeout)
        {
            Assert.Fail($"Timeout: A mensagem não foi recebida no tempo esperado: {waitTime/1000}s");
        }

        var receivedMessage = await receivedTask;
        Assert.Equal("TestMsg", receivedMessage);
        #endregion
    }
}
