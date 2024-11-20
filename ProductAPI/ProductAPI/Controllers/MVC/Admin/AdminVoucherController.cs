
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductAPI.Filters;
using ProductDataAccess.Repositories;
using ProductDataAccess.Repositories.Interfaces;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models;
using ProductDataAccess.Models.Response;
using ProductDataAccess.ViewModels;


namespace ProductAPI.Controllers.MVC.Admin
{
    [JwtAuthorize("Admin")]
    [ServiceFilter(typeof(ValidateTokenAttribute))]
    public class AdminVoucherController : Controller
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepoisitory _userRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IVoucherUserRepository _voucherUserRepository;
        private readonly IVoucherCampaignRepository _voucherCampaignRepository;
        private readonly IVoucherAssignmentRepository _voucherAssignmentRepository;
        private readonly IMapper _mapper;
        public AdminVoucherController(IVoucherRepository voucherRepository, IProductRepository productRepository,
            ICategoryRepository categoryRepository, IUserRepoisitory userRepository,
            IUserGroupRepository userGroupRepository, IVoucherUserRepository voucherUserRepository,
            IVoucherCampaignRepository voucherCampaignRepository,
            IVoucherAssignmentRepository voucherAssignmentRepository, IMapper mapper)
        {
            _voucherRepository = voucherRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _userGroupRepository = userGroupRepository;
            _voucherUserRepository = voucherUserRepository;
            _voucherCampaignRepository = voucherCampaignRepository;
            _voucherAssignmentRepository = voucherAssignmentRepository;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(int page = 1, string searchText = "")
        {

            var vouchers = await _voucherRepository.GetPagedWithIncludeSearchAsync(page, 10, p => p.Code.ToLower().Contains(searchText.ToLower()));
            var voucherDTO = _mapper.Map<PagedResult<VoucherDTO>>(vouchers);
            return View(voucherDTO);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var products = await _productRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();

            VoucherCreateVM vm = new VoucherCreateVM();
            vm.Products = _mapper.Map<List<ProductDTO>>(products);
            vm.Categories = _mapper.Map<List<CategoryDTO>>(categories);
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Create(VoucherCreateVM createVM)
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

            var result = await _voucherRepository.AddAsync(voucher);
            if (result)
            {
                TempData["SuccessMessage"] = "Add voucher successfull";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to add voucher";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var products = await _productRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();
            var voucher = await _voucherRepository.GetByIdAsync(id);

            VoucherEditVM vm = _mapper.Map<VoucherEditVM>(voucher);
            try
            {
                var voucherCondition = JsonConvert.DeserializeObject<VoucherCondition>(voucher.Conditions);
                vm.GroupName = voucherCondition.GroupName;
                vm.MaxDiscountAmount = voucherCondition.MaxDiscountAmount;
                vm.MinOrderValue = voucherCondition.MinOrderValue;
                vm.ProductId = JsonConvert.SerializeObject(voucherCondition.ProductId);
            }
            catch
            {

            }
            vm.Products = _mapper.Map<List<ProductDTO>>(products);
            vm.Categories = _mapper.Map<List<CategoryDTO>>(categories);
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(VoucherEditVM createVM)
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

            var result = await _voucherRepository.UpdateAsync(voucher);
            if (result)
            {
                TempData["SuccessMessage"] = "Voucher edit successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to edit voucher";
            }
            return RedirectToAction("Detail", new { id = voucher.VoucherId });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {

            var voucher = await _voucherRepository.GetByIdAsync(id);
            var voucherDTO = _mapper.Map<VoucherDTO>(voucher);

            return View(voucherDTO);
        }

        public async Task<IActionResult> DistributeVoucher(int page = 1)
        {

            var vouchers = await _voucherRepository.GetPagedAsync(page, 10);
            var users = await _userRepository.GetAllWithPredicateIncludeAsync(u=>u.UserId!=1);
            var userGroups = await _userGroupRepository.GetAllAsync();
            var voucherDTO = _mapper.Map<PagedResult<VoucherDTO>>(vouchers);
            voucherDTO.Users = _mapper.Map<List<UserDTO>>(users);
            voucherDTO.UserGroups = _mapper.Map<List<GroupDTO>>(userGroups);
            return View(voucherDTO);
        }

        [HttpPost]
        public async Task<IActionResult> DistributeVoucher(DistributeVoucherVM model)
        {
            if (ModelState.IsValid)
            {
                var voucher = await _voucherRepository.GetByIdAsync(model.VoucherId);

                if (voucher == null)
                {
                    TempData["ErrorMessage"] = "Voucher not found!";
                    return RedirectToAction("Index");
                }

                if (await _voucherRepository.DistributeVoucher(voucher, model.Quantity, model.SelectedUserIds))
                {
                    TempData["SuccessMessage"] = "Voucher successfully distributed!";
                    return RedirectToAction("Index");
                }

            }

            TempData["ErrorMessage"] = "An error occurred. Please try again.";
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> GetUserVouchers(int campaignId =0, int userId = 0, int page = 1, int pageSize = 5, string searchKeyword = "")
        {
            // Lấy dữ liệu voucherUsers kèm thông tin Voucher từ repository
            var voucherUsers = await _voucherUserRepository.GetAllWithPredicateIncludeAsync(v => v.UserId == userId, v => v.Voucher);

            // Danh sách vouchers
            var vouchers = new PagedResult<VoucherDTO>();

            if (userId > 0)
            {
               var voucherOfUserList = await _voucherRepository.GetVouchersOfUserPaged(userId, page, pageSize);              
                // Lưu userId vào ViewBag để sử dụng trong Partial View
                ViewBag.UserId = userId;
                vouchers = _mapper.Map<PagedResult<VoucherDTO>>(voucherOfUserList);
                foreach(var item in voucherOfUserList.Items)
                {
                    var voucherRedeem = item.VoucherUsers.Where(v=>v.UserId==userId).Select(v=>(v.Quantity - v.TimesUsed)).FirstOrDefault();
                    vouchers.Items.Where(v => v.VoucherId == item.VoucherId).FirstOrDefault().ReedemQuantity = voucherRedeem;
                }
            }
            else
            {
                ViewBag.CampaignId = campaignId;
                var allVouchers = await _voucherRepository.GetPagedAsync(page, pageSize);
                vouchers = _mapper.Map<PagedResult<VoucherDTO>>(allVouchers);
            }
            // Trả về Partial View
            return PartialView("_UserVouchersModal", vouchers);
        }


        public async Task<IActionResult> ListCampaign(int page = 1)
        {
            var voucherCampaigns = await _voucherCampaignRepository.GetPagedAsync(page, 10);
            var voucherCampaignDtos = _mapper.Map<PagedResult<VoucherCampaignDTO>>(voucherCampaigns);
            return View(voucherCampaignDtos);
        }


        [HttpGet]
        public async Task<IActionResult> CreateCampaign()
        {
            var groups = await _userGroupRepository.GetAllAsync();

            CampaignVM vm = new CampaignVM();
            vm.Groups = _mapper.Map<List<GroupDTO>>(groups);    
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> CreateCampaign(CampaignVM createVM)
        {

            var campaign = _mapper.Map<VoucherCampaign>(createVM);
            if (campaign.EndDate < campaign.StartDate)
            {
                TempData["ErrorMessage"] = "Failed to add campaign";
                return RedirectToAction("ListCampaign");
            }

            var result = await _voucherCampaignRepository.AddAsync(campaign);
            if (result)
            {
                TempData["SuccessMessage"] = "Add campaign successfull";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to add campaign";
            }
            return RedirectToAction("ListCampaign");
        }
        public async Task<IActionResult> DetailCampaign(int id)
        {

            var campaign = await _voucherCampaignRepository.GetByIdAsync(id);
            var vouchers = await _voucherRepository.GetAllWithPredicateIncludeAsync(v=>v.VoucherAssignments.Any(va=>va.CampaignId==id));
            var campaignDTO = _mapper.Map<VoucherCampaignDTO>(campaign);
            campaignDTO.AssignedVouchers = _mapper.Map<List<VoucherDTO>>(vouchers);
            return View(campaignDTO);
        }


        [HttpGet]
        public async Task<IActionResult> EditCampaign(int id)
        {
            var campaign = await _voucherCampaignRepository.GetByIdAsync(id);
            var groups = await _userGroupRepository.GetAllAsync();
            var vm = _mapper.Map<CampaignVM>(campaign);
            vm.Groups = _mapper.Map<List<GroupDTO>>(groups);
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> EditCampaign(CampaignVM editVM)
        {

            var campaign = _mapper.Map<VoucherCampaign>(editVM);

            var result = await _voucherCampaignRepository.UpdateAsync(campaign);
            if (result)
            {
                TempData["SuccessMessage"] = "Update campaign successfull";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update campaign";
            }
            return RedirectToAction("ListCampaign");
        }

        [HttpPost]
        public async Task<IActionResult> ApplySelectedVouchers(List<int> voucherIds, int campaignId)
        {
            if (voucherIds == null || !voucherIds.Any() || campaignId==null)
            {
                return BadRequest("No vouchers selected.");
            }
            List<VoucherAssignment> listVA = new List<VoucherAssignment>(); 
            foreach (int voucherId in voucherIds)
            {
                VoucherAssignment va = new VoucherAssignment();
                va.VoucherId = voucherId;
                va.CampaignId = campaignId;
                listVA.Add(va);
            }
            var result = await _voucherAssignmentRepository.AddRangeAsync(listVA);
            if (result)
            {
                TempData["SuccessMessage"] = "Assign successfull";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to assign voucher";
            }

            return RedirectToAction("DetailCampaign", new {id=campaignId});
        }

        public async Task<IActionResult> DeteleVoucherUser(int voucherId, int userId)
        {
            var voucherUser = await _voucherUserRepository.GetByIdWithIncludeAsync(v=>v.VoucherId==voucherId && v.UserId==userId);
            var result = await _voucherUserRepository.DeleteDistributeVoucher(voucherUser.VoucherUserId);
            if (result)
            {
                TempData["SuccessMessage"] = "Delete successfull";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete voucher";
            }
            return RedirectToAction("Index", "AdminUser");
        }
    }
}
