using Microsoft.EntityFrameworkCore;
using System;
using System.Text.RegularExpressions;

namespace SnakeCase
{
    class Program
    {
        static void Main(string[] args)
        {
            using var db = new SampleContext();
            var script = db.Database.GenerateCreateScript();
            Console.WriteLine(script);
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

    public static class SnakeCase
    {
        public static void ToSnakeNames(this ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entity.GetTableName().ToSnakeCase();
                entity.SetTableName(tableName);

                foreach (var property in entity.GetProperties())
                {
                    var columnName = property.GetColumnName().ToSnakeCase();
                    property.SetColumnName(columnName);
                }

                foreach (var key in entity.GetKeys())
                {
                    var keyName = key.GetName().ToSnakeCase();
                    key.SetName(keyName);
                }

                foreach (var key in entity.GetForeignKeys())
                {
                    var foreignKeyName = key.GetConstraintName().ToSnakeCase();
                    key.SetConstraintName(foreignKeyName);
                }

                foreach (var index in entity.GetIndexes())
                {
                    var indexName = index.GetName().ToSnakeCase();
                    index.SetName(indexName);
                }
            }
        }

        private static string ToSnakeCase(this string name)
        {
            return string.IsNullOrWhiteSpace(name)
                ? name
                : Regex.Replace(
                    name,
                    @"([a-z0-9])([A-Z])",
                    "$1_$2",
                    RegexOptions.Compiled,
                    TimeSpan.FromSeconds(1)).ToLower();
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
