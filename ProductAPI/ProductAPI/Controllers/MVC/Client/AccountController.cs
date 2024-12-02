using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductAPI.Filters;
using ProductDataAccess.Repositories.Interfaces;
using ProductAPI.Services;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models.Request;
using ProductDataAccess.Models.Response;
using System.Text;
using ProductBusinessLogic.Interfaces;

namespace ProductAPI.Controllers.MVC.Client
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IAuthService _authService;
        public AccountController(IHttpClientFactory httpClientFactory, IUserService userService, IAuthService authService, IEmailService emailService)
        {
            _httpClientFactory = httpClientFactory;
            _userService = userService;
            _authService = authService;
            _emailService = emailService;
        }
        private readonly string _apiBaseUrl = "https://localhost:7016/api/AuthContronller/";
        private readonly string _apiBaseEmailUrl = "https://localhost:7016/api/Email/ConfirmEmail";
        public async Task<IActionResult> Login(LoginDTO loginDto, string returnUrl = null)
        {
            // Kiểm tra nếu model không hợp lệ
            if (!ModelState.IsValid)
            {
                // Lưu thông báo lỗi vào TempData
                TempData["LoginErrorMessage"] = "Invalid input.";
                return RedirectToAction("Index", "Home");
            }

            var user = await _userService.GetUserByEmail(loginDto.Email);
            if (user == null)
            {
                TempData["LoginErrorMessage"] = "Account not exist.";
                return RedirectToAction("Index", "Home");
            }
            else if(user.IsActive == false)
            {
                TempData["LoginErrorMessage"] = "Account not active";
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

                if (authResponse.Data.RoleName == "Admin")
                {
                    return RedirectToAction("Index", "Dashboard");
                }

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
                TempData["LoginErrorMessage"] = "Login failed. Incorrect email or password.";
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

        [JwtAuthorize("Customer")]
        [ServiceFilter(typeof(ValidateTokenAttribute))]
        public async Task<IActionResult> UserProfile(int userId)
        {
            var user = await _userService.GetByIdAsync(userId);
            return View(user);

        }

        [JwtAuthorize("Customer")]
        [ServiceFilter(typeof(ValidateTokenAttribute))]
        public IActionResult ChangePass(string mess = null)
        {
            ViewBag.Message = mess;
            return View();
        }
        [JwtAuthorize("Customer")]
        [ServiceFilter(typeof(ValidateTokenAttribute))]
        [HttpPost]
        public async Task<IActionResult> ChangePass(ChangePasswordDTO changeDto)
        {
            var user = await _userService.GetByIdAsync((int)HttpContext.Session.GetInt32("UserId"));
            changeDto.Email = user.Email;
            var token = _authService.ChangePassword(changeDto);
            string message = "Failed";
            if (token)
            {
                message = "Success";
            }
            return RedirectToAction("ChangePass", new { mess = message });
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var result = await _emailService.SendForgotPasswordEmail(email);
            if (result)
            {
                TempData["SuccessMessage"] = "New Password sent to your email";
               
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred. Please try again.";
            }
            return View();
        }
    }
}
