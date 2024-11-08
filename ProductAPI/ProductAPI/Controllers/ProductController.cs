﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductAPI.Models;

namespace ProductAPI.Controllers
{
	public class ProductController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public ProductController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
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
            var response = await client.GetAsync(_apiBaseUrl + "products/"+id);

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

        public async Task<IActionResult> Shop()
        {
            // Tạo HttpClient từ IHttpClientFactory
            var client = _httpClientFactory.CreateClient();

            // Gửi GET request tới API
            var response = await client.GetAsync(_apiBaseUrl + "products");

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

        public async Task<IActionResult> ShopByCategory(int id)
        {
            // Tạo HttpClient từ IHttpClientFactory
            var client = _httpClientFactory.CreateClient();

            // Gửi GET request tới API
            var response = await client.GetAsync(_apiBaseUrl + "products/"+id+"/category");

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
    }
}
