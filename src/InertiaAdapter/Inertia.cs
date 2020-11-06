using InertiaAdapter.Core;
using InertiaAdapter.Extensions;
using InertiaAdapter.Interfaces;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;

namespace InertiaAdapter
{
    public static class Inertia
    {
        private static IResultFactory _factory = new NullFactory();

        public static void Init(IResultFactory factory) => _factory = factory;

        public static Result Render(string component, object props) =>
            _factory.Render(component, props);

        public static Result Render(string component) =>
            _factory.Render(component, new { });

        public static Result RedirectBack()
        {
            var result = _factory.Render("", new { });
            result.RirectBack();

            return result;
        }

        public static string? GetVersion() => _factory.GetVersion();

        public static void SetRootView(string s) => _factory.SetRootView(s);

        public static void Version(string s) => _factory.Version(s);

        public static IHtmlContent Html(dynamic m) => _factory.Html(m);

        public static void Version(Func<string> s) => _factory.Version(s);

        public static void ShareData(string key, object value) => _factory.ShareData(key, value);
        public static void AddSharedDataResolver(ISharedDataResolver dataResolver) => _factory.AddSharedDataResolver(dataResolver);
        public static void SharedDataResolvers(List<ISharedDataResolver> dataResolvers) => _factory.SharedDataResolvers = dataResolvers;
            
    }
}