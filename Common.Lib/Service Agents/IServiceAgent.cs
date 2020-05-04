using Microsoft.IdentityModel.Tokens;

namespace Common.Lib.ServiceAgent
{
    public interface IServiceAgent
    {
        bool HasValidToken();
    }
}