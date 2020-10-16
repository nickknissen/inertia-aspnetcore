using InertiaAdapter.Extensions;
using InertiaAdapter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InertiaAdapter.Core
{
    public class Result : IActionResult
    {
        private readonly string _component;
        private readonly Props _props;
        private readonly string _rootView;
        private readonly string? _version;
        private Page? _page;
        private bool _redirectBack;
        private ActionContext? _context;
        private IDictionary<string, object>? _viewData;

        internal Result(Props props, string component, string rootView, string? version) =>
            (_props, _component, _rootView, _version) = (props, component, rootView, version);

        public IActionResult With(object with)
        {
            _props.With = with;
            return this;
        }

        public IActionResult Errors(IDictionary<string, string> errors)
        {
            _props.Errors = errors;

            return this;
        }
        public IActionResult Errors(ModelStateDictionary modelState)
        {
            _props.Errors = (from kvp in modelState
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

            return this;
        }

        public IActionResult WithSuccessMessage(string msg)
        {
            _props.Flash.Success = msg;
            return this;
        }

        public IActionResult WithErrorMessage(string msg)
        {
            _props.Flash.Error = msg;
            return this;
        }

        public bool HasErrors()
        {
            return _props.Errors != null;
        }

        public IActionResult WithViewData(IDictionary<string, object> viewData)
        {
            _viewData = viewData;
            return this;
        }
        
        public IActionResult RirectBack()
        {
            _redirectBack = true;
            return this;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            SetContext(context);

            var (isPartial, only) = PartialRequest();

            if (isPartial)
                _props.Controller = InvokeIfLazy(only);

            ConstructPage();

            string referer = context.HttpContext.Request.Headers["Referer"];
            string path = context.HttpContext.Request.Path;
            if (_redirectBack)
            {
                context.HttpContext.Response.StatusCode = 302;
                context.HttpContext.Response.Headers.Add("Location", referer);
            }

            await GetResult().ExecuteResultAsync(_context);
        }

        private object InvokeIfLazy(IEnumerable<string> str) =>
            str.ToDictionary(o => o, o =>
            {
                var obj = _props.Controller.Value(o);

                return obj.IsLazy() ? ((dynamic) obj).Value : obj;
            });

        private ViewResult View() => new ViewResult
        {
            ViewName = _rootView,
            ViewData = ConstructViewData()
        };

        private JsonResult Json()
        {
            _context.ConfigureResponse();
            return new JsonResult(_page);
        }

        private void ConstructPage()
        {
            _page = new Page
            {
                Props = _props.MergedProps(),
                Component = _component,
                Version = _version,
                Url = _context.RequestedUri()
            };
        }

        private IActionResult GetResult() =>
            IsInertiaRequest() ? (IActionResult) Json() : View();


        private (bool, IList<string>) PartialRequest()
        {
            var only = _props.Controller.Only(PartialData());

            return (ComponentName() != _component && only.Count > 0, only);
        }

        private ViewDataDictionary ConstructViewData() => new ViewData(_page, _context, _viewData).ViewDataDictionary;

        private IList<string> PartialData() => _context.GetPartialData();

        private bool IsInertiaRequest() => _context.IsInertiaRequest();

        private string ComponentName() => _context.ComponentName();

        private void SetContext(ActionContext context) => _context = context;
    }
}