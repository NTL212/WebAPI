﻿using ProductDataAccess.DTOs;
using ProductDataAccess.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductDataAccess.ViewModels
{
    public class AdminUserListVM
    {
        public PagedResult<UserDTO> userDtos { get; set; } = new PagedResult<UserDTO>(); 
        public List<GroupDTO> groupDtos { get; set; } = new List<GroupDTO>();

    }
}