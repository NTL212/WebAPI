using AutoMapper;
using ProductDataAccess.Models.Response;
using ProductBusinessLogic.Interfaces;
using ProductDataAccess.Repositories;


namespace ProductBusinessLogic.Services
{
    public class BaseService<T, TDto> : IBaseService<TDto> where T : class where TDto : class
    {
        protected readonly IMapper _mapper;
        private readonly IRepository<T> _repository;

        public BaseService(IMapper mapper, IRepository<T> repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public virtual async Task<bool> AddAsync(TDto dto)
        {
            // Ánh xạ từ DTO sang Entity
            var entity = _mapper.Map<T>(dto);

            // Thực hiện logic thêm mới (mô phỏng)
            await _repository.AddAsync(entity);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> AddRangeAsync(List<TDto> dto)
        {
            // Ánh xạ từ DTO sang Entity
            var entity = _mapper.Map<T>(dto);

            // Thực hiện logic thêm mới (mô phỏng)
            await _repository.AddAsync(entity);
            return await _repository.SaveChangesAsync();
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
           var entity = await _repository.GetByIdAsync(id);
            _repository.Delete(entity);
            return await _repository.SaveChangesAsync();
        }

        public virtual async Task<List<TDto>> GetAllAsync()
        {
            // Lấy tất cả Entity (mô phỏng)
            var entities = await _repository.GetAllAsync();

            // Ánh xạ từ Entity sang DTO
            return _mapper.Map<List<TDto>>(entities);
        }

        public virtual async Task<PagedResult<TDto>> GetAllPagedAsync(int pageNumber, int pageSize)
        {
            // Lấy dữ liệu phân trang (mô phỏng)
            var pagedEntities = await _repository.GetPagedAsync(pageNumber, pageSize);
            var allEntitiesCount = await _repository.CountAsync();

            // Ánh xạ danh sách Entity sang danh sách DTO
            return new PagedResult<TDto>
            {
                Items = _mapper.Map<List<TDto>>(pagedEntities),
                TotalRecords = allEntitiesCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public virtual async Task<TDto> GetByIdAsync(int id)
        {
            // Lấy Entity theo ID
            var entity = await _repository.GetByIdAsync(id);

            // Ánh xạ Entity sang DTO
            return _mapper.Map<TDto>(entity);
        }

        public virtual async Task<bool> UpdateAsync(TDto dto)
        {
            // Ánh xạ từ DTO sang Entity
            var entity = _mapper.Map<T>(dto);

            _repository.Update(entity);
            return await _repository.SaveChangesAsync();
            
        }

        public async Task<bool> UpdateRangeAsync(List<TDto> dto)
        {
            // Ánh xạ từ DTO sang Entity
            var entity = _mapper.Map<List<T>>(dto);

            _repository.UpdateRange(entity);
            return await _repository.SaveChangesAsync();

        }
    }
}
