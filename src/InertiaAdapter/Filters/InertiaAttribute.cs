using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace InertiaAdapter.Filters 
{
    public class InertiaActionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            List<string> methods =  new List<string>() { "POST", "PUT" };
            if (methods.Contains(context.HttpContext.Request.Method) && !context.ModelState.IsValid)
            {
                Controller controller = (Controller)context.Controller;
                var validation = ValidateModel(context.ModelState);
                controller.TempData.TryAdd("errors", validation);
            }

            base.OnActionExecuting(context);

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
