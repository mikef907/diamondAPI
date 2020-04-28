using Common.Lib.DataAccess;
using Xunit;

namespace Common.Lib.Tests
{
    public class GenericUnitOfWorkTests
    {
        private IGenericUnitOfWork _uow;

        [Fact]
        public void ShouldOnlyReturnSingleInstanceOfRepo() {
            _uow = new GenericUnitOfWork(new TestContext());

            _uow.Repo<TestModelA>().Create(new TestModelA());
            _uow.Repo<TestModelB>().Create(new TestModelB());

            Assert.Equal(0, _uow.Repo<TestModelA>().Count());
            Assert.Equal(0, _uow.Repo<TestModelB>().Count());

            _uow.Commit();

            Assert.Equal(1, _uow.Repo<TestModelA>().Count());
            Assert.Equal(1, _uow.Repo<TestModelB>().Count());
        }
    }
}
