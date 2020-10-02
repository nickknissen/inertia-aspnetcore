using System.Collections.Generic;

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
        public object? Share { get; set; }
        public object? With { get; set; }
        public IDictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();
        public Flash Flash { get; set; } = new Flash();
    }
}