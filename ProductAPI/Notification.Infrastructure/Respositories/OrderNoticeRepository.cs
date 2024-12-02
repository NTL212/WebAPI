using AutoMapper;
using MongoDB.Driver;
using Notification.Application.Interfaces.Repositories;
using Notification.Domain.Entities;
using Notification.Infrastructure.Data;


namespace Notification.Infrastructure.Respositories
{
    public class OrderNoticeRepository : IOrderNoticeRepository
    {
        private readonly IMongoCollection<MongoOrderNotice> _orderNotice;
        private readonly IMapper _mapper;

        public OrderNoticeRepository(MongoDbContext context, IMapper mapper)
        {
            _orderNotice = context.GetCollection<MongoOrderNotice>("OrderNotices");
            _mapper = mapper;   
        }
        public async Task<OrderNotice> AddAsync(OrderNotice notice)
        {
            var document = _mapper.Map<MongoOrderNotice>(notice);
            await _orderNotice.InsertOneAsync(document);
            return _mapper.Map<OrderNotice>(document);
        }

        public async Task DeleteAsync(string id)
        {
            await _orderNotice.DeleteOneAsync(p=>p.Id == id);
        }

        public async Task<IEnumerable<OrderNotice>> GetAllAsync()
        {
            var documents = await _orderNotice.Find(Builders<MongoOrderNotice>.Filter.Empty).ToListAsync();
            return documents.Select(_mapper.Map<OrderNotice>);
        }

        public async Task<IEnumerable<OrderNotice>> GetAllByOrderAsync(int orderId)
        {
            var documents = await _orderNotice.Find(o=>o.OrderId== orderId).ToListAsync();
            return documents.Select(_mapper.Map<OrderNotice>);
        }

        public async Task<IEnumerable<OrderNotice>> GetAllByUserAsync(int userId)
        {
            var documents = await _orderNotice.Find(o=>o.UserId==userId).ToListAsync();
            return documents.Select(_mapper.Map<OrderNotice>);
        }

        public async Task<OrderNotice> GetByIdAsync(string id)  
        {
            var document = await _orderNotice.FindSync(o => o.Id ==id).FirstOrDefaultAsync();
            return _mapper.Map<OrderNotice>(document);
        }

        public Task SaveChanges()
        {
            return Task.CompletedTask;
        }

        public async Task<OrderNotice> UpdateAsync(OrderNotice notice)
        {
            var mongoNotice = _mapper.Map<MongoOrderNotice>(notice);
            await _orderNotice.ReplaceOneAsync(p => p.Id == notice.Id, mongoNotice);
            return _mapper.Map<OrderNotice>(mongoNotice);
        }
    }
}
