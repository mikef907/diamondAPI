using Common.Lib.Models.Primitives;
using System;

namespace Common.Lib.Models.DM
{
    public class RefreshTokenRequest
    {
        public Guid Client_Id { get; set; }
        public GrantType Grant_Type { get; set; }
        public string Refresh_Token { get; set; }
        public string Client_Secret { get; set; }
    }
}
