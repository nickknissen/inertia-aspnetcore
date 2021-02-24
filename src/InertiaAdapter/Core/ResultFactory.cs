using InertiaAdapter.Extensions;
using InertiaAdapter.Interfaces;
using InertiaAdapter.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Web;

namespace InertiaAdapter.Core
{
    internal class ResultFactory : IResultFactory
    {
        private Dictionary<string, object> Share { get; set; } = new Dictionary<string, object>();
        public List<ISharedDataResolver> SharedDataResolvers { get; set; } = new List<ISharedDataResolver>();

        private string _rootView = "Views/App.cshtml";
        private object? _version;

        public void SetRootView(string s) => _rootView = s;

        public void Version(string version) => _version = version;

        public void Version(Func<string> version) => _version = version;

        public string? GetVersion() =>
            _version switch
            {
                Func<string> func => func(),
                string s => s,
                _ => null
            };

        public IHtmlContent Html(dynamic model)
        {
            string data = JsonSerializer.Serialize(model,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            data = HttpUtility.HtmlEncode(data);


            return new HtmlString($"<div id=\"app\" data-page=\"{data}\"></div>");
        }

        public IActionResult Location(string url) => new LocationResult(url);

        public Result Render(string component, object controllerProps)
        {
            var props = controllerProps.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                     .ToDictionary(prop => prop.Name.ToCamelCase(), prop => prop.GetValue(controllerProps, null));

            return new Result(
                new Props { Controller = props, Share = Share }, 
                component, 
                _rootView, 
                GetVersion(),
                SharedDataResolvers
            );
        }

        public void ShareData(string key, object value)
        {
            Share.Add(key, value);
        }

        public void AddSharedDataResolver(ISharedDataResolver dataResolvers)
        {
            SharedDataResolvers.Add(dataResolvers);
        }
    }
}
