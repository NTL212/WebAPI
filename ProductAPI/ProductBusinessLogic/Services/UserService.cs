using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductBusinessLogic.Interfaces;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models;
using ProductDataAccess.Models.Response;
using ProductDataAccess.Repositories.Interfaces;
using System.Text.RegularExpressions;



namespace ProductBusinessLogic.Services
{
    public class UserService : BaseService<User, UserDTO>, IUserService
    {
        private readonly IUserRepoisitory _userRepoisitory;
        private readonly PasswordHasher<User> _passwordHasher;
        public UserService(IMapper mapper,IUserRepoisitory userRepoisitory, PasswordHasher<User> passwordHasher) : base(mapper, userRepoisitory)
        {
            _userRepoisitory = userRepoisitory;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> AssignGroupToUsersAsync(int[] selectedUserIds, int userGroupId)
        {
            var users = await _userRepoisitory.GetAllWithPredicateIncludeAsync(u => selectedUserIds.Contains(u.UserId));
            foreach (var user in users)
            {
                user.GroupId = userGroupId;
            }

            _userRepoisitory.UpdateRange(users.ToList());
            return await _userRepoisitory.SaveChangesAsync();
        }

        public async Task<bool> CreateUser(UserDTO userDTO, string password)
        {
            try
            {
                User user =_mapper.Map<User>(userDTO);
                user.PasswordHash = _passwordHasher.HashPassword(user, password); // Mã hóa mật khẩu
                user.CreatedAt = DateTime.Now;
                user.RoleId = 2;
                await _userRepoisitory.AddAsync(user);
                return await _userRepoisitory.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<PagedResult<UserDTO>> GetAllUserPagedWithSearch(int pageNumber, int pageSize, string searchKey)
        {
            var users = await _userRepoisitory.GetPagedWithIncludeSearchAsync(pageNumber, 10, u => u.Username.ToLower().Contains(searchKey.ToLower()) && u.UserId!=1, u => u.Group);
            var totalRecords = await _userRepoisitory.CountAsync(u => u.Username.ToLower().Contains(searchKey.ToLower()) && u.UserId != 1);
            return new PagedResult<UserDTO> 
            {  
                Items = _mapper.Map<List<UserDTO>>(users),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };

        }

        public override async Task<UserDTO> GetByIdAsync(int id)
        {
            var user = await _userRepoisitory.GetByIdWithIncludeAsync(u => u.UserId == id, u => u.Group);
            return _mapper.Map<UserDTO>(user);
        }

        public async override Task<List<UserDTO>> GetAllAsync()
        {
            var user = await _userRepoisitory.GetAllWithPredicateIncludeAsync(u => u.UserId != 1);
            return _mapper.Map<List<UserDTO>>(user);
        }

        public async Task<UserDTO> GetUserByEmail(string email)
        {
           var user = await _userRepoisitory.GetByIdWithIncludeAsync(u => u.Email == email, u => u.Group);
            return _mapper.Map<UserDTO>(user);
        }
    }
}
