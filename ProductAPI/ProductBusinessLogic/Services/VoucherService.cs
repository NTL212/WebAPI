using AutoMapper;
using Newtonsoft.Json;
using ProductBusinessLogic.Interfaces;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models;
using ProductDataAccess.Models.Response;
using ProductDataAccess.Repositories;
using ProductDataAccess.Repositories.Interfaces;
using ProductDataAccess.ViewModels;


namespace ProductBusinessLogic.Services
{
    public class VoucherService : BaseService<Voucher, VoucherDTO>, IVoucherService
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IVoucherUserRepository _voucherUserRepository;
        private readonly IUserRepoisitory _userRepoisitory;
        private readonly IVoucherAssignmentRepository _voucherAssignmentRepository;
        private readonly IVoucherCampaignRepository _voucherCampaignRepository;
        public VoucherService(IMapper mapper, IVoucherRepository voucherRepository, IUserRepoisitory userRepoisitory, IVoucherUserRepository voucherUserRepository, IVoucherAssignmentRepository voucherAssignmentRepository, IVoucherCampaignRepository voucherCampaignRepository) : base(mapper, voucherRepository)
        {
            _voucherRepository = voucherRepository;
            _userRepoisitory = userRepoisitory;
            _voucherUserRepository = voucherUserRepository;
            _voucherAssignmentRepository = voucherAssignmentRepository;
            _voucherCampaignRepository = voucherCampaignRepository;
        }

