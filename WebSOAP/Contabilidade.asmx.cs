using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Services;

namespace WebSOAP
{
    public class Custos_Peca
    {
        public int ID_Custo { get; set; }
        public int ID_Produto { get; set; }
        public string Codigo_Peca { get; set; }

        public int Tempo_Producao { get; set; }

        public decimal Custo_Producao { get; set; }

        public decimal Prejuizo { get; set; }

        public decimal Lucro { get; set; }

    }
    public class PrejuizoPeca
    {
        public string Codigo_Peca { get; set; }
        public decimal Prejuizo_Total { get; set; }
    }




    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class Contabilidade : WebService
    {
        string connectionString = "Data Source=LAPTOP-381C7S15\\MSSQLSERVER04;Initial Catalog=Contabilidade;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        [WebMethod]
        public string PecaComMaiorPrejuizo()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_PecaMaiorPrejuizo", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return reader["Codigo_Peca"].ToString();
                }
                return null;
            }
        }

        [WebMethod]
        public decimal ObterCustoTotal(DateTime inicio, DateTime fim)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_CustosTotaisPeriodo", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DataInicio", inicio);
                cmd.Parameters.AddWithValue("@DataFim", fim);
                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result == DBNull.Value || result == null)
                    return 0;

                return Convert.ToDecimal(result);
            }
        }

        [WebMethod]
        public decimal ObterLucroTotal(DateTime inicio, DateTime fim)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_LucroTotalPeriodo", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DataInicio", inicio);
                cmd.Parameters.AddWithValue("@DataFim", fim);
                conn.Open();
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        [WebMethod]
        public List<PrejuizoPeca> ObterPrejuizoPorPeca(DateTime inicio, DateTime fim)
        {
            List<PrejuizoPeca> lista = new List<PrejuizoPeca>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_PrejuizoPorPecaPeriodo", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DataInicio", inicio);
                cmd.Parameters.AddWithValue("@DataFim", fim);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new PrejuizoPeca
                    {
                        Codigo_Peca = reader["Codigo_Peca"].ToString(),
                        Prejuizo_Total = Convert.ToDecimal(reader["Prejuizo_Total"])
                    });
                }
            }
            return lista;
        }

        [WebMethod]
        public Custos_Peca ObterDadosPorPeca(string codigoPeca)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_DadosFinanceirosPorPeca", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CodigoPeca", codigoPeca);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new Custos_Peca
                    {
                        ID_Custo = 0,
                        ID_Produto = Convert.ToInt32(reader["ID_Produto"]),
                        Codigo_Peca = reader["Codigo_Peca"].ToString(),
                        Tempo_Producao = Convert.ToInt32(reader["Tempo_Producao"]),
                        Custo_Producao = Convert.ToDecimal(reader["Custo_Producao"]),
                        Prejuizo = Convert.ToDecimal(reader["Prejuizo"]),
                        Lucro = Convert.ToDecimal(reader["Lucro"])
                    };
                }
            }
            return null;
        }
    }
}
