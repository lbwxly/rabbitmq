using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace Subscribe
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "10.40.32.220", UserName="client", Password="abc123_" };
            using(var connection = factory.CreateConnection()){
                using(var channel = connection.CreateModel()){
                    string queueName = channel.QueueDeclare().QueueName;
                    channel.ExchangeDeclare("logs", ExchangeType.Fanout,false, false);
                    channel.QueueBind(queueName,"logs","");
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

                    channel.BasicConsume(queue: queueName,
                                 autoAck: false,
                                 consumer: consumer);
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
