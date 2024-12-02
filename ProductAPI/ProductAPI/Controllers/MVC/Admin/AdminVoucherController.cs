using Microsoft.AspNetCore.Mvc;
using ProductAPI.Filters;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models.Response;
using ProductDataAccess.ViewModels;
using ProductBusinessLogic.Interfaces;

namespace ProductAPI.Controllers.MVC.Admin
{
    [JwtAuthorize("Admin")]
    [ServiceFilter(typeof(ValidateTokenAttribute))]
    public class AdminVoucherController : Controller
    {
        private readonly IVoucherService _voucherService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly IUserGroupService _userGroupService;
        private readonly ICategoryService _categoryService;
        private readonly IVoucherUserService _voucherUserService;
        private readonly IVoucherCampaignService _voucherCampaignService;
        private readonly IVoucherAssignmentService _voucherAssignmentService;
        public AdminVoucherController(IVoucherService voucherService, IProductService productService,
            ICategoryService categoryService, IUserService userService,
            IUserGroupService userGroupService, IVoucherUserService voucherUserService,
            IVoucherCampaignService voucherCampaignService,
            IVoucherAssignmentService voucherAssignmentService)
        {
            _voucherService = voucherService;
            _productService = productService;
            _categoryService = categoryService;
            _userService = userService;
            _userGroupService = userGroupService;
            _voucherUserService = voucherUserService;
            _voucherCampaignService = voucherCampaignService;
            _voucherAssignmentService = voucherAssignmentService;
        }
        public async Task<IActionResult> Index(int page = 1, string searchText = "")
        {

            var vouchers = await _voucherService.GetVoucherPagedWithSearch(page, 10, searchText);
            return View(vouchers);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            VoucherCreateVM vm = new VoucherCreateVM();
            vm.Products = await _productService.GetAllAsync();
            vm.Categories = await _categoryService.GetAllAsync();
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Create(VoucherCreateVM createVM)
        {
             if (!ModelState.IsValid)
            {
                return await ReturnModelError(createVM);
            }

            if (createVM.ExpiryDate < DateTime.Now)
            {
                ModelState.AddModelError("", "The expiry date is not a valid.");
                return await ReturnModelError(createVM);
            }
            
            var result = await _voucherService.CreateVoucher(createVM);
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
            var voucher = await _voucherService.GetByIdAsync(id);
            var vm = await _voucherService.ConvertVoucherEditVM(voucher);           
            vm.Products = await _productService.GetAllAsync();
            vm.Categories = await _categoryService.GetAllAsync();
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(VoucherEditVM editVM)
        {
            if (!ModelState.IsValid)
            {
                return await ReturnModelError(editVM);
            }

            if(editVM.ExpiryDate < DateTime.Now)
            {
                ModelState.AddModelError("", "The expiry date is not a valid.");
                return await ReturnModelError(editVM);
            }

            var result = await _voucherService.UpdateVoucher(editVM);
            if (result)
            {
                TempData["SuccessMessage"] = "Voucher edit successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to edit voucher";
            }
            return RedirectToAction("Detail", new { id = editVM.VoucherId });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {

            var voucher = await _voucherService.GetByIdAsync(id);
            return View(voucher);
        }

        public async Task<IActionResult> DistributeVoucher(int page = 1)
        {

            var vouchers = await _voucherService.GetAllPagedAsync(page, 10);
            vouchers.Users = await _userService.GetAllAsync();
            vouchers.UserGroups = await _userGroupService.GetAllAsync();
            return View(vouchers);
        }

        [HttpPost]
        public async Task<IActionResult> DistributeVoucher(DistributeVoucherVM model)
        {
            if (ModelState.IsValid)
            {
                var voucher = await _voucherService.GetByIdAsync(model.VoucherId);

                if (voucher == null)
                {
                    TempData["ErrorMessage"] = "Voucher not found!";
                    return RedirectToAction("Index");
                }

                if (await _voucherService.DistributeVoucher(voucher, model))
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
            // Danh sách vouchers
            var vouchers = new PagedResult<VoucherDTO>();

            if (userId > 0)
            {
               vouchers = await _voucherService.GetVouchersOfUserPaged(page, pageSize, userId);              
                // Lưu userId vào ViewBag để sử dụng trong Partial View
                ViewBag.UserId = userId;
               
            }
            else
            {
                ViewBag.CampaignId = campaignId;
                vouchers = await _voucherService.GetAllPagedAsync(page, pageSize);
            }
            // Trả về Partial View
            return PartialView("_UserVouchersModal", vouchers);
        }


        public async Task<IActionResult> ListCampaign(int page = 1)
        {
            var voucherCampaigns = await _voucherCampaignService.GetAllPagedAsync(page, 10);
            return View(voucherCampaigns);
        }


        [HttpGet]
        public async Task<IActionResult> CreateCampaign()
        {
            var groups = await _userGroupService.GetAllAsync();

            CampaignVM vm = new CampaignVM();
            vm.Groups = groups;
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> CreateCampaign(CampaignVM createVM)
        {
            if (!ModelState.IsValid)
            {

                return await ReturnModelError(createVM);
            }

            if(createVM.StartDate > createVM.EndDate)
            {
                ModelState.AddModelError("", "The expiry date is not a valid.");
                return await ReturnModelError(createVM);
            }
            
            var result = await _voucherCampaignService.CreateCampaign(createVM);
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

            var campaign = await _voucherCampaignService.GetCampaignWithVoucher(id);
            return View(campaign);
        }


        [HttpGet]
        public async Task<IActionResult> EditCampaign(int id)
        {
            var vm = await _voucherCampaignService.GetCampaign(id);
            vm.Groups = await _userGroupService.GetAllAsync();
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> EditCampaign(CampaignVM editVM)
        {

            if (!ModelState.IsValid)
            {
                return await ReturnModelError(editVM);
            }

            if (editVM.StartDate > editVM.EndDate)
            {
                ModelState.AddModelError("", "The expiry date is not a valid.");
                return await ReturnModelError(editVM);
            }

            var result = await _voucherCampaignService.UpdateCampaign(editVM);
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
            
            var result = await _voucherAssignmentService.CreateAssignments(voucherIds, campaignId);
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
            var voucherUser = await _voucherUserService.GetVoucherUser(voucherId, userId);
            var result = await _voucherUserService.DeleteDistributeVoucher(voucherUser.VoucherUserId);
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

        public async Task<IActionResult> DeteleVoucherOrCampaign(int voucherId, int campaignId)
        {
            var voucherAssign = await _voucherAssignmentService.GetVoucherAssign(voucherId, campaignId);
           
            var result = await _voucherAssignmentService.DeleteAsync(voucherAssign.AssignmentId);
            if (result)
            {
                TempData["SuccessMessage"] = "Delete successfull";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete voucher";
            }
            return RedirectToAction("DetailCampaign", new {id = campaignId});
        }

        private async Task<IActionResult> ReturnModelError(VoucherCreateVM vm)
        {
            vm = await PrepareViewModel(vm);
            return View(vm);
        }

        private async Task<IActionResult> ReturnModelError(VoucherEditVM vm)
        {
            vm = await PrepareViewModel(vm);
            return View(vm);
        }

        private async Task<IActionResult> ReturnModelError(CampaignVM vm)
        {
            vm.Groups = await _userGroupService.GetAllAsync();
            return View(vm);
        }

        private async Task<T> PrepareViewModel<T>(T vm) where T : VoucherBaseVM
        {
            vm.Products = await _productService.GetAllAsync();
            vm.Categories = await _categoryService.GetAllAsync();
            return vm;
        }

    }
}
