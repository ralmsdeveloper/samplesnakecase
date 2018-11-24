using Microsoft.EntityFrameworkCore;
using System;

namespace SnakeCase
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new SampleContext())
            {
                var script = db.Database.GenerateCreateScript();
            }  
        }
    }

    public sealed class SampleContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(
                    "Host=127.0.0.1;Username=postgres;Password=XXX;Database=TestSnake", 
                    _ => _.EnableRetryOnFailure());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelo)
        {
            modelo.Entity<TestSnakeCase>();

            // Aqui está nossa mágica!
            modelo.ToSnakeNames();
        }
    }

    public class TestSnakeCase
    {
        public int Id { get; set; }
        public int CodigoIBGE { get; set; }
        public string NomeCompleto { get; set; } 
        public int AnoNascimento { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
