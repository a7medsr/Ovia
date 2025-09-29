using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Ovia.Helpers;
using Ovia.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ovia.Services
{
    public class JWTService
    {

		private readonly AppSettings _appSettings;

		public JWTService(IOptions<AppSettings> appSettings)
		{
			_appSettings = appSettings.Value;

		}
		public JWTToken GenerateToken(string UserId, List<string> Roles)
		{
			JWTToken user = new JWTToken();
			//user.UserName = UserName;
			//user.Role = role;

			// authentication successful so generate jwt token
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes("THIS IS Medaf TO SIGN AND VERIFY JWT TOKENS,For Community management system Project");
			List<Claim> ClaimRoles = new List<Claim>();
			//Roles.Add(new Claim(ClaimTypes.Role, "user"));
			//Roles.Add(new Claim(ClaimTypes.Role, "admin"));
			foreach (string role in Roles)
            {
				ClaimRoles.Add(new Claim(ClaimTypes.Role, role));
            }

			var ClaimsList = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.NameIdentifier, UserId),

				});
			ClaimsList.AddClaims(ClaimRoles);
			 
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = ClaimsList,
				Expires = DateTime.UtcNow.AddMinutes(30),
				//Expires = DateTime.UtcNow.AddSeconds(1),
				//IssuedAt = DateTime.UtcNow.AddHours(2),
				
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			user.Token = tokenHandler.WriteToken(token);



			return user;
		}


		public JWTToken logout(string UserId, List<string> Roles)
		{
			JWTToken user = new JWTToken();
			//user.UserName = UserName;
			//user.Role = role; 

			// authentication successful so generate jwt token
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
			List<Claim> ClaimRoles = new List<Claim>();
			//Roles.Add(new Claim(ClaimTypes.Role, "user"));
			//Roles.Add(new Claim(ClaimTypes.Role, "admin"));
			foreach (string role in Roles)
			{
				ClaimRoles.Add(new Claim(ClaimTypes.Role, role));
			}

			var ClaimsList = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.NameIdentifier, UserId),

				});
			ClaimsList.AddClaims(ClaimRoles);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = ClaimsList,
				//Expires = DateTime.UtcNow.AddHours(2),
				Expires = DateTime.UtcNow.AddSeconds(1),
				//IssuedAt = DateTime.UtcNow.AddHours(2),

				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			user.Token = tokenHandler.WriteToken(token);

			

			return user;
		}

	}




}
