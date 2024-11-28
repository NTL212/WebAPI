
using AutoMapper;
using ProductBusinessLogic.Interfaces;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models;
using ProductDataAccess.Repositories;
using ProductDataAccess.Repositories.Interfaces;

namespace ProductBusinessLogic.Services
{
    public class UserGroupService : BaseService<UserGroup, GroupDTO>, IUserGroupService
    {
        private readonly IUserGroupRepository _userGroupRepository;
        public UserGroupService(IMapper mapper, IUserGroupRepository repository) : base(mapper, repository)
        {
            _userGroupRepository = repository;
        }

    }
}
