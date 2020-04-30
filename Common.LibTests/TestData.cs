using AutoMapper.Configuration.Annotations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;

namespace Common.Lib.Tests
{

    public class SqliteInMemory
    {
        public static DbContextOptions CreateOptions<T>()
        where T : DbContext
        {
            //This creates the SQLite connection string to in-memory database
            var connectionStringBuilder = new SqliteConnectionStringBuilder
            { DataSource = ":memory:" };
            var connectionString = connectionStringBuilder.ToString();

            //This creates a SqliteConnectionwith that string
            var connection = new SqliteConnection(connectionString);

            //The connection MUST be opened here
            connection.Open();

            //Now we have the EF Core commands to create SQLite options
            var builder = new DbContextOptionsBuilder<T>();
            builder.UseSqlite(connection);

            return builder.Options;
        }
    }

    class TestContext : DbContext, IDisposable
    {
        public TestContext(DbContextOptions options) : base(options) { }
        public DbSet<TestModelA> TestModelASet { get; set; }
        public DbSet<TestModelB> TestModelBSet { get; set; }
        public DbSet<TestModelC> TestModelCSet { get; set; }
        public DbSet<TestModelD> TestModelDSet { get; set; }
        public DbSet<TestModelE> TestModelESet { get; set; }

        public DbSet<TestModelF> TestModelFSet { get; set; }

        public static void GenerateData(TestContext context, int iterations)
        {
            context.Database.EnsureCreated();
            for (int i = 0; i < iterations; i++)
            {
                var modelA = context.TestModelASet.Add(new TestModelA());
                var modelB = context.TestModelBSet.Add(new TestModelB() { TestModelAId = modelA.Entity.Id });
                var modelC = context.TestModelCSet.Add(new TestModelC() { TestModelBId = modelB.Entity.Id });

                var modelDList = new List<TestModelD>() {
                    new TestModelD() { TestModelCId = modelC.Entity.Id, TestModelF = new List<TestModelF> { new TestModelF() { }, new TestModelF() { } } },
                    new TestModelD() { TestModelCId = modelC.Entity.Id },
                    new TestModelD() { TestModelCId = modelC.Entity.Id },
                    new TestModelD() { TestModelCId = modelC.Entity.Id }
                };

                modelDList.ForEach(item => context.TestModelDSet.Add(item));

                var modelEList = new List<TestModelE>() {
                    new TestModelE() { TestModelAId = modelA.Entity.Id },
                    new TestModelE() { TestModelAId = modelA.Entity.Id },
                    new TestModelE() { TestModelAId = modelA.Entity.Id },
                    new TestModelE() { TestModelAId = modelA.Entity.Id }
                };

                modelEList.ForEach(item => context.TestModelESet.Add(item));
            }
            context.SaveChanges();
        }
    }

    internal class TestModelA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public TestModelB TestModelB { get; set; }
        public ICollection<TestModelE> TestModelE { get; set; }
    }

    internal class TestModelB
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid TestModelAId { get; set; }
        public TestModelC TestModelC { get; set; }
    }

    internal class TestModelC
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid TestModelBId { get; set; }
        public ICollection<TestModelD> TestModelD { get; set; }
    }

    internal class TestModelD
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid TestModelCId { get; set; }

        public ICollection<TestModelF> TestModelF { get; set; }
    }

    internal class TestModelE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid TestModelAId { get; set; }
    }

    internal class TestModelF
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid TestModelDId { get; set; }
    }
}