using ProductDataAccess.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductDataAccess.ViewModels
{
    public class CampaignVM
    {
        public int? CampaignId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string TargetAudience { get; set; }

        public string Status { get; set; }
        public List<GroupDTO> Groups { get; set; }
    }
}
