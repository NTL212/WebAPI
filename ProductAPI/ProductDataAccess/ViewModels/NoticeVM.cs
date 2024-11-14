using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductDataAccess.ViewModels
{
    public class NoticeVM
    {

        public bool Success { get; set; }
        public string Message { get; set; } = null!;

        public NoticeVM() { }

        public NoticeVM(bool success, string message) 
        { 
            Success = success;
            Message = message;  
        }
    }
}
