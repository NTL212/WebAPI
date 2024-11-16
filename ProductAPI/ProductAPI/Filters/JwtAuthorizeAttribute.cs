using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using ProductAPI.Services;

namespace ProductAPI.Filters
{
    public class JwtAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string _role;
        public JwtAuthorizeAttribute(string role)
        {
            _role = role; // Role mà bạn muốn kiểm tra
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Lấy JWT Token từ Session
            var token = context.HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new UnauthorizedResult(); // Nếu không có token, trả về Unauthorized
                return;
            }

            // Kiểm tra token hợp lệ và giải mã nó
            var jwtHandler = new JwtSecurityTokenHandler();
            try
            {
                var jwtToken = jwtHandler.ReadToken(token) as JwtSecurityToken;

                // Kiểm tra role trong token
                var roles = jwtToken?.Claims.Where(c => c.Type.Contains("role")).Select(c => c.Value).ToList();

                if (roles == null || !roles.Contains(_role))
                {
                    context.Result = new ViewResult
                    {
                        ViewName = "Unauthorized", // Tên View hiển thị (đặt tên view theo ý bạn)
                    };
                }
            }
            catch
            {
                context.Result = new UnauthorizedResult(); // Nếu không giải mã được token, trả về Unauthorized
            }

            base.OnActionExecuting(context);
        }
    }
}
