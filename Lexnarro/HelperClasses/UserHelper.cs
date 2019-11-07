using System.Linq;

namespace Lexnarro.HelperClasses
{
    public class UserHelper 
    {
        public static string GetUserId()
        {
            var identity = (System.Security.Claims.ClaimsPrincipal)System.Threading.Thread.CurrentPrincipal;
            var principal = System.Threading.Thread.CurrentPrincipal as System.Security.Claims.ClaimsPrincipal;
            var userId = identity.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            return userId;
        }

        public static string GetUserName()
        {
            var identity = (System.Security.Claims.ClaimsPrincipal)System.Threading.Thread.CurrentPrincipal;
            var principal = System.Threading.Thread.CurrentPrincipal as System.Security.Claims.ClaimsPrincipal;
            var name = identity.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();
            return name;
        }

        public static string GetUserMail()
        {
            var identity = (System.Security.Claims.ClaimsPrincipal)System.Threading.Thread.CurrentPrincipal;
            var principal = System.Threading.Thread.CurrentPrincipal as System.Security.Claims.ClaimsPrincipal;
            var mail = identity.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
            return mail;
        }

        public static string GetUserRole()
        {
            var identity = (System.Security.Claims.ClaimsPrincipal)System.Threading.Thread.CurrentPrincipal;
            var principal = System.Threading.Thread.CurrentPrincipal as System.Security.Claims.ClaimsPrincipal;
            var role = identity.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).SingleOrDefault();
            return role;
        }

        public static string GetUserStateEnrolledId()
        {
            var identity = (System.Security.Claims.ClaimsPrincipal)System.Threading.Thread.CurrentPrincipal;
            var principal = System.Threading.Thread.CurrentPrincipal as System.Security.Claims.ClaimsPrincipal;
            string stateEnrolledId = identity.FindFirst("StateEnrolledId").Value;            
            return stateEnrolledId;
        }

        public static string GetUserStateEnrolledName()
        {
            var identity = (System.Security.Claims.ClaimsPrincipal)System.Threading.Thread.CurrentPrincipal;
            var principal = System.Threading.Thread.CurrentPrincipal as System.Security.Claims.ClaimsPrincipal;
            string stateName = identity.FindFirst("StateEnrolled").Value;

            return stateName;
        }

        public static string GetUserStateEnrolledShortName()
        {
            var identity = (System.Security.Claims.ClaimsPrincipal)System.Threading.Thread.CurrentPrincipal;
            var principal = System.Threading.Thread.CurrentPrincipal as System.Security.Claims.ClaimsPrincipal;
            string stateShortName = identity.FindFirst("StateShortName").Value;

            return stateShortName;
        }
    }
}