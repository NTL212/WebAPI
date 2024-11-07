using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductAPI.Models;
using ProductAPI.ViewModels;

namespace ProductAPI.Controllers
{
	public class HomeController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public HomeController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}
		private readonly string _apiBaseUrl = "https://localhost:7016/api/";

		public async Task<IActionResult> Index()
		{
			// Tạo HttpClient từ IHttpClientFactory
			var client = _httpClientFactory.CreateClient();

			// Gửi GET request tới API
			var response = await client.GetAsync(_apiBaseUrl+ "categories");
			var response2 = await client.GetAsync(_apiBaseUrl+ "products");
			if (response.IsSuccessStatusCode && response2.IsSuccessStatusCode)
			{
				// Đọc nội dung trả về từ API
				var content = await response.Content.ReadAsStringAsync();
				var content2 = await response2.Content.ReadAsStringAsync();

				HomeVM homeVM = new HomeVM();

				homeVM.categories = JsonConvert.DeserializeObject<List<Category>>(content);
                homeVM.products = JsonConvert.DeserializeObject<List<Product>>(content2);



				// Trả về view với danh sách sản phẩm
				return View(homeVM);
			}
			else
			{
				// Xử lý nếu API không trả về kết quả thành công
				ViewBag.ErrorMessage = "Có lỗi khi lấy dữ liệu từ API.";
				return View();
			}
		}
	}
}
