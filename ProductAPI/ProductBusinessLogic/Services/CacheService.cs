using Microsoft.Extensions.Caching.Distributed;
using ProductBusinessLogic.Interfaces;


namespace ProductBusinessLogic.Services
{
    public class CacheService: ICacheService
    {
        private readonly IDistributedCache _distributedCache;

        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }


        public async Task InvalidateUserOrdersCacheAsync(int userId, int pageCount)
        {
            for (int pageNumber = 1; pageNumber <= pageCount; pageNumber++)
            {
                string cacheKey = $"UserOrders:{userId}:Page:{pageNumber}";
                await _distributedCache.RemoveAsync(cacheKey); // Xóa cache cho từng trang
            }
        }
    }
}
