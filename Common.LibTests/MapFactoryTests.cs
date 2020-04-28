using Common.Lib.Models;
using Xunit;

namespace Common.Lib.Tests
{
    public class MapFactoryTests
    {
        [Fact]
        public void Identity_Config_Success()
        {
            var config = MapFactory.CreateIdentityConfig();

            config.AssertConfigurationIsValid();

            var mapper = MapFactory.CreateIdentityMapper();

            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [Fact]
        public void Games_Config_Success()
        {
            var config = MapFactory.CreateGamesConfig();

            config.AssertConfigurationIsValid();

            var mapper = MapFactory.CreateIdentityMapper();

            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}