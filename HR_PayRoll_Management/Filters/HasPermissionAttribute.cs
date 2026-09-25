using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR_PayRoll_Management.DAL;

namespace HR_PayRoll_Management.Filters
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        private readonly string _permission;

        public HasPermissionAttribute(string permission)
        {
            _permission = permission;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!base.AuthorizeCore(httpContext)) return false;

            var username = httpContext.User.Identity.Name;
            using (var db = new AppDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Username == username);
                if (user == null) return false;

                // Special case for Admin role - they have all permissions
                var role = db.Roles.FirstOrDefault(r => r.RoleId == user.RoleId);
                if (role != null && role.RoleName == "Admin") return true;

                // Check if the user's role has the required permission
                bool hasPermission = db.RolePermissions
                    .Any(rp => rp.RoleId == user.RoleId && rp.Permission.PermissionName == _permission);
                
                return hasPermission;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult
                {
                    Data = new { success = false, message = "Access Denied: You do not have permission to perform this action." },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
                filterContext.HttpContext.Response.StatusCode = 403;
            }
            else
            {
                filterContext.Result = new ViewResult { ViewName = "AccessDenied" };
            }
        }
    }
}
