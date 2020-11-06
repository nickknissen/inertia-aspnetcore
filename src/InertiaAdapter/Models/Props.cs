using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InertiaAdapter.Models
{

    public class Flash
    {
        public string Error { get; set; } = "";
        public string Success { get; set; } = "";
    }


    public class Props
    {
        public object? Controller { get; set; }
        public Dictionary<string, object> Share { get; set; }
        public object? With { get; set; }
        public IDictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();
        public Flash Flash { get; set; } = new Flash();


        public Dictionary<string, object> MergedProps()
        {

            Controller ??= new { };
            Share ??= new Dictionary<string, object>();
            With ??= new { };

            object props = new  {
                Errors,
                Flash,
            };

            props = TypeMerger.TypeMerger.Merge(props, Controller);
            props = TypeMerger.TypeMerger.Merge(props, With);

            var objectDict = props.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                     .ToDictionary(prop => prop.Name.ToLower(), prop => prop.GetValue(props, null));

            return objectDict.Concat(Share).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        }
    }
}