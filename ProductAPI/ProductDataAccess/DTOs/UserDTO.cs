using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProductDataAccess.DTOs
{
    public class UserDTO
    {
        public int UserId { get; set; }

        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdated { get; set; }

        public bool? IsActive { get; set; }

        public int? GroupId { get; set; }

        public int RoleId { get; set; } 

        public GroupDTO Group { get; set; }
    }
}
