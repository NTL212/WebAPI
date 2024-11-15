using ProductDataAccess.DTOs;
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

        public List<UserDTO> Users { get; set; } = new List<UserDTO>();
		public int TotalRecords { get; set; }
		public int PageNumber { get; set; }
		public int PageSize { get; set; }

        // Tính toán số trang tổng cộng
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);

        // Kiểm tra nếu có trang trước
        public bool HasPreviousPage => PageNumber > 1;

        // Kiểm tra nếu có trang sau
        public bool HasNextPage => PageNumber < TotalPages;
    }
}

