using RabbitMQ.Stream.Client.Reliable;
using RabbitMQ.Stream.Client;
using System.Text;
using System.Text.Json;

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
        var json = Encoding.UTF8.GetString(message.Data.Contents);
        var peca = JsonSerializer.Deserialize<Peca>(json);

        if (peca != null && peca.Codigo_Resultado != "01")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[FALHA] Código: {peca.Codigo_Peca} | Data: {peca.Data_Producao} | Resultado: {peca.Codigo_Resultado}");
            Console.ResetColor();
        }

        await Task.CompletedTask;
    }
});

Console.WriteLine("A receber dados de produção via Stream. Enter para sair.");
Console.ReadLine();

record Peca
{
    public string Codigo_Peca { get; set; }
    public string Data_Producao { get; set; }
    public string Hora_Producao { get; set; }
    public int Tempo_Producao { get; set; }
    public string Codigo_Resultado { get; set; }
}
