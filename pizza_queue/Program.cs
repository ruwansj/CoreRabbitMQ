using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;



var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(queue: "delivery_queue",
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var deliveryMessage = Encoding.UTF8.GetString(body);
    Console.WriteLine($"[x] Received {deliveryMessage}");

    Thread.Sleep(2000); // Simulating delivery time
    Console.WriteLine($"[x] Delivered: {deliveryMessage}");
    return Task.CompletedTask;
};

await channel.BasicConsumeAsync(queue: "delivery_queue",
                                autoAck: true,
                                consumer: consumer);

Console.WriteLine("Delivery service waiting for pizzas...");
Console.ReadLine();

