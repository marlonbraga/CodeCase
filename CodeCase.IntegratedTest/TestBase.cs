using CodeCase.Domain;
using CodeCase.Domain.Interfaces;
using CodeCase.Domain.Services;
using CodeCase.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;
using System.Data.Common;

namespace CodeCase.IntegratedTest
{
    [Collection("IntegratedTest")]
    public class TestBase : IAsyncLifetime
    {
        //private readonly DatabaseFixture _databaseFixture;
        protected readonly MessageBrokerFixture _messageBrokerFixture;
        //protected TurimBrainContext DbContext;
        protected IServiceProvider serviceProvider;
        IModel _channel;

        public TestBase(/*DatabaseFixture databaseFixture, */MessageBrokerFixture messageBrokerFixture)
        {
            //_databaseFixture = databaseFixture;
            _messageBrokerFixture = messageBrokerFixture;
        }

        public Task InitializeAsync()
        {
            //var dbContextOptions = new DbContextOptionsBuilder<TurimBrainContext>().UseSqlServer(_databaseFixture.DbConnection).Options;
            //DbContext = new TurimBrainContext(dbContextOptions);
            DependecyInjection();
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            //await _databaseFixture.Respawner.ResetAsync(_databaseFixture.DbConnection);
        }

        public void DependecyInjection()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    //services.AddHttpClient();
                    services.AddScoped<IHardSkillRepository, HardSkillRepository>();
                    services.AddScoped<IHandleHardSkillService, HandleHardSkillService>(provider =>
                    {
                        return new HandleHardSkillService(_messageBrokerFixture.Uri.ToString());
                    });
                    services.AddScoped<IModel>(provider =>
                    {
                        var factory = new ConnectionFactory { Uri = _messageBrokerFixture.Uri };
                        var _connection = factory.CreateConnection();
                        _channel = _connection.CreateModel();
                        return _channel;
                    });
                });

            var host = hostBuilder.Build();
            serviceProvider = host.Services;
            _ = host.Services.GetRequiredService<IHardSkillRepository>();
            _ = host.Services.GetRequiredService<IHandleHardSkillService>();
            _ = host.Services.GetRequiredService<IModel>();

            /*
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddHttpContextAccessor();
            //builder.Services.AddDbContext<TurimBrainContext>(options =>
            //{
            //    options.UseSqlServer(_databaseFixture.DbConnection); // Certifique-se de ajustar o banco de dados
            //});
            builder.Services.AddTransient<IMessageBrokerParameters, MessageBrokerParameters>(_ => new MessageBrokerParameters
            {
                Hostname = _messageBrokerFixture.Uri.Host,
                Username = _messageBrokerFixture.UserName,
                Password = _messageBrokerFixture.Password,
                //Port = _messageBrokerFixture.Port,

                //Hostname = "localhost",// builder.Configuration["RabbitMQ:Hostname"],
                //Username = "guest",//builder.Configuration["RabbitMQ:Username"],
                //Password = "guest",//builder.Configuration["RabbitMQ:Password"],
                ClientName = builder.Configuration["SystemName"],
                //Port = 15672,
            });
            builder.Services.AddSingleton<IForwarderHttpClientFactory, CustomForwarderHttpClientFactory>();
            //builder.Services.AddScoped<IHardSkillService, HardSkillService>();
            //builder.Services.AddScoped<IHardSkillRepository, HardSkillRepository>();
            //builder.Services.AddHttpClient<ITurimGlobalApiClient, TurimGlobalApiClient>().AddHttpMessageHandler<HttpClientXRayTracingHandler>();

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration["RedisConnectionString"];
                options.InstanceName = builder.Configuration["SystemName"];
            });

            var app = builder.Build();

            serviceProvider = app.Services;

            //_ = serviceProvider.GetRequiredService<IHardSkillRepository>();
            */
        }
    }
}
