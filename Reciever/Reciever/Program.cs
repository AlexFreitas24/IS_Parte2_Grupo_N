using RabbitMQ.Stream.Client.Reliable;
using RabbitMQ.Stream.Client;
using System.IO;
using System.Text;


var streamSystem = await StreamSystem.Create(new StreamSystemConfig());

await streamSystem.CreateStream(new StreamSpec("hello_stream")
{
    MaxLengthBytes = 5_000_000_000
});

var consumer = await Consumer.Create(new ConsumerConfig(streamSystem, "hello_stream")
{
    OffsetSpec = new OffsetTypeFirst(),
    MessageHandler = async (stream, _, _, message) =>
    {
        Console.WriteLine($"Stream: {stream} - " +
                          $"Received message: {Encoding.UTF8.GetString(message.Data.Contents)}");
        await Task.CompletedTask;
    }
});

Console.WriteLine("Press [enter] to exit.");
Console.ReadLine();