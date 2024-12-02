using Microsoft.AspNetCore.Mvc;
using ProductDataAccess.DTOs;
using ProductDataAccess.ViewModels;
using ProductAPI.Filters;
using ProductBusinessLogic.Interfaces;

namespace ProductAPI.Controllers.MVC.Admin
{
    [JwtAuthorize("Admin")]
    [ServiceFilter(typeof(ValidateTokenAttribute))]
    public class AdminUserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUserGroupService _userGroupService;

        public AdminUserController(IUserService userService, IUserGroupService userGroupService)
        {
            _userService = userService;
            _userGroupService = userGroupService;
        }

        public async Task<IActionResult> Index(int page=1, string searchText="")
        {
            var users = await _userService.GetAllUserPagedWithSearch(page, 10, searchText);

            var userGroups = await _userGroupService.GetAllAsync();
            var userVM = new AdminUserListVM();

            userVM.userDtos = users;
            userVM.groupDtos = userGroups;

            return View(userVM);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            return View(user);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewData["groups"] = await _userGroupService.GetAllAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserDTO userDTO, string password)
        {
            if (!ModelState.IsValid)
            {
                return await ReturnModelError(userDTO);
            }

            var userExist = _userService.GetUserByEmail(userDTO.Email);
            if (userExist != null)
            {
                ModelState.AddModelError("", "Email had exist");
                return await ReturnModelError(userDTO);
            }

            var result = await _userService.CreateUser(userDTO, password);
            if (result)
            {
                TempData["SuccessMessage"] = "Add user successfully";           
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to add user";
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 1)
            {
                TempData["ErrorMessage"] = "Admin user not allow edit";
                return RedirectToAction("Index");
            }
            var user = await _userService.GetByIdAsync(id);
            ViewData["groups"] = await _userGroupService.GetAllAsync();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                return await ReturnModelError(userDto);
            }

            var user = await _userService.GetByIdAsync(userDto.UserId);

            if(user == null)
            {
                return await ReturnModelError(userDto);
            }

            var result = await _userService.UpdateAsync(userDto);
            if (result)
            {
                TempData["SuccessMessage"] = "Update user successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update user";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AssignGroup(int[] selectedUserIds, int userGroupId)
        {
            if (selectedUserIds != null && selectedUserIds.Length > 0)
            {
                var result = await _userService.AssignGroupToUsersAsync(selectedUserIds, userGroupId);
                if (result)
                {
                    TempData["SuccessMessage"] = "Group successfully assigned to selected users.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to assign group.";
                }
            }

            return RedirectToAction("Index");
        }

        private async Task<IActionResult> ReturnModelError(UserDTO userDTO)
        {
            ViewData["groups"] = await _userGroupService.GetAllAsync();
            return View(userDTO);
        }
    }
}
