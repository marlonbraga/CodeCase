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
using System.Net;
using System.Text;
using Moq;
using Moq.Protected;
using Microsoft.AspNetCore.Http;
using CodeCase.Domain.DTOs;
using System.Text.Json;
using System.Security.Claims;
using CodeCase.Domain.Entities;

namespace CodeCase.IntegratedTest;

public class HardSkillTest : TestBase
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
    private readonly Mock<HttpClientHandler> _handlerMock;

    public HardSkillTest(MessageBrokerFixture messageBrokerFixture) : base(messageBrokerFixture)
    {
        _httpContextAccessor = new();

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, "test user"),
            new Claim("token", "valid_token")
        }.ToArray();
        var identity = new ClaimsIdentity(claims, "Name");
        var userData = new UserData
        {
            Email = "user@email.com",
            Username = "uname",
            Name = "User",
            UserId = 1,
            Token = new Guid("21aafc9d-6839-472c-9ddd-81270c1206a6"),
        };
        identity.AddClaim(new Claim(ClaimTypes.UserData, JsonSerializer.Serialize(userData)));
        var principal = new ClaimsPrincipal(identity);

        _httpContextAccessor.Setup(a => a.HttpContext!.User).Returns(principal);
        _handlerMock = new Mock<HttpClientHandler>();
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

    public async Task SendSofkSkillAsync()
    {
        //Mock do HTTP Client
        var clienteAssociado10 = new DtoResponse()
        {
            ClienteId = 10,
            NumeroCliente = "SIGLA_ASSOCIADA_10"
        };
        var content = new BasePagedResponse<DtoResponse>()
        {
            Items = new List<DtoResponse>(1) { clienteAssociado10 },
            TotalCount = 1,
            PageSize = 10
        };

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(content))
            })
            .Verifiable();

        //Injeção de dependência
        builder.Services.AddHttpClient<IApiClient, ApiClient>()
            .ConfigureHttpClient(client => client.BaseAddress = new Uri(baseAddress))
            .ConfigurePrimaryHttpMessageHandler(() => _handlerMock.Object);
        builder.Services.AddSingleton<IHttpContextAccessor>(_ => _httpContextAccessor.Object);

        var app = builder.Build();
        serviceProvider = app.Services;
    }
}
