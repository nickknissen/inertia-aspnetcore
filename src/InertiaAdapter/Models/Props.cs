using InertiaAdapter.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InertiaAdapter.Models
{

    public class Props
    {
        public Dictionary<string, object> Controller { get; set; }
        public Dictionary<string, object> Share { get; set; }
        public object? With { get; set; }

        public Dictionary<string, object> MergedProps()
        {
            Controller ??= new Dictionary<string, object>();
            Share ??= new Dictionary<string, object>();
            With ??= new { };

            var dict = With.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                     .ToDictionary(prop => Helpers.ToCamelCase(prop.Name), prop => prop.GetValue(With, null));

            return dict 
                .Concat((IEnumerable<KeyValuePair<string, object>>)Controller)
                .Concat(Share)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        }
    }
}