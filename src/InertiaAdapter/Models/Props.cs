﻿namespace InertiaAdapter.Models
{
    public class Props
    {
        public object? Controller { get; set; }
        public object? Share { get; set; }
        public object? With { get; set; }
        public IDictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();
    }
}