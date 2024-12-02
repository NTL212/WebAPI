using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductDataAccess.Models.Response
{
    public class OperationResult
    {
        public bool IsSuccess { get; }
        public string ErrorMessage { get; }

        public OperationResult(bool isSuccess, string errorMessage = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }
    }

}
