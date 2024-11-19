using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductAPI.Services;
using ProductDataAccess.Models;
using ProductDataAccess.ViewModels;

namespace ProductAPI.Controllers.MVC.Client
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICartService _cartService;
        public HomeController(IHttpClientFactory httpClientFactory, ICartService cartService)
        {
            _httpClientFactory = httpClientFactory;
            _cartService = cartService;
        }
        private readonly string _apiBaseUrl = "https://localhost:7016/api/";

        public async Task<IActionResult> Index()
        {
            // Tạo HttpClient từ IHttpClientFactory
            var client = _httpClientFactory.CreateClient();

            // Gửi GET request tới API
            var response = await client.GetAsync(_apiBaseUrl + "categories");
            var response2 = await client.GetAsync(_apiBaseUrl + "products");
            if (response.IsSuccessStatusCode && response2.IsSuccessStatusCode)
            {
                // Đọc nội dung trả về từ API
                var content = await response.Content.ReadAsStringAsync();
                var content2 = await response2.Content.ReadAsStringAsync();

                var categories = JsonConvert.DeserializeObject<List<Category>>(content);
                var products = JsonConvert.DeserializeObject<List<Product>>(content2);

                HomeVM homeVM = new HomeVM();

                homeVM.categories = categories.Take(6).ToList();
                homeVM.products = products.Take(10).ToList();

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
