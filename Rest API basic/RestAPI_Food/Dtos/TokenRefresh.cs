using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace RestAPI_Food.Dtos
{
    public class TokenRefresh
    {
        [Required]
        public string Token { get; set; }

        [Required]

        public string RefreshToken { get; set; }
    }
}
