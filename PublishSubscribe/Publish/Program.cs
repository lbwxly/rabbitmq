using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;

namespace Publish
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory(){ HostName = "10.40.32.220", UserName="client", Password="abc123_" };
            using(var connection = factory.CreateConnection())
            {
                using(var channel = connection.CreateModel()){
                    channel.ExchangeDeclare("logs",ExchangeType.Fanout,false,false);
                    int index = 0;
                    while(true)
                    {
                        index += 1;
                        string message = "Hello World! " + DateTime.Now.AddSeconds(index).ToString();
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "logs",
                                    routingKey: "",
                                    basicProperties: null,
                                    body: body);

                        Console.WriteLine(" [x] Sent {0}", message);
                        Thread.Sleep(30000);
                    }
                }
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
