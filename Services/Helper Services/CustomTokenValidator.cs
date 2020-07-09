using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Services.Helper_Services
{
   
        public class CustomTokenValidator
        {
            private TokenValidationParameters tokenValidationParameters;
            private JwtSecurityTokenHandler jwtSecurityTokenHandler;
            public CustomTokenValidator()
            {
                tokenValidationParameters = new TokenValidationParameters();
                jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            }

            public ClaimsPrincipal validateToken(string token, string Customkey)
            {
                SecurityToken securityToken;
                var key = Encoding.ASCII.GetBytes(Customkey);
                tokenValidationParameters.ValidateIssuerSigningKey = true;
                tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(key);
                tokenValidationParameters.RequireSignedTokens = true;
                tokenValidationParameters.RequireExpirationTime = true;
                tokenValidationParameters.ValidateLifetime = true;
                tokenValidationParameters.ValidateIssuer = false;
                tokenValidationParameters.ValidateAudience = false;

                ClaimsPrincipal user = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

                return user;
            }

        }
    
}
