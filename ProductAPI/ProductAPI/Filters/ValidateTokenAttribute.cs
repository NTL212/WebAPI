using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Services;
using NuGet.Common;

namespace ProductAPI.Filters
{
    public class ValidateTokenAttribute : ActionFilterAttribute
    {
        private readonly IAuthService _authService;

        public ValidateTokenAttribute(IAuthService authService)
        {
            _authService = authService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Kiểm tra token từ header
            if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out var tokenHeader))
            {
                var token = tokenHeader.ToString().Replace("Bearer ", "").Trim();
                if (!IsTokenValid(token))
                {
                    var result = new ObjectResult(new { message = "Token is invalid or has expired." })
                    {
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                    context.Result = result;
                }
            }
            var jwtToken = context.HttpContext.Session.GetString("Token");

            if (!IsTokenValid(jwtToken))
            {
                SetUnauthorizedResult(context, "Token is invalid or has expired.");

               
                return;
            }

            base.OnActionExecuting(context);
        }

        // Phương thức kiểm tra token hợp lệ
        private bool IsTokenValid(string token)
        {
            return !string.IsNullOrEmpty(token) && _authService.ValidateToken(token);
        }

        // Phương thức thiết lập kết quả Unauthorized
        private void SetUnauthorizedResult(ActionExecutingContext context, string message)
        {
            context.HttpContext.Session.Remove("Token");
            context.HttpContext.Session.Remove("UserId");
            context.HttpContext.Session.Remove("UserName");
            context.Result = new ViewResult
            {
                ViewName = "Unauthorized", // Tên View hiển thị (đặt tên view theo ý bạn)
            };
        }
    }

}
