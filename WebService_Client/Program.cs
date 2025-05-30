using System;
using WebService_Client.ContabilidadeService;
using System.Globalization;


class Program
{
    static void Main(string[] args)
    {
        var client = new ContabilidadeSoapClient();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== MENU FINANCEIRO ===");
            Console.WriteLine("1. Peça com Maior Prejuízo");
            Console.WriteLine("2. Custo Total num Período");
            Console.WriteLine("3. Lucro Total num Período");
            Console.WriteLine("4. Prejuízo Total por Peça num Período");
            Console.WriteLine("5. Dados Financeiros por Código de Peça");
            Console.WriteLine("0. Sair");
            Console.Write("Escolha: ");
            var opcao = Console.ReadLine();

            try
            {
                switch (opcao)
                {
                    case "1":
                        var peca = client.PecaComMaiorPrejuizo();
                        Console.WriteLine($"Peça com maior prejuízo: {peca}");
                        break;

                    case "2":
                        var (d1, d2) = LerDatas();
                        var custo = client.ObterCustoTotal(d1, d2);
                        Console.WriteLine($"Custo total: {custo:C}");
                        break;

                    case "3":
                        var (d3, d4) = LerDatas();
                        var lucro = client.ObterLucroTotal(d3, d4);
                        Console.WriteLine($"Lucro total: {lucro:C}");
                        break;

                    case "4":
                        var (d5, d6) = LerDatas();
                        var prejuizos = client.ObterPrejuizoPorPeca(d5, d6);
                        foreach (var p in prejuizos)
                            Console.WriteLine($"{p.Codigo_Peca} → Prejuízo: {p.Prejuizo_Total:C}");
                        break;

                    case "5":
                        Console.Write("Código da peça: ");
                        var codigo = Console.ReadLine();
                        var info = client.ObterDadosPorPeca(codigo);
                        if (info != null)
                        {
                            Console.WriteLine($"Produto: {info.Codigo_Peca}");
                            Console.WriteLine($"Tempo Produção: {info.Tempo_Producao}s");
                            Console.WriteLine($"Custo: {info.Custo_Producao:C}");
                            Console.WriteLine($"Prejuízo: {info.Prejuizo:C}");
                            Console.WriteLine($"Lucro: {info.Lucro:C}");
                        }
                        else
                        {
                            Console.WriteLine("Peça não encontrada.");
                        }
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Opção inválida.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }

            Console.WriteLine("\nPressiona ENTER para continuar...");
            Console.ReadLine();
        }
    }

    static (DateTime, DateTime) LerDatas()
    {
        Console.Write("Data Início (yyyy-mm-dd): ");
        var data1 = DateTime.Parse(Console.ReadLine());
        Console.Write("Data Fim (yyyy-mm-dd): ");
        var data2 = DateTime.Parse(Console.ReadLine());
        return (data1, data2);
    }
}
