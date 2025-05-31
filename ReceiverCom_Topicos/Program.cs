using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Net.Http.Json;
using System.Text.Json;

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
HttpClient httpClient = new HttpClient();
var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var routingKey = ea.RoutingKey;

    Console.WriteLine($"Recebido: {message}");

    try
    {
        var peca = JsonSerializer.Deserialize<Produto>(message);
        if (peca == null)
        {
            Console.WriteLine("Erro: peça desserializada é null");
            return;
        }

        // Monta o produto para enviar para API
        var produto = new
        {
            Codigo_Peca = peca.Codigo_Peca,
            Data_Producao = peca.Data_Producao,
            Hora_Producao = peca.Hora_Producao,
            Tempo_Producao = peca.Tempo_Producao
        };

        Console.WriteLine($"Produto a inserir:\n" +
                          $"  -> Codigo_Peca: {produto.Codigo_Peca}\n" +
                          $"  -> Data_Producao: {produto.Data_Producao}\n" +
                          $"  -> Hora_Producao: {produto.Hora_Producao}\n" +
                          $"  -> Tempo_Producao: {produto.Tempo_Producao}");

        // POST Produto
        var produtoJson = JsonSerializer.Serialize(produto);
        var contentProduto = new StringContent(produtoJson, Encoding.UTF8, "application/json");

        var postProdutoResponse = await httpClient.PostAsync("http://localhost:5180/api/Produto", contentProduto);
        if (!postProdutoResponse.IsSuccessStatusCode)
        {
            Console.WriteLine($"Erro ao inserir produto: {postProdutoResponse.StatusCode}");
            return;
        }

        Console.WriteLine("Resposta da API Produto: Produto inserido com sucesso!");

        // Agora faz GET para buscar o produto inserido com o ID, usando Codigo_Peca
        // Agora faz o GET do produto inserido e mostra na consola
        var getResponse = await httpClient.GetAsync($"http://localhost:5180/api/Produto/{produto.Codigo_Peca}");
        if (!getResponse.IsSuccessStatusCode)
        {
            Console.WriteLine($"Erro ao buscar produto inserido: {getResponse.StatusCode}");
            return;
        }

        var produtoInseridoJson = await getResponse.Content.ReadAsStringAsync();
        var produtoInserido = JsonSerializer.Deserialize<Produto>(produtoInseridoJson);

        if (produtoInserido == null || produtoInserido.ID_Produto == 0)
        {
            Console.WriteLine("❌ Produto inserido mas não retornou ID_Produto válido.");
            return;
        }

        Console.WriteLine($"Produto inserido com ID: {produtoInserido.ID_Produto}");
        using var client = new HttpClient();
        string urlGet = $"http://localhost:5000/api/produto/{produtoInserido.Codigo_Peca}";
        HttpResponseMessage respostaGet = await client.GetAsync(urlGet);

        if (respostaGet.IsSuccessStatusCode)
        {
            string jsonProduto = await respostaGet.Content.ReadAsStringAsync();
            Console.WriteLine("Resposta do GET produto:");
            Console.WriteLine(jsonProduto);
        }
        else
        {
            Console.WriteLine($"Erro no GET produto: {respostaGet.StatusCode} - {respostaGet.ReasonPhrase}");
        }

        // Criar e enviar o teste
        var teste = new Teste
        {
            ID_Produto = produtoInserido.ID_Produto,
            Codigo_Resultado = peca.Codigo_Resultado,
            Data_Teste = DateTime.Now
        };

        var testeJson = JsonSerializer.Serialize(teste);
        var contentTeste = new StringContent(testeJson, Encoding.UTF8, "application/json");

        var postTesteResponse = await httpClient.PostAsync("http://localhost:5180/api/Teste", contentTeste);
        if (postTesteResponse.IsSuccessStatusCode)
        {
            Console.WriteLine("Teste inserido com sucesso.");
        }
        else
        {
            Console.WriteLine($"Erro ao inserir teste: {postTesteResponse.StatusCode}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro no processamento da mensagem: {ex.Message}");
    }
};





// Consumir mensagens da fila
await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);

// Manter a aplicação ativa
Console.ReadLine();

//Colocar API A DAR RUN
public class Produto
{
    public string Codigo_Resultado { get; set; }
    public int ID_Produto { get; set; }
    public string Codigo_Peca { get; set; }
    public DateTime Data_Producao { get; set; }
    public TimeSpan Hora_Producao { get; set; }
    public int Tempo_Producao { get; set; }
}

public class Teste
{
    public int ID_Produto { get; set; }
    public string Codigo_Resultado { get; set; }
    public DateTime Data_Teste { get; set; } = DateTime.Now;
}
public class PecaMensagem
{
    public string Codigo_Peca { get; set; }
    public string Data_Producao { get; set; } // string no formato "2025-05-31"
    public string Hora_Producao { get; set; } // string no formato "17:36:12"
    public int Tempo_Producao { get; set; }
    public string Codigo_Resultado { get; set; }
}

