using Common.Lib.DataAccess;
using Xunit;

namespace Common.Lib.Tests
{
    public class GenericRepositoryTests
    {
        
        TestContext _context = new TestContext();

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
    }


}