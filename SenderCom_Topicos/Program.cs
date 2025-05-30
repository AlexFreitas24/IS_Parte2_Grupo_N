using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

// Criação da conexão com RabbitMQ
var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

// Declara o exchange do tipo topic
await channel.ExchangeDeclareAsync(exchange: "topic_logs", type: ExchangeType.Topic);

// Simulação de peças
var codigosResultado = new[] { "01", "02", "03", "04", "05", "06" };
var prefixos = new[] { "aa", "ab", "ba", "bb" };
var random = new Random();

for (int i = 0; i < 10; i++) // reduzido para teste
{
    var peca = new
    {
        Codigo_Peca = $"{prefixos[random.Next(0, 4)]}{random.Next(100000, 999999)}",
        Data_Producao = DateTime.Today.ToString("yyyy-MM-dd"),
        Hora_Producao = DateTime.Now.ToString("HH:mm:ss"),
        Tempo_Producao = random.Next(10, 51),
        Codigo_Resultado = codigosResultado[random.Next(0, 6)]
    };

    var routingKey = peca.Codigo_Resultado == "01" ? "peca.ok" : "peca.falha";
    var json = JsonSerializer.Serialize(peca);
    var body = Encoding.UTF8.GetBytes(json);

    await channel.BasicPublishAsync(exchange: "topic_logs", routingKey: routingKey, body: body);
    Console.WriteLine($"[x] Enviado para '{routingKey}':{json}");

    Thread.Sleep(500);
}
