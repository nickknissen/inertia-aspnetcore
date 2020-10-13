using System.Dynamic;

namespace InertiaAdapter.Models
{
    public class Page
    {
        public string? Component { get; set; }
        public string? Version { get; set; }
        public string? Url { get; set; }
        public object Props { get; set; } = new { };
    }
}