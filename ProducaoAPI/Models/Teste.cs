using System.ComponentModel.DataAnnotations;

namespace ProducaoAPI.Models
{
    public class Teste
    {
        public int ID_Teste { get; set; }
        public int ID_Produto { get; set; }
        public string Codigo_Resultado { get; set; }
        public DateTime Data_Teste { get; set; }
    }
}
