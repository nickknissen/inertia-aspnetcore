using System;
using System.Collections.Generic;
using InertiaAdapter.Core;
using Microsoft.AspNetCore.Html;

namespace InertiaAdapter.Interfaces
{
    public interface IResultFactory
    {
        void Share(string key, object obj);

        void Share(string key, Func<object> func);

        Dictionary<string, object> GetShared();

        object GetSharedByKey(string key);

        void SetRootView(string s);

        void Version(string version);

        void Version(Func<string> version);

        string? GetVersion();

        IHtmlContent Html(dynamic model);

        Result Render(string component, object controller);
    }
}
