using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductDataAccess.DTOs
{
    public class GroupDTO
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; } = null!;

        public string? Description { get; set; }
    }
}
