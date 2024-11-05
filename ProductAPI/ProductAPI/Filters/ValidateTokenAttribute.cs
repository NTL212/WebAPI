using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Services;

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
			if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out var tokenHeader))
			{
				var token = tokenHeader.ToString().Replace("Bearer ", "");

				if (!_authService.ValidateToken(token))
				{
					var result = new ObjectResult(new { message = "Token is invalid or has expired." })
					{
						StatusCode = StatusCodes.Status401Unauthorized
					};
					context.Result = result;
				}
			}
			base.OnActionExecuting(context);
		}
	}

}
