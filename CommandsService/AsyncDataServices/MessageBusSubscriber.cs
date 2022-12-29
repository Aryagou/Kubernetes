using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandsService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _config;
        private readonly IEventProcessor _processor;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(IConfiguration config, IEventProcessor processor)
        {
            _config = config;
            _processor = processor;
            InitialiseRabbitMQ();
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (ModuleHandle, ea) => 
                {
                    Console.WriteLine("--> Event received");

                    var body = ea.Body;
                    var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                    _processor.ProcessEvent(notificationMessage); 
                };

                _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

                return Task.CompletedTask;
        }

        private void InitialiseRabbitMQ()
        {
            var factory = new ConnectionFactory() {
                HostName = _config["RabbitMQHost"],
                Port = int.Parse(_config["RabbitMQPort"])
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(
                queue: _queueName,
                exchange: "trigger",
                routingKey:""
            );

            Console.WriteLine("--> Listening on the Message Bus");

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutDown;
        }

        private void RabbitMQ_ConnectionShutDown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> Connection Shutdown");
        }

        public override void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
            base.Dispose();
        }
    }
}