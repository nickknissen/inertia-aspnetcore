using InertiaAdapter.Core;
using InertiaAdapter.Extensions;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;

namespace InertiaAdapter.Interfaces
{
    public interface IResultFactory
    {
        void ShareData(string key, object value);

        void SetRootView(string s);

        void Version(string version);

        void Version(Func<string> version);

        string? GetVersion();

        IHtmlContent Html(dynamic model);

        Result Render(string component, object controller);
        void AddSharedDataResolver(ISharedDataResolver dataResolvers);
        List<ISharedDataResolver> SharedDataResolvers { get; set; }
    }
}