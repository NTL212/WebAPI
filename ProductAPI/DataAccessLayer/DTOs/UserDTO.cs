using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataAccessLayer.DTOs
{
    public class UserDTO
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Username { get; set; } = null!;
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdated { get; set; }

        public bool? IsActive { get; set; }

        public int? GroupId { get; set; }

        public int RoleId { get; set; } 

        public GroupDTO? Group { get; set; }
    }
}
