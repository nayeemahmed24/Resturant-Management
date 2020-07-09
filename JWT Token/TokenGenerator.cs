using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JWT_Token.Configurations;
using Microsoft.IdentityModel.Tokens;
using Model;
using Model.Entities;

namespace JWT_Token
{
    public class TokenGenerator : ITokenGenerator
    {
        private JwtSecurityTokenHandler tokenHandler;
        private SecurityTokenDescriptor tokenDescriptor;
        public TokenGenerator(IJwtSetting settings)
        {
            tokenHandler = new JwtSecurityTokenHandler();
        }

        public String generateToken(RestaurantModel userModel, string secretKey, int lifeSpanInHours)
        {
            var key = Encoding.ASCII.GetBytes(secretKey);
            tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(Claims.UserId, userModel.Id.ToString()),
                    new Claim(Claims.Role, userModel.role)
                }),
                Expires = DateTime.UtcNow.AddHours(lifeSpanInHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            String userToken = tokenHandler.WriteToken(token);

            return userToken;
        }
        public String generateToken(AdminUserModel userModel, string secretKey, int lifeSpanInHours)
        {
            var key = Encoding.ASCII.GetBytes(secretKey);
            tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(Claims.UserId, userModel.Id.ToString()),
                    new Claim(Claims.Role, userModel.role)
                }),
                Expires = DateTime.UtcNow.AddHours(lifeSpanInHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            String userToken = tokenHandler.WriteToken(token);

            return userToken;
        }



    }
    public interface ITokenGenerator
    {
        public String generateToken(RestaurantModel userModel, string secretKey, int lifeSpanInHours);
        public String generateToken(AdminUserModel userModel, string secretKey, int lifeSpanInHours);
    }
}
