using Microsoft.Extensions.Caching.Distributed;
using ProductBusinessLogic.Interfaces;
using ProductDataAccess.Repositories;

namespace ProductAPI.Helper
{
    public class CacheHelper
    {

        public async Task InvalidateUserOrdersCache(int userId, int pageCount, IDistributedCache _distributedCache)
        {
            for (int pageNumber = 1; pageNumber <= pageCount; pageNumber++)
            {
                string cacheKey = $"UserOrders:{userId}:Page:{pageNumber}";
                await _distributedCache.RemoveAsync(cacheKey); // Xóa cache cho từng trang
            }
        }
    }
}
