using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductDataAccess.Models;
using System.Net.Http.Headers;

namespace ProductWebPage.Controllers
{
	public class ShoppingCartController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public ShoppingCartController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}
		private readonly string _apiBaseUrl = "https://localhost:7016/api/";
		public async Task<IActionResult> Index(int id, string token)
		{
			// Tạo HttpClient từ IHttpClientFactory
			var client = _httpClientFactory.CreateClient();

            // Thêm Bearer Token vào header
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Gửi GET request tới API
            var response = await client.GetAsync(_apiBaseUrl+"Cart/" +id+ "/items");

			if (response.IsSuccessStatusCode)
			{
				// Đọc nội dung trả về từ API
				var content = await response.Content.ReadAsStringAsync();

				var items = JsonConvert.DeserializeObject<List<CartItem>>(content);

				// Trả về view với danh sách sản phẩm
				return View(items);
			}
			else
			{
				// Xử lý nếu API không trả về kết quả thành công
				ViewBag.ErrorMessage = "Có lỗi khi lấy dữ liệu từ API.";
				return View("Error");
			}
		}


		public async Task<IActionResult> Checkout(int id, string token)
		{
			// Tạo HttpClient từ IHttpClientFactory
			var client = _httpClientFactory.CreateClient();

			// Thêm Bearer Token vào header
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

			// Gửi GET request tới API
			var response = await client.GetAsync(_apiBaseUrl + "Cart/" + id + "/items");

			if (response.IsSuccessStatusCode)
			{
				// Đọc nội dung trả về từ API
				var content = await response.Content.ReadAsStringAsync();

				var items = JsonConvert.DeserializeObject<List<CartItem>>(content);

				// Trả về view với danh sách sản phẩm
				return View(items);
			}
			else
			{
				// Xử lý nếu API không trả về kết quả thành công
				ViewBag.ErrorMessage = "Có lỗi khi lấy dữ liệu từ API.";
				return View("Error");
			}
		}
	}
}
