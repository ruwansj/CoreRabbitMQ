using System;
using RabbitMQ.Client;
using System.Text;


        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();


        await channel.QueueDeclareAsync(queue: "pizza_orders",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            while (true)
            {
                Console.Write("Enter pizza order: ");
                string message = Console.ReadLine();
                var body = Encoding.UTF8.GetBytes(message);

                await channel.BasicPublishAsync(exchange: "",
                                     routingKey: "pizza_orders",
                                     body: body);

                Console.WriteLine($" [x] Sent order: {message}");
            }
   
