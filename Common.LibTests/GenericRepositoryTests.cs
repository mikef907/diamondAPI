using Common.Lib.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Common.Lib.Tests
{
    public class GenericRepositoryTests : IDisposable
    {

        private DbContextOptions _options;
        private TestContext _context;

        public GenericRepositoryTests() {
            _options = SqliteInMemory.CreateOptions<TestContext>();
            _context = new TestContext(_options);
            _context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
        }


        [Fact]
        public void Create_Success()
        {
            var repo = new GenericRepository<TestModelA>(_context);

            var user = new TestModelA();

            repo.Create(user);
            _context.SaveChanges();

            Assert.Equal(1, repo.Count());
        }

        [Fact]
        public void Find_Success()
        {
            var repo = new GenericRepository<TestModelA>(_context);

            var user = new TestModelA();

            var testModel = repo.Create(user);

            Assert.Equal(testModel, repo.Find(testModel.Id));
        }

        [Fact]
        public void Where_Success()
        {
            using (var context = new TestContext(_options)) { 
                TestContext.GenerateData(context, 10);
            }

            var repo = new GenericRepository<TestModelA>(_context);

            var model = repo.Get().First();

            var result = repo.Get(x => x.Id == model.Id, true).Single();

            Assert.Equal(model.Id, result.Id);
        }
        
        [Fact]
        public void WhereInclude_Fail()
        {
            using (var context = new TestContext(_options))
                TestContext.GenerateData(context, 10);

   
            var repo = new GenericRepository<TestModelA>(_context);

            var model = repo.Get().First();

            var result = repo.Get(x => x.Id == model.Id, true).Single();

            Assert.Equal(model.Id, result.Id);
            Assert.Null(result.TestModelB);
         }
        
        [Fact]
        public void WhereInclude_Success()
        {
            using (var context = new TestContext(_options))
                TestContext.GenerateData(context, 10);

            var repo = new GenericRepository<TestModelA>(_context);

            var model = repo.Get().First();

            var result = repo.Get(x => x.Include(a => a.TestModelB), x => x.Id == model.Id).Single();

            Assert.Equal(model.Id, result.Id);
            Assert.NotNull(result.TestModelB);
        }
        
        [Fact]
        public void WhereThenInclude_Success()
        {
            using (var context = new TestContext(_options))
                TestContext.GenerateData(context, 10);

            var repo = new GenericRepository<TestModelA>(_context);

            var model = repo.Get().First();

            var result = repo.Get(x => x.Include(a => a.TestModelB).ThenInclude(b => b.TestModelC), x => x.Id == model.Id).Single();

            Assert.Equal(model.Id, result.Id);
            Assert.NotNull(result.TestModelB);
            Assert.NotNull(result.TestModelB.TestModelC);
        }

        [Fact]
        public void WhereThenIncludeCollection_Success()
        {
            using (var context = new TestContext(_options))
                TestContext.GenerateData(context, 10);

            var repo = new GenericRepository<TestModelA>(_context);

            var model = repo.Get().First();

            var result = repo.Get(x => x.Include(a => a.TestModelB).ThenInclude(b => b.TestModelC).ThenInclude(c => c.TestModelD), x => x.Id == model.Id).Single();

            Assert.Equal(model.Id, result.Id);
            Assert.NotNull(result.TestModelB);
            Assert.NotNull(result.TestModelB.TestModelC);
            Assert.NotNull(result.TestModelB.TestModelC.TestModelD);
        }
        
        [Fact]
        public void WhereThenIncludeCollectionMultiple_Success()
        {
            using (var context = new TestContext(_options))
                TestContext.GenerateData(context, 10);

            var repo = new GenericRepository<TestModelA>(_context);

            var model = repo.Get().First();

            var includes = new List<Func<IQueryable<TestModelA>, IIncludableQueryable<TestModelA, object>>>() {
                x => x.Include(a => a.TestModelB).ThenInclude(b => b.TestModelC).ThenInclude(c => c.TestModelD),
                x => x.Include(a => a.TestModelE)
            };

            var result = repo.Get(includes, x => x.Id == model.Id).Single();

            Assert.Equal(model.Id, result.Id);
            Assert.NotNull(result.TestModelB);
            Assert.NotNull(result.TestModelB.TestModelC);
            Assert.NotNull(result.TestModelB.TestModelC.TestModelD);
            Assert.NotNull(result.TestModelE);
        }
        
        [Fact]
        public void WhereThenIncludeCollectionOfCollections_Success()
        {
            using (var context = new TestContext(_options))
                TestContext.GenerateData(context, 10);

            var repo = new GenericRepository<TestModelA>(_context);

            var model = repo.Get().First();

            var result = repo.Get(x => x.Include(a => a.TestModelB).ThenInclude(b => b.TestModelC).ThenInclude(c => c.TestModelD).ThenInclude(d => d.TestModelF), x => x.Id == model.Id).Single();

            Assert.Equal(model.Id, result.Id);
            Assert.NotNull(result.TestModelB);
            Assert.NotNull(result.TestModelB.TestModelC);
            Assert.NotNull(result.TestModelB.TestModelC.TestModelD);

            result.TestModelB.TestModelC.TestModelD.ToList().ForEach(d => Assert.NotNull(d.TestModelF));
        }

        [Fact]
        public void WhereThenMultipleIncludeCollectionOfCollections_Success()
        {
            using (var context = new TestContext(_options))
                TestContext.GenerateData(context, 10);

            var repo = new GenericRepository<TestModelA>(_context);

            var model = repo.Get().First();

            var includes = new List<Func<IQueryable<TestModelA>, IIncludableQueryable<TestModelA, object>>>()
            {
                x => x.Include(a => a.TestModelB).ThenInclude(b => b.TestModelC).ThenInclude(c => c.TestModelD).ThenInclude(d => d.TestModelF),
                x => x.Include(a => a.TestModelE)
            };


            var result = repo.Get(includes, x => x.Id == model.Id).Single();

            Assert.Equal(model.Id, result.Id);
            Assert.NotNull(result.TestModelB);
            Assert.NotNull(result.TestModelB.TestModelC);
            Assert.NotNull(result.TestModelB.TestModelC.TestModelD);

            result.TestModelB.TestModelC.TestModelD.ToList().ForEach(d => Assert.NotNull(d.TestModelF));

            Assert.NotNull(result.TestModelE);
        }
        
    }
    

}