using System;

namespace Common.Lib.Models.DM
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public Guid JwtId { get; set; }
        public Guid UserId { get; set; }
        public DateTime Expiry { get; set; }
        public string IP { get; set; }
        public bool InUse { get; set; }
        public bool Valid { get; set; }
    }
}
