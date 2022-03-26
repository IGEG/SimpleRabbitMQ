using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Producer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Task.Run(CreatTask(6000, "error"));
            Task.Run(CreatTask(10000, "info"));
            Task.Run(CreatTask(12000, "warning"));
            Console.ReadKey();
        }

        static Func<Task> CreatTask(int timeToSleep, string routingKey)
        {

            return () =>
            {
                var counter = 0;
                do
                {
                    int timeToSleepTo = new Random().Next(1000, timeToSleep);
                    Thread.Sleep(timeToSleepTo);
                    var factory = new ConnectionFactory() { HostName = "LocalHost" };
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);
                        string message = $"The message:{routingKey} from publisher {counter}";
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: "direct_logs", routingKey: "error", basicProperties: null, body: body);
                        Console.WriteLine($"The message:{routingKey} is sent to direct exchege {counter++}");

                    }
                }
                while (true);
            };
        }

    }
}

