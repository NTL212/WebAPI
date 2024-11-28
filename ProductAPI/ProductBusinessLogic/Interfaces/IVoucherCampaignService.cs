using ProductDataAccess.DTOs;
using ProductDataAccess.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductBusinessLogic.Interfaces
{
    public interface IVoucherCampaignService:IBaseService<VoucherCampaignDTO>
    {
        Task<bool> CreateCampaign(CampaignVM cVM);
        Task<bool> UpdateCampaign(CampaignVM cVM);
        Task<VoucherCampaignDTO> GetCampaignWithVoucher(int id);

        Task<CampaignVM> GetCampaign(int id);
    }
}
