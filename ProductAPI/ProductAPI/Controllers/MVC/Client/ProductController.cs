using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductDataAccess.Repositories;
using ProductDataAccess.Models;
using ProductDataAccess.Models.Response;
using ProductBusinessLogic.Interfaces;


namespace ProductAPI.Controllers.MVC.Client
{
    public class ProductController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IProductService _productService;
        public ProductController(IHttpClientFactory httpClientFactory, IProductService productService)
        {
            _httpClientFactory = httpClientFactory;
            _productService = productService;
        }
        private readonly string _apiBaseUrl = "https://localhost:7016/api/";

        public async Task<IActionResult> Index()
        {
            // Tạo HttpClient từ IHttpClientFactory
            var client = _httpClientFactory.CreateClient();

            // Gửi GET request tới API
            var response = await client.GetAsync(_apiBaseUrl + "categories");

            if (response.IsSuccessStatusCode)
            {
                // Đọc nội dung trả về từ API
                var content = await response.Content.ReadAsStringAsync();

                var products = JsonConvert.DeserializeObject<List<Product>>(content);

                // Trả về view với danh sách sản phẩm
                return View(products);
            }
            else
            {
                // Xử lý nếu API không trả về kết quả thành công
                ViewBag.ErrorMessage = "Có lỗi khi lấy dữ liệu từ API.";
                return View();
            }
        }

        public async Task<IActionResult> Detail(int id)
        {
            // Tạo HttpClient từ IHttpClientFactory
            var client = _httpClientFactory.CreateClient();

            // Gửi GET request tới API
            var response = await client.GetAsync(_apiBaseUrl + "products/" + id);

            if (response.IsSuccessStatusCode)
            {
                // Đọc nội dung trả về từ API
                var content = await response.Content.ReadAsStringAsync();

                var product = JsonConvert.DeserializeObject<Product>(content);

                // Trả về view với danh sách sản phẩm
                return View(product);
            }
            else
            {
                // Xử lý nếu API không trả về kết quả thành công
                ViewBag.ErrorMessage = "Có lỗi khi lấy dữ liệu từ API.";
                return View();
            }
        }

        public async Task<IActionResult> Shop(int page = 1, string searchKey = "")
        {
            // Tạo HttpClient từ IHttpClientFactory
            var client = _httpClientFactory.CreateClient();

            // Gửi GET request tới API
            var response = await client.GetAsync(_apiBaseUrl + "products/paged/" + page + "?searchKey=" + searchKey);

            if (response.IsSuccessStatusCode)
            {
                // Đọc nội dung trả về từ API
                var content = await response.Content.ReadAsStringAsync();

                var products = JsonConvert.DeserializeObject<PagedResult<Product>>(content);

                // Trả về view với danh sách sản phẩm
                return View(products);
            }
            else
            {
                // Xử lý nếu API không trả về kết quả thành công
                ViewBag.ErrorMessage = "Có lỗi khi lấy dữ liệu từ API.";
                return View();
            }
        }

        public async Task<IActionResult> ShopByCategory(int id, int page = 1)
        {
            // Tạo HttpClient từ IHttpClientFactory
            var client = _httpClientFactory.CreateClient();

            // Gửi GET request tới API
            var response = await client.GetAsync(_apiBaseUrl + "products/" + id + "/category/paged/" + page);
            if (response.IsSuccessStatusCode)
            {
                // Đọc nội dung trả về từ API
                var content = await response.Content.ReadAsStringAsync();

                var products = JsonConvert.DeserializeObject<PagedResult<Product>>(content);

                // Trả về view với danh sách sản phẩm
                return View(products);
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
