using CodeCase.Domain;
using CodeCase.Domain.Interfaces;
using CodeCase.Domain.Services;
using CodeCase.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq.Protected;
using Moq;
using RabbitMQ.Client;
using System;
using System.Data.Common;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Yarp.ReverseProxy.Forwarder;

namespace CodeCase.IntegratedTest
{
    [Collection("IntegratedTest")]
    public class TestBase : IAsyncLifetime
    {
        protected const string baseAddress = "http://baseaddress.fake";
        IModel _channel;

        //private readonly DatabaseFixture _databaseFixture;
        private readonly MessageBrokerFixture _messageBrokerFixture;
        //private readonly CacheFixture _cacheFixture;
        //protected DatabaseContext DbContext;
        protected IServiceProvider serviceProvider;
        protected WebApplicationBuilder builder;

        public TestBase(/*DatabaseFixture databaseFixture, */MessageBrokerFixture messageBrokerFixture/*, CacheFixture cacheFixture*/)
        {
            //_databaseFixture = databaseFixture;
            _messageBrokerFixture = messageBrokerFixture;
            //_cacheFixture = cacheFixture;
        }

        public async Task InitializeAsync()
        {
            //var dbContextOptions = new DbContextOptionsBuilder<DatabaseContext>().UseSqlServer(_databaseFixture.DbConnection).Options;
            //DbContext = new DatabaseContext(dbContextOptions);
            await FeedBase();
            DependecyInjection();
        }
        private async Task FeedBase()
        {
            //DbContext.StatusAssociacao.Add(new StatusAssociacao() { Status = StatusAssociacao.StatusNomeAtivo });
            //DbContext.StatusAssociacao.Add(new StatusAssociacao() { Status = StatusAssociacao.StatusNomeTermoPendente });
            //DbContext.StatusAssociacao.Add(new StatusAssociacao() { Status = StatusAssociacao.StatusNomeInativo });

            //DbContext.MotivoCambio.Add(new MotivoCambio() { Descricao = "Aporte de Capital" });
            //DbContext.MotivoCambio.Add(new MotivoCambio() { Descricao = "Compra de Bens" });
            //DbContext.MotivoCambio.Add(new MotivoCambio() { Descricao = "Despesas Gerais" });
            //DbContext.MotivoCambio.Add(new MotivoCambio() { Descricao = "Doação" });
            //DbContext.MotivoCambio.Add(new MotivoCambio() { Descricao = "Pagamento de Empréstimo" });
            //DbContext.MotivoCambio.Add(new MotivoCambio() { Descricao = "Pagamento de Impostos" });
            //DbContext.MotivoCambio.Add(new MotivoCambio() { Descricao = "Investimento" });

            //DbContext.TipoOperacaoCambio.Add(new TipoOperacaoCambio() { Descricao = "Remessa" });
            //DbContext.TipoOperacaoCambio.Add(new TipoOperacaoCambio() { Descricao = "Internação" });

            //await DbContext.SaveChangesAsync();

            //DbContext.NaturezaOperacaoCambio.Add(new NaturezaOperacaoCambio() { Descricao = "Aumento de capital", TipoOperacaoCambioId = NaturezaOperacaoCambio.AumentoDeCapital, PossuiRelacaoComEmpresaExterior = true });
            //DbContext.NaturezaOperacaoCambio.Add(new NaturezaOperacaoCambio() { Descricao = "Disponibilidade no exterior", TipoOperacaoCambioId = NaturezaOperacaoCambio.AumentoDeCapital, PossuiRelacaoComEmpresaExterior = false });
            //DbContext.NaturezaOperacaoCambio.Add(new NaturezaOperacaoCambio() { Descricao = "Redução de capital", TipoOperacaoCambioId = NaturezaOperacaoCambio.DisponibilidadeNoExterior, PossuiRelacaoComEmpresaExterior = true });
            //DbContext.NaturezaOperacaoCambio.Add(new NaturezaOperacaoCambio() { Descricao = "Distribuição de dividendos", TipoOperacaoCambioId = NaturezaOperacaoCambio.DisponibilidadeNoExterior, PossuiRelacaoComEmpresaExterior = true });
            //DbContext.NaturezaOperacaoCambio.Add(new NaturezaOperacaoCambio() { Descricao = "Retorno de disponibilidade", TipoOperacaoCambioId = NaturezaOperacaoCambio.DisponibilidadeNoExterior, PossuiRelacaoComEmpresaExterior = false });
            //DbContext.NaturezaOperacaoCambio.Add(new NaturezaOperacaoCambio() { Descricao = "Investimento", TipoOperacaoCambioId = NaturezaOperacaoCambio.AumentoDeCapital, PossuiRelacaoComEmpresaExterior = false });

            //await DbContext.SaveChangesAsync();
        }

        public async Task DisposeAsync()
        {
            //await _databaseFixture.Respawner.ResetAsync(_databaseFixture.DbConnection);
            //await _cacheFixture.ResetStateAsync();
        }

        public void DependecyInjection()
        {
            var builder = WebApplication.CreateBuilder();

            #region Mock AppSettings
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["ApiBaseUrl"]).Returns(baseAddress);
            mockConfiguration.Setup(c => c["RabbitMQ:Queues:filaIndiana"]).Returns("fila_indiana");
            builder.Services.AddScoped(_ => mockConfiguration.Object);
            #endregion

            #region Classes
            builder.Services.AddScoped<IHardSkillRepository, HardSkillRepository>();
            builder.Services.AddScoped<IHandleHardSkillService, HandleHardSkillService>(provider =>
            {
                return new HandleHardSkillService(_messageBrokerFixture.Uri.ToString());
            });
            #endregion

            #region Mock HttpClient
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<IForwarderHttpClientFactory, CustomForwarderHttpClientFactory>();
            builder.Services.AddHttpClient<IApiClient, ApiClient>();
            #endregion

            #region Message Broker
            builder.Services.AddScoped<IModel>(provider =>
            {
                var factory = new ConnectionFactory { Uri = _messageBrokerFixture.Uri };
                var _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                return _channel;
            });
            #endregion

            #region SQL Server
            //builder.Services.AddDbContext<DatabaseContext>(options =>
            //{
            //    options.UseSqlServer(_databaseFixture.DbConnection); // Certifique-se de ajustar o banco de dados
            //});
            #endregion

            #region Redis
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration["RedisConnectionString"];
                options.InstanceName = builder.Configuration["SystemName"];
            });
            #endregion

            var app = builder.Build();
            serviceProvider = app.Services;

            //_ = serviceProvider.GetRequiredService<IHardSkillRepository>();
        }

        protected static Mock<HttpClientHandler> MockHttpClient(string pathUri, string content, HttpStatusCode statusCode, Mock<HttpClientHandler>? handlerMock = null)
        {
            if (pathUri.Length == 0)
            {
                return MockHttpClient(content, statusCode, handlerMock);
            }

            handlerMock ??= new Mock<HttpClientHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri == new Uri($"{baseAddress}{pathUri}")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content)
                })
                .Verifiable();

            return handlerMock;
        }

        protected static Mock<HttpClientHandler> MockHttpClient(string content, HttpStatusCode statusCode, Mock<HttpClientHandler>? handlerMock = null)
        {
            handlerMock ??= new Mock<HttpClientHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content)
                })
                .Verifiable();

            return handlerMock;
        }
    }

    public static class SerializerOptions
    {
        public static JsonSerializerOptions Options => new() { ReferenceHandler = ReferenceHandler.IgnoreCycles };
    }
}
