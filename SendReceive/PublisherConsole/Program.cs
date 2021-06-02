using System;
using System.Text;
using RabbitMQ.Client;

namespace PublisherConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory(){ HostName = "localhost" };
            using(var connection = factory.CreateConnection())
            {
                using(var channel = connection.CreateModel()){
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    channel.QueueDeclare(queue: "hello",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
                    for(int index = 0; index < 100; index++){
                        string message = "Hello World! " + DateTime.Now.AddSeconds(index).ToString();
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                                    routingKey: "hello",
                                    basicProperties: properties,
                                    body: body);

                        Console.WriteLine(" [x] Sent {0}", message);
                    }
                }
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
