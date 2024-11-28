using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.ViewModels
{
    public class ValidateVoucherVM
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public ValidateVoucherVM() { }
        public ValidateVoucherVM(bool success, string message)
        {
            Success = success;
            Message = message;
        }   
    }
}
