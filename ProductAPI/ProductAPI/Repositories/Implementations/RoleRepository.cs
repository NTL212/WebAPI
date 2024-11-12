﻿using Microsoft.EntityFrameworkCore;
using ProductAPI.Repositories.Interfaces;
using ProductDataAccess.Models;

namespace ProductAPI.Repositories
{
	public class RoleRepository:Repository<Role>, IRoleRepository
	{
		public RoleRepository(ProductCategoryContext context) : base(context)
		{

		}

		public async Task<Role> GetRoleByName(string name)
		{
			return await _dbSet.FirstOrDefaultAsync(r=>r.Name==name);
		}

	}
}