using ProductDataAccess.DTOs;
using ProductDataAccess.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductBusinessLogic.Interfaces
{
    public interface IUserService:IBaseService<UserDTO>
    {
        Task<PagedResult<UserDTO>> GetAllUserPagedWithSearch(int pageNumber, int pageSize, string searchKey);
        Task<UserDTO> GetUserByEmail(string email);

        Task<bool> CreateUser(UserDTO userDTO, string password);

        Task<bool> AssignGroupToUsersAsync(int[] selectedUserIds, int userGroupId);

        Task<int> GetCountOrderOfUser(int userId);

        Task<string> ForgotPassword(string email);
    }
}
