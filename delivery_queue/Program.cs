using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;


var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(queue: "pizza_orders",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

await channel.QueueDeclareAsync(queue: "delivery_queue",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var order = Encoding.UTF8.GetString(body);
    Console.WriteLine($"[x] Received order: {order}");

    Thread.Sleep(3000); // Simulating pizza preparation time
    Console.WriteLine($"[x] Pizza prepared: {order}");

    var deliveryMessage = Encoding.UTF8.GetBytes(order);
    await channel.BasicPublishAsync(exchange: "",
                         routingKey: "delivery_queue",
                         body: deliveryMessage);
    Console.WriteLine($"[x] Sent to delivery queue: {order}");
};
await channel.BasicConsumeAsync(queue: "pizza_orders",
                               autoAck: true,
                               consumer: consumer);

Console.WriteLine("Kitchen waiting for orders...");
Console.ReadLine();





  
