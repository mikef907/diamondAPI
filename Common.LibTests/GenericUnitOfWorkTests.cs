using Common.Lib.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;

namespace Common.Lib.Tests
{
    public class GenericUnitOfWorkTests: IDisposable
    {
        private IGenericUnitOfWork _uow;
        private DbContextOptions _options;
        private TestContext _context;

        public GenericUnitOfWorkTests()
        {
            _options = SqliteInMemory.CreateOptions<TestContext>();
            _context = new TestContext(_options);
            _context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
        }

        [Fact]
        public void ShouldOnlyReturnSingleInstanceOfRepo() {
            _uow = new GenericUnitOfWork(_context);

            var modela = _uow.Repo<TestModelA>().Create(new TestModelA());
            _uow.Repo<TestModelB>().Create(new TestModelB() { TestModelAId = modela.Id });

            Assert.Equal(0, _uow.Repo<TestModelA>().Count());
            Assert.Equal(0, _uow.Repo<TestModelB>().Count());

            _uow.Commit();

            Assert.Equal(1, _uow.Repo<TestModelA>().Count());
            Assert.Equal(1, _uow.Repo<TestModelB>().Count());
        }
    }
}
