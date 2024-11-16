
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductAPI.Filters;
using ProductAPI.Repositories;
using ProductAPI.Repositories.Interfaces;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models;
using ProductDataAccess.Models.Response;
using ProductDataAccess.ViewModels;
using System.Diagnostics;

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
        private readonly IMapper _mapper;
        public AdminVoucherController(IVoucherRepository voucherRepository, IProductRepository productRepository, ICategoryRepository categoryRepository, IUserRepoisitory userRepository, IUserGroupRepository userGroupRepository, IVoucherUserRepository voucherUserRepository, IMapper mapper)
        {
            _voucherRepository = voucherRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _userGroupRepository = userGroupRepository;
            _voucherUserRepository = voucherUserRepository;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(int page = 1)
        {

            var vouchers = await _voucherRepository.GetPagedAsync(page, 10);
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
            var users = await _userRepository.GetAllAsync();
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
        public async Task<IActionResult> GetUserVouchers(int userId)
        {
            // Lấy tất cả voucherUsers với thông tin liên quan đến Voucher
            var voucherUsers = await _voucherUserRepository.GetAllWithPredicateIncludeAsync(v => v.UserId == userId, v => v.Voucher);

            // Lọc và ánh xạ từ voucherUser sang VoucherDTO, đồng thời gán RedeemQuantity từ Quantity của voucherUser
            var vouchers = voucherUsers.Select(voucherUser =>
            {
                var voucherDto = _mapper.Map<VoucherDTO>(voucherUser.Voucher); // Ánh xạ Voucher thành VoucherDTO
                voucherDto.ReedemQuantity = voucherUser.Quantity - voucherUser.TimesUsed; 
                return voucherDto;
            }).ToList();

            return PartialView("_UserVouchersModal", vouchers); // Trả về partial view với danh sách vouchers
        }

    }
}
