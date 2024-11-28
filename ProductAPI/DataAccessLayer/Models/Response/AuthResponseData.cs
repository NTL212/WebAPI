using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.Response
{
	public class AuthResponseData
	{
		public string Token { get; set; }
		public int UserId { get; set; }
		public string UserName { get; set; }

		public string RoleName { get; set; }
		public AuthResponseData(string token, int userId, string userName, string roleName = null)
		{
			Token = token;
			UserId = userId;
			UserName = userName;
			RoleName = roleName;
		}
	}
}
