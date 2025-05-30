using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.Reliable;
using System.Text;
using System.Text.Json;

// Conecta ao sistema de streams
var streamSystem = await StreamSystem.Create(new StreamSystemConfig());

// Garante que o stream existe
await streamSystem.CreateStream(new StreamSpec("pecas_stream")
{
    MaxLengthBytes = 5000000000
});

// Cria o consumidor
var consumer = await Consumer.Create(new ConsumerConfig(
    streamSystem,
    stream: "pecas_stream")
{
    OffsetSpec = new OffsetTypeFirst(),
    MessageHandler = async (stream, source, consumer, message) =>
    {
        var json = Encoding.UTF8.GetString(message.Data.Contents);
        var peca = JsonSerializer.Deserialize<Peca>(json);

        if (peca != null)
        {
            var status = peca.Codigo_Resultado == "01" ? "✔️ OK" : "❌ FALHA";
            Console.WriteLine($"[{status}] {peca.Codigo_Peca} | {peca.Codigo_Resultado}");
        }

        await Task.CompletedTask;
    }
});

Console.WriteLine("Stream a correr... ENTER para sair");
Console.ReadLine();

public class Peca
{
    public string Codigo_Peca { get; set; }
    public string Data_Producao { get; set; }
    public string Hora_Producao { get; set; }
    public int Tempo_Producao { get; set; }
    public string Codigo_Resultado { get; set; }
}
