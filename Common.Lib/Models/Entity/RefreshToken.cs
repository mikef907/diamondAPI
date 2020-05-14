using System;
using System.ComponentModel.DataAnnotations;

namespace Common.Lib.Models.EM
{
    public class RefreshToken
    {
        [Key]
        public string Token { get; set; }
        public Guid JwtId { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public DateTime Expiry { get; set; }
        public string Description { get; set; }
        public string IP { get; set; }
        public bool InUse { get; set; }
        public bool Valid { get; set; }
    }
}
