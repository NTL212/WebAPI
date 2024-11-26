using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
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
        private readonly IDistributedCache _cache;
        private readonly string _apiBaseUrl = "https://localhost:7016/api/";

        public HomeController(IHttpClientFactory httpClientFactory, ICartService cartService, IDistributedCache cache)
        {
            _httpClientFactory = httpClientFactory;
            _cartService = cartService;
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            // Cache keys
            const string categoriesCacheKey = "home:categories";
            const string productsCacheKey = "home:products";

            // Attempt to get categories from cache
            var cachedCategories = await _cache.GetStringAsync(categoriesCacheKey);
            var cachedProducts = await _cache.GetStringAsync(productsCacheKey);

            List<Category> categories;
            List<Product> products;

            if (!string.IsNullOrEmpty(cachedCategories) && !string.IsNullOrEmpty(cachedProducts))
            {
                // Deserialize cached data
                categories = JsonConvert.DeserializeObject<List<Category>>(cachedCategories);
                products = JsonConvert.DeserializeObject<List<Product>>(cachedProducts);
            }
            else
            {
                // Create HttpClient from IHttpClientFactory
                var client = _httpClientFactory.CreateClient();

                // Send GET requests to APIs
                var categoriesResponse = await client.GetAsync(_apiBaseUrl + "categories");
                var productsResponse = await client.GetAsync(_apiBaseUrl + "products");

                if (categoriesResponse.IsSuccessStatusCode && productsResponse.IsSuccessStatusCode)
                {
                    var categoriesContent = await categoriesResponse.Content.ReadAsStringAsync();
                    var productsContent = await productsResponse.Content.ReadAsStringAsync();

                    // Deserialize API data
                    categories = JsonConvert.DeserializeObject<List<Category>>(categoriesContent);
                    products = JsonConvert.DeserializeObject<List<Product>>(productsContent);

                    // Store data in cache with expiration time
                    var cacheOptions = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) // Cache expires after 1 hour
                    };

                    await _cache.SetStringAsync(categoriesCacheKey, JsonConvert.SerializeObject(categories), cacheOptions);
                    await _cache.SetStringAsync(productsCacheKey, JsonConvert.SerializeObject(products), cacheOptions);
                }
                else
                {
                    // Handle API errors
                    ViewBag.ErrorMessage = "Có lỗi khi lấy dữ liệu từ API.";
                    return View();
                }
            }

            // Filter and prepare data for the view
            HomeVM homeVM = new HomeVM
            {
                categories = categories.Where(c =>c.IsDeleted==false).Take(6).ToList(),
                products = products.Take(10).ToList()
            };

            return View(homeVM);
        }
    }
}
