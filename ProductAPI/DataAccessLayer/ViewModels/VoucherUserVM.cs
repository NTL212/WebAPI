using DataAccessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.ViewModels
{
    public class VoucherUserVM
    {
        public UserDTO User { get; set; }
        public List<VoucherDTO> vouchers { get; set; }
    }
}
