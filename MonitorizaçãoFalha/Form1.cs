using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using System.Text.Json.Nodes;

namespace MonitorizaçãoFalha
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            _ = StartConsuming();
        }

        public async Task StartConsuming()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(exchange: "topic_logs", type: ExchangeType.Topic);

            var result = await channel.QueueDeclareAsync(queue: "", durable: false, exclusive: true, autoDelete: true);
            var queueName = result.QueueName;

            await channel.QueueBindAsync(queue: queueName, exchange: "topic_logs", routingKey: "peca.falha");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var json = JsonNode.Parse(message);

                    if (json is not null)
                    {
                        this.Invoke(new Action(() =>
                        {
                            var item = new ListViewItem(json["Codigo_Peca"]?.ToString());
                            item.SubItems.Add(json["Data_Producao"]?.ToString());
                            item.SubItems.Add(json["Hora_Producao"]?.ToString());
                            item.SubItems.Add(json["Tempo_Producao"]?.ToString());
                            item.SubItems.Add(json["Codigo_Resultado"]?.ToString());
                            listView1.Items.Add(item);
                        }));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao processar mensagem: {ex.Message}");
                }
            };

            await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);
        }


        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;

            listView1.Columns.Add("Código Peça", 150);
            listView1.Columns.Add("Data Produção", 120);
            listView1.Columns.Add("Hora Produção", 120);
            listView1.Columns.Add("Tempo Produção (s)", 150);
            listView1.Columns.Add("Código Resultado", 130);
        }
    }
} 