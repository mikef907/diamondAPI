using AutoMapper;
using Common.Lib.DataAccess;
using Common.Lib.Models;
using Common.Lib.Models.EM;
using Games.Lib;
using Microsoft.AspNetCore.SignalR;
using Moq;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Games.Web.Tests
{
    public class GameHubTests
    {
        private IGenericUnitOfWork _uow;
        private GamesContext _context = new GamesContext(SqliteInMemory.CreateOptions<GamesContext>());
        private IMapper _mapper = MapFactory.CreateGamesMapper();

        public GameHubTests() {
            _context.Database.EnsureCreated();
            _uow = new GenericUnitOfWork(_context);
        }

        [Fact]
        public async Task RespondToChallengeTest()
        {
            var hub = new GameHub(_uow, _mapper);

            var p1 = new Player() { Username = "P1" };
            var p2 = new Player() { Username = "P2" };

            _uow.Repo<Player>().Create(p1);
            _uow.Repo<Player>().Create(p2);
            _uow.Commit();

            Mock<HubCallerContext> hubContextMock = new Mock<HubCallerContext>();
            hubContextMock.Setup(context => context.User).Returns(CreateClaimsPrincipalFromPlayer(p1));
            hub.Context = hubContextMock.Object;
            Mock<IHubCallerClients> clientsMock = new Mock<IHubCallerClients>();
            Mock<IClientProxy> clientProxyMock = new Mock<IClientProxy>();
            clientsMock.Setup(clients => clients.Client(null)).Returns(clientProxyMock.Object);
            clientsMock.Setup(clients => clients.Others).Returns(clientProxyMock.Object);
            hub.Clients = clientsMock.Object;

            await hub.JoinLobby();

            await hub.RespondToChallenge(p2, true);

            var playerMatches = _uow.Repo<PlayerMatch>().Get();

            Assert.Equal(2, playerMatches.Count());
            Assert.NotNull(playerMatches.Single(p => p.PlayerId == p1.Id));
            Assert.NotNull(playerMatches.Single(p => p.PlayerId == p2.Id));

            var matchId = playerMatches.First().MatchId;

            Assert.True(playerMatches.All(p => p.MatchId == matchId));
        }

        private ClaimsPrincipal CreateClaimsPrincipalFromPlayer(Player player) {
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.Name, player.Id.ToString()));
            var principal = new ClaimsPrincipal();
            principal.AddIdentity(identity);
            return principal;
        }
    }
}