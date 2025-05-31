using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProducaoAPI.Data;
using ProducaoAPI.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ProducaoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        string sqlConnectionString = "Data Source=LAPTOP-381C7S15\\MSSQLSERVER04;Initial Catalog=Producao;" +
            "Integrated Security= True;Connect Timeout = 30; " +
            "Encrypt=False; TrustServerCertificate=False;" +
            "ApplicationIntent= ReadWrite;" +
            "MultiSubnetFailover=False";

        // GET: api/Produto
        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                Console.WriteLine("GET /api/Produto called"); // Debug log
                List<Produto> produtos = new List<Produto>();

                using (SqlConnection con = new SqlConnection(sqlConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Produto", con))
                    {
                        cmd.CommandType = CommandType.Text; // Raw SQL instead of stored procedure
                                                            // Rest of the code remains the same
                    
                    con.Open();

                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Produto item = new Produto();

                            // Safely parse data (handle NULLs)
                            item.ID_Produto = reader["ID_Produto"] != DBNull.Value ? Convert.ToInt32(reader["ID_Produto"]) : 0;
                            item.Codigo_Peca = reader["Codigo_Peca"] != DBNull.Value ? reader["Codigo_Peca"].ToString() : string.Empty;
                            item.Data_Producao = reader["Data_Producao"] != DBNull.Value ? Convert.ToDateTime(reader["Data_Producao"]) : DateTime.MinValue;

                            // Handle TimeSpan parsing safely
                            if (reader["Hora_Producao"] != DBNull.Value)
                            {
                                string horaStr = reader["Hora_Producao"].ToString();
                                if (TimeSpan.TryParse(horaStr, out TimeSpan horaProducao))
                                {
                                    item.Hora_Producao = horaProducao;
                                }
                                else
                                {
                                    item.Hora_Producao = TimeSpan.Zero; // Default if parsing fails
                                }
                            }

                            item.Tempo_Producao = reader["Tempo_Producao"] != DBNull.Value ? Convert.ToInt32(reader["Tempo_Producao"]) : 0;

                            produtos.Add(item);
                        }
                        con.Close();

                        Console.WriteLine($"Returning {produtos.Count} produtos"); // Debug log
                        return Ok(produtos);
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}"); // Debug log
                return BadRequest($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}"); // Debug log
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("{codigoPeca}")]
        public IActionResult GetProduto(string codigoPeca)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(sqlConnectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_ObterProdutoPorCodigo", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Codigo_Peca", codigoPeca);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var produto = new Produto
                                {
                                    ID_Produto = reader.GetInt32(reader.GetOrdinal("ID_Produto")),
                                    Codigo_Peca = reader.GetString(reader.GetOrdinal("Codigo_Peca")),
                                    Data_Producao = reader.GetDateTime(reader.GetOrdinal("Data_Producao")),
                                    Hora_Producao = reader.GetTimeSpan(reader.GetOrdinal("Hora_Producao")),
                                    Tempo_Producao = reader.GetInt32(reader.GetOrdinal("Tempo_Producao"))
                                };

                                return Ok(produto);
                            }
                            else
                            {
                                return NotFound($"Produto com código '{codigoPeca}' não encontrado.");
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return BadRequest($"Erro ao buscar produto: {ex.Message}");
            }
        }

        [HttpPost]
        public ActionResult Post([FromBody] Produto produto)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(sqlConnectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_InserirProduto", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Codigo_Peca", produto.Codigo_Peca);
                        cmd.Parameters.AddWithValue("@Data_Producao", produto.Data_Producao);

                        // Fix TimeSpan serialization
                        cmd.Parameters.AddWithValue("@Hora_Producao", produto.Hora_Producao.ToString(@"hh\:mm\:ss"));

                        cmd.Parameters.AddWithValue("@Tempo_Producao", produto.Tempo_Producao);

                        cmd.ExecuteNonQuery();
                        return Ok("Produto inserido com sucesso!");
                    }
                }
            }
            catch (SqlException ex)
            {
                return BadRequest($"Erro ao inserir produto: {ex.Message}");
            }
        }
        // PUT api/Produto/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Produto body)
        {
            try
            {
                Console.Write("PUT Request");

                using (SqlConnection con = new SqlConnection(sqlConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("AtualizarProduto", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;


                        // Parâmetros para a stored procedure de atualização
                        cmd.Parameters.AddWithValue("@ID_Produto", id); // ID do produto para identificar qual produto atualizar
                        cmd.Parameters.AddWithValue("@Codigo_Peca", body.Codigo_Peca);
                        cmd.Parameters.AddWithValue("@Data_Producao", body.Data_Producao);
                        cmd.Parameters.AddWithValue("@Hora_Producao", body.Hora_Producao);
                        cmd.Parameters.AddWithValue("@Tempo_Producao", body.Tempo_Producao);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();

                        return Ok("Produto atualizado com sucesso!"); // Retorna sucesso
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Erro SQL: " + ex.Message);
                return BadRequest("Erro ao inserir produto: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro geral: " + ex.Message);
                return BadRequest("Erro inesperado: " + ex.Message);
            }
        }
        // DELETE api/Produto/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                Console.Write("DELETE Request");

                using (SqlConnection con = new SqlConnection(sqlConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "RemoverProduto"; // Nome da stored procedure para deletar produto
                        cmd.Connection = con;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        // Parâmetro para deletar o produto com base no ID_Produto
                        cmd.Parameters.AddWithValue("@ID_Produto", id); // ID do produto que será deletado

                        con.Open();
                        cmd.ExecuteNonQuery(); // Executa a exclusão
                        con.Close();

                        return Ok("Produto eliminado com sucesso!"); // Retorna sucesso
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest(); // Retorna erro em caso de falha na execução
            }
        }



    }
}
