﻿using ProductAPI.Repositories.Interfaces;
using ProductDataAccess.Models;

namespace ProductAPI.Repositories
{
	public class OrderVoucherRepository:Repository<OrderVoucher>, IOrderVoucherRepository
	{
		public OrderVoucherRepository(ProductCategoryContext context) : base(context)
		{
		}

		public async Task<bool> ApplyVoucherToOrderAsync(int orderId, int voucherId, decimal discountValue)
		{
			var orderVoucher = new OrderVoucher
			{
				OrderId = orderId,
				VoucherId = voucherId,
				DiscountApplied = discountValue
			};
			_dbSet.Add(orderVoucher);
			return await _context.SaveChangesAsync() > 0;
		}
	}
}
