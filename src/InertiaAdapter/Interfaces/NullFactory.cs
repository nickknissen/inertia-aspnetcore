using InertiaAdapter.Core;
using InertiaAdapter.Extensions;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;

namespace InertiaAdapter.Interfaces
{
    internal class NullFactory : IResultFactory
    {
        public object? Share { get; set; }
        public List<ISharedDataResolver> SharedDataResolvers { get; set; }

        public void SetRootView(string s) => throw new NotImplementedException();

        public void Version(string version) => throw new NotImplementedException();

        public void Version(Func<string> version) => throw new NotImplementedException();

        public string GetVersion() => throw new NotImplementedException();

        public IHtmlContent Html(dynamic model) => throw new NotImplementedException();

        public Result Render(string component, object controller) => throw new NotImplementedException();

        public void ShareData(string key, object value) => throw new NotImplementedException();

        public void AddSharedDataResolver(ISharedDataResolver dataResolvers) => throw new NotImplementedException();
    }
}