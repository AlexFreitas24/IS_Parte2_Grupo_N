using System.Text;
using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.Reliable;

var streamSystem = await StreamSystem.Create(new StreamSystemConfig());

await streamSystem.CreateStream(new StreamSpec("hello_stream")
{
    MaxLengthBytes = 5_000_000_000
});

var producer = await Producer.Create(new ProducerConfig(streamSystem, "hello_stream"));

for (var i = 0; i < 1000; i++)
{
    // Send a message to the stream
    await producer.Send(new Message(Encoding.UTF8.GetBytes($"Hello, World {i}")));
    Console.WriteLine($"Sent message {i}");
    Thread.Sleep(1000);
}