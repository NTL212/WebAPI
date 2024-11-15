using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

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
                    context.Result = new ForbidResult(); // Nếu không có role phù hợp, trả về Forbidden
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
