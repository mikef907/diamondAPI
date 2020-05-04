using Common.Lib.Models.DM;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace Common.Lib.ServiceAgent
{
    public interface IIdentityAgent
    {
        Task<Guid?> Authenticate(AuthenticateModel model, SecurityToken stsToken);
    }
}