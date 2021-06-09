using System;
using System.Text;
using RabbitMQ.Client;
using Newtonsoft.Json;

namespace MessageProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Enter message to send.Press Esc to stop the application");
            string message = "";
            int counter = 0;
            while(message != "exit")
            {
                Console.WriteLine("Enter new message");
                message = Console.ReadLine();
                MessageModel mm = new MessageModel() { Id = counter++, Message = message };
                Messager(mm);
            }
        }

        static void Messager(MessageModel model)
        {
            var factory = new ConnectionFactory() { Uri = new Uri("amqp://admin:admin@10.100.14.41:5672") };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "card-file-queue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = JsonConvert.SerializeObject(model);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "card-file-queue",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }
        }
    }
}
