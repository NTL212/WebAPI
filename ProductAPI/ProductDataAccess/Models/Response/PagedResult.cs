using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductDataAccess.Models.Response
{
	public class PagedResult<T> where T : class
	{
		public List<T> Items { get; set; } = new List<T>();
		public int TotalRecords { get; set; }
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
	}
}

