﻿using ProductDataAccess.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductDataAccess.ViewModels
{
    public class VoucherBaseVM
    {
        public List<ProductDTO> Products { get; set; } = new List<ProductDTO>();
        public List<CategoryDTO> Categories { get; set; } = new List<CategoryDTO>();
    }

}
