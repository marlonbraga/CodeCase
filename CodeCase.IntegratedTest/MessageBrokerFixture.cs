using Testcontainers.RabbitMq;

namespace CodeCase.IntegratedTest
{
    public class MessageBrokerFixture : IAsyncLifetime
    {
        private readonly RabbitMqContainer _rabbitMqContainer;
        public Uri Uri { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }

        public MessageBrokerFixture()
        {
            _rabbitMqContainer = new RabbitMqBuilder()
                .WithImage("rabbitmq:management") // Imagem com RabbitMQ Management UI
                .WithUsername("guest")     // Usuário RabbitMQ
                .WithPassword("guest")     // Senha RabbitMQ
                .WithPortBinding(5673, 5672) // Porta padrão do RabbitMQ (AMQP)
                .WithPortBinding(15673, 15672) // Porta padrão do RabbitMQ Management UI
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _rabbitMqContainer.StartAsync();

            var rabbitMqUri = _rabbitMqContainer.GetConnectionString();
            Uri = new Uri(rabbitMqUri);
            UserName = "guest";
            Password = "guest";
            Port = 5673;
        }

        public async Task DisposeAsync()
        {
            await _rabbitMqContainer.StopAsync();
        }
    }
}
