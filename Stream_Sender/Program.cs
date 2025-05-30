using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.Reliable;
using System.Text;
using System.Text.Json;

// Configura o sistema de streams
var streamSystem = await StreamSystem.Create(new StreamSystemConfig());

// Cria um stream se ainda não existir
await streamSystem.CreateStream(new StreamSpec("pecas_stream")
{
    MaxLengthBytes = 5000000000
});

var producer = await Producer.Create(new ProducerConfig(streamSystem, "pecas_stream"));

// Geração de dados fictícios de produção
var prefixos = new[] { "aa", "ab", "ba", "bb" };
var resultados = new[] { "01", "02", "03", "04", "05", "06" };
var random = new Random();

for (int i = 0; i < 100; i++)
{
    var peca = new
    {
        Codigo_Peca = $"{prefixos[random.Next(0, 4)]}{random.Next(100000, 999999)}",
        Data_Producao = DateTime.Today.ToString("yyyy-MM-dd"),
        Hora_Producao = DateTime.Now.ToString("HH:mm:ss"),
        Tempo_Producao = random.Next(10, 51),
        Codigo_Resultado = resultados[random.Next(0, 6)]
    };

    var json = JsonSerializer.Serialize(peca);
    await producer.Send(new Message(Encoding.UTF8.GetBytes(json)));
    Console.WriteLine($"[SEND] {json}");
    await Task.Delay(500);
}
