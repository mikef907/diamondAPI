using Common.Lib.Models.DM;
using Common.Lib.Service_Agents;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Common.Lib.ServiceAgent.Tests
{
    public class IdentityAgentTests
    {
        private IdentityAgent _agent;
        private IOptions<AppSettings> _appSettingsMock;
        private IHttpContextAccessor _contextAccessorMock = Mock.Of<IHttpContextAccessor>();

        public IdentityAgentTests() {
            
            var appSettings = new AppSettings() {
                IdentityURL = "http://test.com"
            };

            var appSettingsMock = new Mock<IOptions<AppSettings>>();
            appSettingsMock.Setup(o => o.Value).Returns(appSettings);
            _appSettingsMock = appSettingsMock.Object;
        }

        [Fact()]
        public async Task AuthenticateTest()
        {
            var testGuid = Guid.NewGuid();

            var handlerMock = new Mock<HttpMessageHandler>();
            // We can mock the abstracted classes proctected SendAsync method with some Moq foo
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage() {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(testGuid), Encoding.UTF8, "application/json")
                }).Verifiable();


            var stsToken = new JwtSecurityTokenHandler().CreateJwtSecurityToken();

            var authenticateModel = new AuthenticateModel()
            {
                Email = "test@test.com",
                Password = "Password01!"
            };

            var saFactory = new ServiceAgentFactory(new HttpClient(handlerMock.Object));

            _agent = new IdentityAgent(_appSettingsMock, _contextAccessorMock, saFactory);

            Guid? result = await _agent.Authenticate(authenticateModel, stsToken);

            Assert.NotNull(result);
            Assert.Equal(result, testGuid);

            // Verifies the sa called a POST 1 time to the expect URL
            handlerMock.Protected().Verify("SendAsync", Times.Exactly(1), ItExpr.Is<HttpRequestMessage>(req => 
                req.Method == HttpMethod.Post && req.RequestUri == new Uri($"{_appSettingsMock.Value.IdentityURL}token/authenticate")), ItExpr.IsAny<CancellationToken>());
        }

        //[Fact()]
        //public void CreateRefreshTokenTest()
        //{
        //    Assert.True(false, "This test needs an implementation");
        //}

        //[Fact()]
        //public void FetchRefreshTokenTest()
        //{
        //    Assert.True(false, "This test needs an implementation");
        //}

        //[Fact()]
        //public void RemoveRefreshTokenTest()
        //{
        //    Assert.True(false, "This test needs an implementation");
        //}
    }
}