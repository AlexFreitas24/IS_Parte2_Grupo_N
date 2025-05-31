using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProducaoAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProducaoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TesteController : ControllerBase
    {
        string sqlConnectionString = "Data Source=LAPTOP-381C7S15\\MSSQLSERVER04;Initial Catalog=Producao;" +
            "Integrated Security= True;Connect Timeout = 30; " +
            "Encrypt=False; TrustServerCertificate=False;" +
            "ApplicationIntent= ReadWrite;" +
            "MultiSubnetFailover=False";

        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                List<Teste> testes = new List<Teste>();
                using (SqlConnection con = new SqlConnection(sqlConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetTestes", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Teste item = new Teste
                            {
                                ID_Teste = reader["ID_Teste"] != DBNull.Value ? Convert.ToInt32(reader["ID_Teste"]) : 0,
                                ID_Produto = reader["ID_Produto"] != DBNull.Value ? Convert.ToInt32(reader["ID_Produto"]) : 0,
                                Codigo_Resultado = reader["Codigo_Resultado"] != DBNull.Value ? reader["Codigo_Resultado"].ToString() : string.Empty,
                                Data_Teste = reader["Data_Teste"] != DBNull.Value ? Convert.ToDateTime(reader["Data_Teste"]) : DateTime.MinValue
                            };
                            testes.Add(item);
                        }
                        return Ok(testes);
                    }
                }
            }
            catch (SqlException ex)
            {
                return BadRequest($"Database error: {ex.Message}");
            }
        }

        [HttpPost]
        public ActionResult Post([FromBody] Teste teste)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(sqlConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_inserirTeste", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Apenas estes 2 parâmetros (ajuste conforme sua SP)
                        cmd.Parameters.AddWithValue("@ID_Produto", teste.ID_Produto);
                        cmd.Parameters.AddWithValue("@Codigo_Resultado", teste.Codigo_Resultado);

                        // Se sua SP espera Data_Teste, adicione:
                        // cmd.Parameters.AddWithValue("@Data_Teste", teste.Data_Teste);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        return Ok("Teste inserido com sucesso!");
                    }
                }
            }
            catch (SqlException ex)
            {
                return BadRequest($"Erro ao inserir teste: {ex.Message}");
            }
        }
    }
}