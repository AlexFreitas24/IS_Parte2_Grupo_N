using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

// Criar conexão com RabbitMQ
var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

// Declarar o exchange (deve existir)
await channel.ExchangeDeclareAsync(exchange: "topic_logs", type: ExchangeType.Topic);

// Criar fila anônima/temporária
var result = await channel.QueueDeclareAsync(queue: "", durable: false, exclusive: true, autoDelete: true);
var queueName = result.QueueName;

// Ligação da fila apenas com a routing key "peca.falha"
await channel.QueueBindAsync(queue: queueName, exchange: "topic_logs", routingKey: "peca.falha");

Console.WriteLine(" [*] À espera de mensagens de peças com falha. CTRL+C para sair.");

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var routingKey = ea.RoutingKey;

    Console.WriteLine($" [x] Peça com falha: {message}");
};

// Consumir mensagens da fila
await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);

// Manter a aplicação ativa
Console.ReadLine();



//iNVOCAR API DO 1º tRABALHO!