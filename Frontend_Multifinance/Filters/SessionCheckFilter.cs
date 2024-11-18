using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Frontend_Multifinance.Filters
{
    public class SessionCheckFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Get the action name and controller name from the route data
            var actionName = context.RouteData.Values["action"]?.ToString();
            var controllerName = context.RouteData.Values["controller"]?.ToString();

            // Retrieve the session data
            var sessionData = context.HttpContext.Session.GetString("UserData");

            // Check if the session is null or empty and if the current action is not Login
            if (string.IsNullOrEmpty(sessionData) && (actionName != "Login" || controllerName != "Account"))
            {
                // Redirect to Login page if session does not exist and it's not the login action
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // This method is executed after the action executes, but we don't need to do anything here
        }
    }
}
