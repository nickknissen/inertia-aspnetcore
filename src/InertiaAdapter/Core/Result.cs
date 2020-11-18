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
        private List<ISharedDataResolver> _sharedDataResolvers;

        internal Result(Props props, string component, string rootView, string? version, List<ISharedDataResolver> dataResolvers)
        {
            (_props, _component, _rootView, _version, _sharedDataResolvers) = (props, component, rootView, version, dataResolvers);
        }

        public Result With(object with)
        {
            _props.With = with;
            return this;
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


            foreach(var resolver in _sharedDataResolvers)
            {
                var data = await resolver.ResolveDataAsync(context.HttpContext);
                _props.Share = _props.Share.Concat(data).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            ConstructPage();

            string referer = context.HttpContext.Request.Headers["Referer"];
            string path = context.HttpContext.Request.Path;
            if (_redirectBack)
            {
                //Pass error/succes message to next request
                ITempDataDictionaryFactory factory = context.HttpContext.RequestServices.GetService(typeof(ITempDataDictionaryFactory)) as ITempDataDictionaryFactory;
                ITempDataDictionary tempData = factory.GetTempData(context.HttpContext);

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