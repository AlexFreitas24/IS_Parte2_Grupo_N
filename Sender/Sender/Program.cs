using System.Text;
using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.Reliable;
using System.Text.Json;

var streamSystem = await StreamSystem.Create(new StreamSystemConfig());

await streamSystem.CreateStream(new StreamSpec("hello_stream")
{
    MaxLengthBytes = 5_000_000_000
});

var producer = await Producer.Create(new ProducerConfig(streamSystem, "hello_stream"));

var random = new Random();
var codigosResultado = new[] { "01", "02", "03", "04", "05", "06" };
var prefixos = new[] { "aa", "ab", "ba", "bb" };

for (var i = 0; i < 1000; i++)
{
    var peca = new
    {
        Codigo_Peca = $"{prefixos[random.Next(0, 4)]}{random.Next(100000, 999999)}",
        Data_Producao = DateTime.Today.ToString("yyyy-MM-dd"),
        Hora_Producao = DateTime.Now.ToString("HH:mm:ss"),
        Tempo_Producao = random.Next(10, 51),
        Codigo_Resultado = codigosResultado[random.Next(0, 6)]
    };

    var json = JsonSerializer.Serialize(peca);
    await producer.Send(new Message(Encoding.UTF8.GetBytes(json)));
    Console.WriteLine($"[SENT] {json}");
    Thread.Sleep(1000);
}



