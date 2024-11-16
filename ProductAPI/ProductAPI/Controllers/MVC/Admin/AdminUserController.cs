using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Repositories.Interfaces;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models.Response;
using ProductDataAccess.Models;
using ProductDataAccess.ViewModels;
using ProductAPI.Filters;

namespace ProductAPI.Controllers.MVC.Admin
{
    [JwtAuthorize("Admin")]
    [ServiceFilter(typeof(ValidateTokenAttribute))]
    public class AdminUserController : Controller
    {
        private readonly IUserRepoisitory _userRepoisitory;
        private readonly IUserGroupRepository _userUserGroupRepository;
        private readonly IMapper _mapper;

        public AdminUserController(IUserRepoisitory userRepoisitory, IMapper mapper, IUserGroupRepository userGroupRepository)
        {
            _userRepoisitory = userRepoisitory;
            _mapper = mapper;
            _userUserGroupRepository = userGroupRepository;
        }

        public async Task<IActionResult> Index(int page=1, string searchText=null)
        {
            var users = new PagedResult<User>();
            if (searchText != null)
            {
                users = await _userRepoisitory.GetPagedWithIncludeSearchAsync(page, 10, u => u.Username.ToLower().Contains(searchText.ToLower()), u => u.Group);
            }
            else
            {
                users =  await _userRepoisitory.GetPagedWithIncludeAsync(page, 10, u => u.Group);
            }
            var userDtos = _mapper.Map<PagedResult<UserDTO>>(users);
            var userGroups = await _userUserGroupRepository.GetAllAsync();
            var userGroupDtos = _mapper.Map<List<GroupDTO>>(userGroups);
            var userVM = new AdminUserListVM();

            userVM.userDtos = userDtos;
            userVM.groupDtos = userGroupDtos;

            return View(userVM);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var user = await _userRepoisitory.GetByIdWithIncludeAsync(u=>u.UserId==id, u=>u.Group);
            var userDto = _mapper.Map<UserDTO>(user);
            return View(userDto);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var group = await _userUserGroupRepository.GetAllAsync();
            ViewData["groups"] = _mapper.Map<List<GroupDTO>>(group);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserDTO userDTO, string password)
        {
            var user = _mapper.Map<User>(userDTO);
            var result = await _userRepoisitory.CreateUser(user, password);
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
            var user = await _userRepoisitory.GetByIdAsync(id);
            var group = await _userUserGroupRepository.GetAllAsync();

            var userDto = _mapper.Map<UserDTO>(user);
            ViewData["groups"] = _mapper.Map<List<GroupDTO>>(group);

            return View(userDto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserDTO userDto)
        {
            var user = await _userRepoisitory.GetByIdAsync(userDto.UserId);
            user.Username = userDto.Username;
            user.Email = userDto.Email;
            user.GroupId = userDto.GroupId;
            user.IsActive = userDto.IsActive;

            var result = await _userRepoisitory.UpdateAsync(user);
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
                var result = await _userRepoisitory.AssignGroupToUsersAsync(selectedUserIds, userGroupId);
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
    }
}
