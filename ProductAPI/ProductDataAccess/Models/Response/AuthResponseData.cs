using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductDataAccess.Models.Response
{
	public class AuthResponseData
	{
		public string Token { get; set; }
		public int UserId { get; set; }
		public string UserName { get; set; }

		public AuthResponseData(string token, int userId, string userName)
		{
			Token = token;
			UserId = userId;
			UserName = userName;
		}
	}
}
