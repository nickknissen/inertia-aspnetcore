using InertiaAdapter.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

namespace InertiaAdapter.Filters 
{
    public class InertiaActionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Method == "POST" && !context.ModelState.IsValid)
            {
                Controller controller = (Controller)context.Controller;
                var validation = ValidateModel(context.ModelState);
                controller.TempData.TryAdd("errors", validation);
            }

            base.OnActionExecuting(context);

        }

        public override void OnActionExecuted (ActionExecutedContext context)
        {
            if (!(context.Result is Result))
            {
                return;
            }

            Controller controller = (Controller)context.Controller;

            HttpMethodActionConstraint contraint = (HttpMethodActionConstraint)context.ActionDescriptor.ActionConstraints?.FirstOrDefault();

	    // contraint is null when method does not have HttpGet attribute
            if (contraint != null && !contraint.HttpMethods.Any(m => m == "GET"))
            {
                return;
            }

            var errors = (IDictionary<string, string>)controller.TempData["errors"];

            if (errors!= null)
            {
                _ = (context.Result as Result).Errors(errors);
            }

            base.OnActionExecuted(context);
        }

        private Dictionary<string, string> ValidateModel(ModelStateDictionary modelState)
        {
            return (from kvp in modelState
                    let field = kvp.Key
                    let state = kvp.Value
                    let errors = state.Errors.Select(e => e.ErrorMessage)
                    where state.Errors.Count > 0
                    select new
                    {
                        Key = kvp.Key.ToLower(),
                        Errors = errors.FirstOrDefault(),
                    })
                .ToDictionary(e => e.Key, e => e.Errors);
        }
    }

}
