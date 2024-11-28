using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.Request
{
    public class RabbitMessage
    {
        public string ActionType { get; set; }
        public object Payload { get; set; }
    }
}