        public async Task<VoucherEditVM> ConvertVoucherEditVM(VoucherDTO voucher)
        {
            VoucherEditVM vm = _mapper.Map<VoucherEditVM>(voucher);
            try
            {
                var voucherCondition = JsonConvert.DeserializeObject<VoucherCondition>(voucher.Conditions);
                vm.GroupName = voucherCondition.GroupName;
                vm.MaxDiscountAmount = voucherCondition.MaxDiscountAmount;
                vm.MinOrderValue = voucherCondition.MinOrderValue;
                vm.ProductId = JsonConvert.SerializeObject(voucherCondition.ProductId);
                return vm;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CreateVoucher(VoucherCreateVM createVM)
        {
            List<int> selectedProductIds = createVM.ProductId != null
            ? JsonConvert.DeserializeObject<List<int>>(createVM.ProductId) ?? new List<int>()
            : new List<int>();


            var voucherCondition = new VoucherCondition();

            voucherCondition.MinOrderValue = createVM.MinOrderValue ?? 0m; // Gán mặc định là 0 nếu null
            voucherCondition.MaxDiscountAmount = createVM.MaxDiscountAmount ?? 0m; // Gán mặc định là 0 nếu null
            voucherCondition.GroupName = createVM.GroupName ?? "Default Group Name"; // Gán mặc định nếu null
            voucherCondition.ProductId = selectedProductIds ?? new List<int>(); // Gán danh sách rỗng nếu null

            var voucher = _mapper.Map<Voucher>(createVM);
            voucher.Conditions = JsonConvert.SerializeObject(voucherCondition);
            await _voucherRepository.AddAsync(voucher);
            return await _voucherRepository.SaveChangesAsync();

        }

        public async Task<bool> DistributeVoucher(VoucherDTO voucherDTO, DistributeVoucherVM distributeVoucherVM)
        {

            var listUserId = JsonConvert.DeserializeObject<List<int>>(distributeVoucherVM.SelectedUserIds);

            var isValid = ValidateVoucher(voucherDTO, distributeVoucherVM, listUserId);
            if (!isValid) return false;


            var condition = JsonConvert.DeserializeObject<VoucherCondition>(voucherDTO.Conditions);

            var usersWithGroups = await _userRepoisitory.GetAllWithPredicateIncludeAsync(u => listUserId.Contains(u.UserId), u => u.Group);

            var existingVoucherUsers = await _voucherUserRepository.GetAllWithPredicateIncludeAsync(vu => vu.VoucherId == voucherDTO.VoucherId && listUserId.Contains(vu.UserId));


            var newVoucherUsers = new List<VoucherUser>();
            var updatedVoucherUsers = new List<VoucherUser>();

            // Xử lý từng người dùng
            foreach (var user in usersWithGroups)
            {
                var existingVoucher = existingVoucherUsers.FirstOrDefault(vu => vu.UserId == user.UserId);

                if (existingVoucher != null)
                {
                    ProcessExistingVoucherUser(existingVoucher, distributeVoucherVM.Quantity);
                    updatedVoucherUsers.Add(existingVoucher);
                }
                else
                {
                    // Kiểm tra điều kiện phân phối voucher
                    if (voucherDTO.Conditions == "All" || condition.GroupName == user.Group.GroupName)
                    {
                        var newVoucherUser = CreateNewVoucherUser(voucherDTO.VoucherId, user.UserId, distributeVoucherVM.Quantity);
                        newVoucherUsers.Add(newVoucherUser);
                    }
                }
            }

            if (!newVoucherUsers.Any() && !updatedVoucherUsers.Any())
            {
                return false;
            }

            // Cập nhật cơ sở dữ liệu
            await _voucherUserRepository.AddRangeAsync(newVoucherUsers);
            _voucherUserRepository.UpdateRange(updatedVoucherUsers);

            voucherDTO.UsedCount += distributeVoucherVM.Quantity * listUserId.Count;
            var voucher = _mapper.Map<Voucher>(voucherDTO); 
            _voucherRepository.Update(voucher);

            return await _voucherRepository.SaveChangesAsync();
        }

        public async Task<PagedResult<VoucherDTO>> GetVoucherPagedWithSearch(int pageNumber, int pageSize, string searchKey)
        {
            var vouchers = await _voucherRepository.GetPagedWithIncludeSearchAsync(pageNumber, pageSize, p => p.Code.ToLower().Contains(searchKey.ToLower()));
            var totalRecords = await _voucherRepository.CountAsync(p => p.Code.ToLower().Contains(searchKey.ToLower()));
            return new PagedResult<VoucherDTO>
            {
                Items = _mapper.Map<List<VoucherDTO>>(vouchers),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }

        public async Task<bool> UpdateVoucher(VoucherEditVM editVM)
        {
            List<int> selectedProductIds = editVM.ProductId != null
             ? JsonConvert.DeserializeObject<List<int>>(editVM.ProductId) ?? new List<int>()
             : new List<int>();


            var voucherCondition = new VoucherCondition();

            voucherCondition.MinOrderValue = editVM.MinOrderValue ?? 0m; // Gán mặc định là 0 nếu null
            voucherCondition.MaxDiscountAmount = editVM.MaxDiscountAmount ?? 0m; // Gán mặc định là 0 nếu null
            voucherCondition.GroupName = editVM.GroupName ?? "Default Group Name"; // Gán mặc định nếu null
            voucherCondition.ProductId = selectedProductIds ?? new List<int>(); // Gán danh sách rỗng nếu null


            var voucher = _mapper.Map<Voucher>(editVM);
            voucher.Conditions = JsonConvert.SerializeObject(voucherCondition);
            _voucherRepository.Update(voucher);
            return await _voucherRepository.SaveChangesAsync();
        }
        public async Task<PagedResult<VoucherDTO>> GetVouchersOfUserPaged(int pageNumber, int pageSize, int userId)
        {
            var voucherUsers = await _voucherUserRepository.GetPagedWithIncludeSearchAsync(pageNumber, pageSize, vu => vu.UserId == userId);
            List<VoucherDTO> voucherDTOs = new List<VoucherDTO>();
            foreach (var item in voucherUsers)
            {
                var voucher = await _voucherRepository.GetByIdAsync(item.VoucherId);
                voucherDTOs.Add(_mapper.Map<VoucherDTO>(voucher));
            }
            var totalRecords = await _voucherUserRepository.CountAsync(vu => vu.UserId == userId);
            return new PagedResult<VoucherDTO>
            {
                Items = voucherDTOs,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }

        public async Task UpdateVoucherUsageAsync(int voucherId)
        {
            var voucher = await _voucherRepository.GetByIdAsync(voucherId);
            if (voucher != null)
            {
                voucher.UsedCount++;
                await _voucherRepository.SaveChangesAsync();
            }
        }


        public override async Task<bool> DeleteAsync(int id)
        {
            var voucher = await _voucherRepository.GetByIdAsync(id);
            if (voucher == null)
            {
                return false;
            }
            voucher.Status = "Inactive";
            _voucherRepository.Update(voucher);
            return await _voucherRepository.SaveChangesAsync();
        }

        public async Task<ValidateVoucherVM> ValidateApplyVoucher(VoucherDTO voucher, UserDTO user, List<int> productIds, decimal totalOrder)
        {
            var voucherUser = await _voucherUserRepository.GetByIdWithIncludeAsync(x => x.VoucherId == voucher.VoucherId && x.UserId == user.UserId);
            var voucherAssigns = await _voucherAssignmentRepository.GetAllWithPredicateIncludeAsync(c => c.VoucherId == voucher.VoucherId);
            var campaigns = await _voucherCampaignRepository.GetAllWithPredicateIncludeAsync(c => voucherAssigns.Select(v => v.CampaignId).ToList().Contains(c.CampaignId));

            if (campaigns != null && campaigns.ToList().Count > 0)
            {
                foreach (var campaign in campaigns)
                {
                    if (campaign.Status == "Inactive")
                    {
                        return new ValidateVoucherVM(false, "The campaign not active.");
                    }
                    if (campaign.EndDate < DateTime.Now)
                    {
                        return new ValidateVoucherVM(false, "The campaign has expired.");
                    }

                    if (campaign.TargetAudience != "All" && campaign.TargetAudience != user.Group.GroupName)
                    {
                        return new ValidateVoucherVM(false, "The campagin of voucher is not valid for your user group.");
                    }
                }
            }

            if (voucherUser.Status == false)
            {
                return new ValidateVoucherVM(false, "The voucher not active.");
            }

            if (voucherUser.Quantity == voucherUser.TimesUsed)
            {
                return new ValidateVoucherVM(false, "The voucher out of stock");
            }


            if (voucher == null)
            {
                return new ValidateVoucherVM(false, "The voucher not exist.");
            }

            // Check expiry date
            if (voucher.ExpiryDate <= DateTime.Now)
            {
                return new ValidateVoucherVM(false, "The voucher has expired.");
            }

            // Check voucher conditions
            if (voucher.Conditions != "All")
            {
                var conditions = JsonConvert.DeserializeObject<VoucherCondition>(voucher.Conditions);

                // Check user group
                if (conditions.GroupName != "All" && conditions.GroupName != user.Group.GroupName)
                {
                    return new ValidateVoucherVM(false, "The voucher is not valid for your user group.");
                }

                // Check product list
                if (conditions.ProductId?.Any() == true && productIds.Any(p => !conditions.ProductId.Contains(p)))
                {
                    return new ValidateVoucherVM(false, "The voucher is not valid for some items in your cart.");
                }

                // Check minimum order value
                if (conditions.MinOrderValue > 0 && totalOrder < conditions.MinOrderValue)
                {
                    return new ValidateVoucherVM(false, $"Your order must be at least {conditions.MinOrderValue:C} to apply this voucher.");
                }

                // Calculate discount value
                decimal discount = voucher.DiscountType == "Amount"
                    ? (decimal)voucher.DiscountValue
                    : totalOrder * (decimal)voucher.DiscountValue;

                // Check maximum discount amount
                if (conditions.MaxDiscountAmount > 0 && discount > conditions.MaxDiscountAmount)
                {
                    return new ValidateVoucherVM(false, $"The voucher supports a maximum discount of {conditions.MaxDiscountAmount:C}.");
                }
            }

            // All conditions are valid
            return new ValidateVoucherVM(true, "The voucher is valid.");
        }


        public async Task<bool> IsVoucherValidAsync(string code)
        {
            var voucher = await _voucherRepository.GetByIdWithIncludeAsync(v=>v.Code==code);
            return voucher != null && voucher.UsedCount < voucher.MaxUsage;
        }
        private bool ValidateVoucher(VoucherDTO voucherDTO, DistributeVoucherVM distributeVoucherVM, List<int> userIds)
        {
            if (voucherDTO.MaxUsage < distributeVoucherVM.Quantity * userIds.Count)
                return false;

            if (voucherDTO.ExpiryDate < DateTime.Now)
                return false;

            if (voucherDTO.Status == "Inactive")
                return false;

            return true;
        }

        private void ProcessExistingVoucherUser(VoucherUser existingVoucherUser, int quantity)
        {
            existingVoucherUser.Quantity += quantity;
        }

        private VoucherUser CreateNewVoucherUser(int voucherId, int userId, int quantity)
        {
            return new VoucherUser
            {
                VoucherId = voucherId,
                UserId = userId,
                TimesUsed = 0,
                Quantity = quantity,
                DateAssigned = DateTime.Now,
                Status = true
            };
        }

        public async Task<VoucherDTO> GetVoucherByCode(string code)
        {
            var voucher = await _voucherRepository.GetVoucherByCodeAsync(code);
            return _mapper.Map<VoucherDTO>(voucher);
        }
    }
}
