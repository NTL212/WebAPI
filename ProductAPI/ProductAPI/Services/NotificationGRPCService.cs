using Grpc.Core;
using Grpc.Net.Client;

namespace ProductAPI.Services
{
    public class NotificationGRPCService : INotificationGRPCService
    {
        private readonly NotificationService.NotificationServiceClient _client;

        public NotificationGRPCService()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7257");
            _client = new NotificationService.NotificationServiceClient(channel);
        }

        public async Task<List<Notice>> GetNoticesAsync()
        {
            try
            {
                var response = await _client.GetNotifyListAsync(new Empty());
                return response.Notices.ToList();
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"gRPC Error: {ex.StatusCode} - {ex.Message}");
                throw;
            }
        }
        public async Task<List<Notice>> GetNoticesByUserAsync(int id)
        {
            try
            {
                var response = await _client.GetNotifyByUserListAsync(new IdRequest { Id=id});
                return response.Notices.ToList();
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"gRPC Error: {ex.StatusCode} - {ex.Message}");
                throw;
            }
        }

        public async Task<List<Notice>> GetNoticesByOrderAsync(int id)
        {
            try
            {
                var response = await _client.GetNotifyByOrderListAsync(new IdRequest { Id=id});
                return response.Notices.ToList();
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"gRPC Error: {ex.StatusCode} - {ex.Message}");
                throw;
            }
        }
        public async Task<Notice> GetNoticeByIdAsync(string id)
        {
            try
            {
                var response = await _client.GetNoticeByIdAsync(new NoticeIdRequest { Id = id });
                return response;
            }
            catch (Grpc.Core.RpcException ex)
            {
                Console.WriteLine($"gRPC Error: {ex.StatusCode} - {ex.Message}");
                throw;
            }
        }

        public async Task<string> AddNoticeAsync(Notice notice)
        {
            try
            {
                var response = await _client.AddNoticeAsync(notice);
                return response.Message;
            }
            catch (Grpc.Core.RpcException ex)
            {
                Console.WriteLine($"gRPC Error: {ex.StatusCode} - {ex.Message}");
                throw;
            }
        }

        public async Task<string> UpdateNoticeAsync(Notice notice)
        {
            try
            {
                var response = await _client.UpdateNoticeAsync(notice);
                return response.Message;
            }
            catch (Grpc.Core.RpcException ex)
            {
                Console.WriteLine($"gRPC Error: {ex.StatusCode} - {ex.Message}");
                throw;
            }
        }

        public async Task<string> DeleteNoticeAsync(string id)
        {
            try
            {
                var response = await _client.DeleteNoticeAsync(new NoticeIdRequest { Id = id });
                return response.Message;
            }
            catch (Grpc.Core.RpcException ex)
            {
                Console.WriteLine($"gRPC Error: {ex.StatusCode} - {ex.Message}");
                throw;
            }
        }

        public async Task<string> UpdateNoticeIsSeen(string id)
        {
            try
            {
                var response = await _client.UpdateNoticeIsSeenAsync(new NoticeIdRequest { Id = id });
                return response.Message;
            }
            catch (Grpc.Core.RpcException ex)
            {
                Console.WriteLine($"gRPC Error: {ex.StatusCode} - {ex.Message}");
                throw;
            }
        }

        public async Task<string> UpdateNoticeIsSeenAsync(string id)
        {
            try
            {
                var response = await _client.UpdateNoticeIsSeenAsync(new NoticeIdRequest { Id = id });
                return response.Message;
            }
            catch (Grpc.Core.RpcException ex)
            {
                Console.WriteLine($"gRPC Error: {ex.StatusCode} - {ex.Message}");
                throw;
            }
        }
    }
}
