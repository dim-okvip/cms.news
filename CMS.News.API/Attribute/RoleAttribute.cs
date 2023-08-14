using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;

namespace CMS.News.API.Attribute
{
    public class RoleAttribute : TypeFilterAttribute
    {
        public RoleAttribute(string claimType, string claimValue) : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { new Claim(claimType, claimValue) };
        }

        public class ClaimRequirementFilter : IAuthorizationFilter
        {
            private readonly Claim _claim;
            private readonly IRightHandler _rightHandler;

            public ClaimRequirementFilter(Claim claim, IRightHandler rightHandler)
            {
                _claim = claim;
                _rightHandler = rightHandler;
            }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                var claimedData = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == Constants.CLAIM_TYPE);
                if (claimedData is not null)
                {
                    string jwt = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", String.Empty);
                    var logedInUser = JsonConvert.DeserializeObject<UserQueryResult>(claimedData.Value);
                    Response<List<RightQueryResult>> result = _rightHandler.GetByRoleIdToAuthorizeAsync(jwt, logedInUser).Result;

                    if (result.Data.FirstOrDefault(x => x.Code == _claim.Value) is null)
                    {
                        if (result.Status == HttpStatusCode.Forbidden)
                            context.Result = new ForbidResult();
                        else
                        {
                            if (_claim.Value is not RightName.NOT_AUTHORIZATION)
                                context.Result = new UnauthorizedResult();
                        }
                    }
                }
                else
                    context.Result = new ForbidResult();
            }
        }
    }
}
