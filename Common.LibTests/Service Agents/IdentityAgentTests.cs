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
        private Mock<IOptions<AppSettings>> _appSettingsMock = new Mock<IOptions<AppSettings>>();
        private Mock<HttpMessageHandler> _handlerMock = new Mock<HttpMessageHandler>();
        private Mock<IHttpContextAccessor> _contextAccessorMock = new Mock<IHttpContextAccessor>();
        private JwtSecurityToken _stsToken = new JwtSecurityTokenHandler().CreateJwtSecurityToken();
        private AppSettings _appSettings = new AppSettings()
        {
            IdentityURL = "http://test.com"
        };

        public IdentityAgentTests() {
            _appSettingsMock.Setup(o => o.Value).Returns(_appSettings);
        }

        [Fact()]
        public async Task AuthenticateTest()
        {
            var testGuid = Guid.NewGuid();
            var authenticateModel = new AuthenticateModel()
            {
                Email = "test@test.com",
                Password = "Password01!"
            };

            SetupHandler(HttpStatusCode.OK, new StringContent(JsonConvert.SerializeObject(testGuid), Encoding.UTF8, "application/json"));

            var saFactory = new ServiceAgentFactory(new HttpClient(_handlerMock.Object));

            _agent = new IdentityAgent(_appSettingsMock.Object, _contextAccessorMock.Object, saFactory);

            Guid? result = await _agent.Authenticate(authenticateModel, _stsToken);

            Assert.NotNull(result);
            Assert.Equal(result, testGuid);

            // Verifies the sa called a POST 1 time to the expected URL
            _handlerMock.Protected().Verify("SendAsync", Times.Exactly(1), ItExpr.Is<HttpRequestMessage>(req => 
                req.Method == HttpMethod.Post && req.RequestUri == new Uri($"{_appSettingsMock.Object.Value.IdentityURL}token/authenticate")), ItExpr.IsAny<CancellationToken>());
        }

        [Fact()]
        public async Task CreateRefreshTokenTest()
        {
            RefreshToken refreshTokenMock = new RefreshToken();
           
            SetupHandler(HttpStatusCode.OK);

            var saFactory = new ServiceAgentFactory(new HttpClient(_handlerMock.Object));

            _agent = new IdentityAgent(_appSettingsMock.Object, _contextAccessorMock.Object, saFactory);

            await _agent.CreateRefreshToken(refreshTokenMock, _stsToken);

            // Verifies the sa called a POST 1 time to the expected URL
            _handlerMock.Protected().Verify("SendAsync", Times.Exactly(1), ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post && req.RequestUri == new Uri($"{_appSettingsMock.Object.Value.IdentityURL}token/refresh")), ItExpr.IsAny<CancellationToken>());
        }

        [Fact()]
        public async Task FetchRefreshTokenTest()
        {
            RefreshToken refreshTokenMock = new RefreshToken();
            Guid testUserId = Guid.NewGuid();
            Guid testJti = Guid.NewGuid();

            SetupHandler(HttpStatusCode.OK, new StringContent(JsonConvert.SerializeObject(refreshTokenMock), Encoding.UTF8, "application/json"));

            var saFactory = new ServiceAgentFactory(new HttpClient(_handlerMock.Object));

            _agent = new IdentityAgent(_appSettingsMock.Object, _contextAccessorMock.Object, saFactory);

            var result = await _agent.FetchRefreshToken(testUserId, testJti, _stsToken);

            // Verifies the sa called a POST 1 time to the expected URL
            _handlerMock.Protected().Verify("SendAsync", Times.Exactly(1), ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get && req.RequestUri == new Uri($"{_appSettingsMock.Object.Value.IdentityURL}token/refresh/{testUserId}/{testJti}")), ItExpr.IsAny<CancellationToken>());
        }

        //[Fact()]
        //public void RemoveRefreshTokenTest()
        //{
        //    Assert.True(false, "This test needs an implementation");
        //}

        private void SetupHandler(HttpStatusCode code, HttpContent content = null) {
            // We can mock the abstracted classes proctected SendAsync method with some Moq foo
            _handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = code,
                    Content = content
                }).Verifiable();
        }
    }
}