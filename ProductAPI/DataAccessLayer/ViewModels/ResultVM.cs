using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.ViewModels
{
    public class ResultVM
    {
        public bool IsSuccess { get; set; } 
        public string Message { get; set; }

        public ResultVM() { }
        public ResultVM(bool isSuccess, string message) 
        {
            IsSuccess = isSuccess;
            Message = message;
        }
    }
}
