using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.Reliable;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace GUI_Stream
{

    public partial class Form1 : Form
    {
        // Contadores para análise
        private int totalPecas = 0;
        private int totalOk = 0;
        private int totalFalhas = 0;
        private int somaTempoProducao = 0;

        public Form1()
        {
            InitializeComponent();
            _ = IniciarReceiverAsync();
        }

        private async Task IniciarReceiverAsync()
        {
            var streamSystem = await StreamSystem.Create(new StreamSystemConfig());

            // Garante que o stream existe
            await streamSystem.CreateStream(new StreamSpec("pecas_stream")
            {
                MaxLengthBytes = 5000000000
            });

            await Consumer.Create(new ConsumerConfig(
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
                        bool isOk = peca.Codigo_Resultado == "01";

                        // Atualiza estatísticas
                        totalPecas++;
                        if (isOk) totalOk++; else totalFalhas++;
                        somaTempoProducao += peca.Tempo_Producao;

                        Invoke(new Action(() =>
                        {
                            // Atualiza ListView
                            var item = new ListViewItem(peca.Codigo_Peca);
                            item.SubItems.Add(isOk ? "✔️ OK" : "❌ FALHA");
                            item.SubItems.Add(peca.Data_Producao);
                            item.SubItems.Add(peca.Hora_Producao);
                            item.SubItems.Add(peca.Tempo_Producao.ToString());
                            listViewPecas.Items.Add(item);

                            // Atualiza estatísticas
                            labelTotal.Text = $"Total: {totalPecas}";
                            labelOk.Text = $"OK: {totalOk}";
                            labelFalha.Text = $"Falhas: {totalFalhas}";
                            labelTempoMedio.Text = $"Média: {(totalPecas > 0 ? (somaTempoProducao / totalPecas) : 0)}s";
                        }));
                    }

                    await Task.CompletedTask;
                }
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listViewPecas.View = View.Details;
            listViewPecas.FullRowSelect = true;
            listViewPecas.GridLines = true;

            listViewPecas.Columns.Add("Código Peça", 120);
            listViewPecas.Columns.Add("Resultado", 80);
            listViewPecas.Columns.Add("Data Produção", 100);
            listViewPecas.Columns.Add("Hora Produção", 100);
            listViewPecas.Columns.Add("Tempo (s)", 80);
        }
    }

    public class Peca
    {
        public string Codigo_Peca { get; set; }
        public string Data_Producao { get; set; }
        public string Hora_Producao { get; set; }
        public int Tempo_Producao { get; set; }
        public string Codigo_Resultado { get; set; }
    }
}
