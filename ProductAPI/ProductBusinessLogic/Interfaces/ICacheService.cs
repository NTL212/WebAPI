namespace ProductBusinessLogic.Interfaces
{
    public interface ICacheService
    {
        Task InvalidateUserOrdersCacheAsync(int userId, int pageCount);

    }

}
