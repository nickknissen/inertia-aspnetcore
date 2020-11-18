using InertiaAdapter.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InertiaAdapter.Models
{

    public class Props
    {
        public object? Controller { get; set; }
        public Dictionary<string, object> Share { get; set; }
        public object? With { get; set; }

        public Dictionary<string, object> MergedProps()
        {
            Controller ??= new { };
            Share ??= new Dictionary<string, object>();
            With ??= new { };
            object props = new  { };

            bool controllerPropsIsDict = Controller is IDictionary && Controller.GetType().IsGenericType && Controller.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));

            if (!controllerPropsIsDict) 
            {
                props = TypeMerger.TypeMerger.Merge(props, Controller);
            }

            props = TypeMerger.TypeMerger.Merge(props, With);

            var objectDict = props.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                     .ToDictionary(prop => Helpers.ToCamelCase(prop.Name), prop => prop.GetValue(props, null));

            if (controllerPropsIsDict) {
                objectDict = objectDict
                    .Concat((IEnumerable<KeyValuePair<string, object>>)Controller)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            return objectDict
                .Concat(Share)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        }
    }
}