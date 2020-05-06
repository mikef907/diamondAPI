using Common.Lib.Models.Primitives;

namespace Common.Lib.Models.DM
{
    public class AccessTokenRequest
    {
        public GrantType Grant_Type { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

    }
}
