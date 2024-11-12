using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models.Request;
using ProductDataAccess.Models.Response;
using System.Text;

namespace ProductAPI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        private readonly string _apiBaseUrl = "https://localhost:7016/api/AuthContronller/";
        private readonly string _apiBaseEmailUrl = "https://localhost:7016/api/Email/ConfirmEmail";
        public async Task<IActionResult> Login(LoginDTO loginDto, string returnUrl = null)
        {
            // Kiểm tra nếu model không hợp lệ
            if (!ModelState.IsValid)
            {
                // Lưu thông báo lỗi vào TempData
                TempData["ErrorMessage"] = "Invalid input.";
                return RedirectToAction("Index", "Home");
            }

            // Tạo HttpClient từ IHttpClientFactory
            var client = _httpClientFactory.CreateClient();

            // Tạo nội dung POST yêu cầu
            var content = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json");

            // Gửi POST request tới API login
            var response = await client.PostAsync(_apiBaseUrl + "login", content);

            if (response.IsSuccessStatusCode)
            {
                // Đọc nội dung trả về từ API
                var responseData = await response.Content.ReadAsStringAsync();

                // Deserialize nội dung trả về thành đối tượng AuthResponseData
                var authResponse = JsonConvert.DeserializeObject<ApiResponse<AuthResponseData>>(responseData);

                // Lưu trữ thông tin người dùng (token, userId, etc.) vào session hoặc cookie
                HttpContext.Session.SetString("Token", authResponse.Data.Token);
                HttpContext.Session.SetInt32("UserId", authResponse.Data.UserId);
                HttpContext.Session.SetString("UserName", authResponse.Data.UserName);

                // Nếu có returnUrl thì điều hướng người dùng trở lại trang đó, nếu không thì redirect về trang chủ
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction("Index", "Home");
                }
                return Redirect(returnUrl);

            }
            else
            {
                // Trả về lỗi nếu đăng nhập thất bại
                TempData["ErrorMessage"] = "Login failed. Incorrect email or password.";
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Token");
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Register(RegisterDTO registerDTO, string returnUrl = null)
        {
            // Kiểm tra nếu model không hợp lệ
            if (!ModelState.IsValid)
            {
                // Lưu thông báo lỗi vào TempData
                TempData["ErrorMessage"] = "Invalid input.";
                return RedirectToAction("Index", "Home");
            }

            // Tạo HttpClient từ IHttpClientFactory
            var client = _httpClientFactory.CreateClient();

            // Tạo nội dung POST yêu cầu
            var content = new StringContent(JsonConvert.SerializeObject(registerDTO), Encoding.UTF8, "application/json");

            // Gửi POST request tới API login
            var response = await client.PostAsync(_apiBaseUrl + "register", content);

            if (response.IsSuccessStatusCode)
            {
                // Đọc nội dung trả về từ API
                var responseData = await response.Content.ReadAsStringAsync();

                // Deserialize nội dung trả về thành đối tượng AuthResponseData
                var authResponse = JsonConvert.DeserializeObject<ApiResponse<AuthResponseData>>(responseData);

                // Lưu trữ thông tin người dùng (token, userId, etc.) vào session hoặc cookie
                HttpContext.Session.SetString("Token", authResponse.Data.Token);

                return Redirect("WaitingConfirm");

            }
            else
            {
                // Trả về lỗi nếu đăng nhập thất bại
                TempData["ErrorMessage"] = "Login failed. Incorrect email or password.";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> ConfirmEmail(string email)
        {
            // Tạo HttpClient từ IHttpClientFactory
            var client = _httpClientFactory.CreateClient();
            // Tạo nội dung POST yêu cầu
            var request = new ConfirmEmailRequest();
            request.Email = email;
            request.Token = HttpContext.Session.GetString("Token");

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_apiBaseEmailUrl, content);
            
            if (response.IsSuccessStatusCode)
            {
                //// Đọc nội dung trả về từ API
                //var responseData = await response.Content.ReadAsStringAsync();

                //// Deserialize nội dung trả về thành đối tượng AuthResponseData
                //var authResponse = JsonConvert.DeserializeObject<string>(responseData);
                return RedirectToAction("Index", "Home");

            }
            else
            {
                // Trả về lỗi nếu đăng nhập thất bại
                TempData["ErrorMessage"] = "Login failed. Incorrect email or password.";
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult WaitingConfirm()
        {
            return View();
        }
    }
}
