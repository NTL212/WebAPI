using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductDataAccess.DTOs
{
    public class UserDTO
    {
        public int UserId { get; set; }

        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

        public bool? IsActive { get; set; }

        public int? GroupId { get; set; }
    }
}
