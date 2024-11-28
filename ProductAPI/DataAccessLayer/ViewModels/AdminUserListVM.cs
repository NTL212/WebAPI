using DataAccessLayer.DTOs;
using DataAccessLayer.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.ViewModels
{
    public class AdminUserListVM
    {
        public PagedResult<UserDTO> userDtos { get; set; } = new PagedResult<UserDTO>(); 
        public List<GroupDTO> groupDtos { get; set; } = new List<GroupDTO>();

    }
}
