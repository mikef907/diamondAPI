using Common.Lib.Models.DM;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace Common.Lib.ServiceAgent
{
    public interface IIdentityAgent
    {
        Task<Guid?> Authenticate(AuthenticateModel model, SecurityToken stsToken);
        Task CreateRefreshToken(RefreshToken model, SecurityToken stsToken);
        Task<RefreshToken> FetchRefreshToken(Guid userId, Guid jti, SecurityToken stsToken);
        Task RemoveRefreshToken(string token, SecurityToken stsToken);
    }
}