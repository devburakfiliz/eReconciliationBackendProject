using Business.Constans;
using Castle.DynamicProxy;
using Core.Extensions;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Business.BusinessAspect
{
    public class SecuredOperation : MethodInterception
    {
        private string[] _roles;
        private IHttpContextAccessor _httpContextAccessor;

        public SecuredOperation(string roles)
        {
            _roles = roles.Split(",");
            _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
        }

        protected override void Onbefore(IInvocation invocation)
        {
            var token = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");

            if (token != "")

            {

                var handler = new JwtSecurityTokenHandler();

                var jwtSecurityToken = handler.ReadJwtToken(token);

                var decodeToken = jwtSecurityToken.Claims;

                foreach (var claim in decodeToken)

                {

                    foreach (var role in _roles)

                    {

                        if (claim.ToString().Contains(role))

                        {

                            return;

                        }

                    }

                }

            }

            throw new Exception("İşlem için yetkiniz bulunmuyor");
        }
    }
}
