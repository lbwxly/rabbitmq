using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace SubscriberConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "10.40.32.220" };
            using(var connection = factory.CreateConnection()){
                using(var channel = connection.CreateModel()){
                    channel.QueueDeclare(queue: "hello",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                    channel.BasicQos(0,2,false);
                    var consumer = new EventingBasicConsumer(channel);
                    int count = 0;
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        System.Threading.Thread.Sleep(1000);
                        channel.BasicAck(ea.DeliveryTag, false);
                        Console.WriteLine(" [x] Received {0} on thread {1}, count:{2}", message, Thread.CurrentThread.ManagedThreadId, ++count);
                    };

                    channel.BasicConsume(queue: "hello",
                                 autoAck: false,
                                 consumer: consumer);
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
