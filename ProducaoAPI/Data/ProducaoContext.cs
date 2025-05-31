using Microsoft.EntityFrameworkCore;
using ProducaoAPI.Models;

namespace ProducaoAPI.Data
{
    public class ProducaoDbContext :DbContext
    {
        public ProducaoDbContext(DbContextOptions<ProducaoDbContext> options) : base(options) { }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Teste> Testes { get; set; }
    }
}
