using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Consumer.Error
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "LocalHost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);
                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName, exchange: "direct_logs", routingKey: "error");
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, e) =>
                {
                    var body = e.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    Console.WriteLine($"Received message[{message}]");
                };
                channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
                Console.WriteLine($"Subscribe to the queue {queueName}");
                Console.ReadLine();

            }
        }

    
    }
}
