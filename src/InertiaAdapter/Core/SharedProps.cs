using System;
using System.Collections.Generic;
using System.Linq;
using InertiaAdapter.Exceptions;

namespace InertiaAdapter.Core
{
    internal class SharedProps
    {
        private readonly Dictionary<string, object> _stringWithObjects = new();

        private readonly Dictionary<string, Func<object>> _stringWithDelegates = new();

        private void CheckKey(string s)
        {
            if (KeyExists(s))
                throw new DuplicateKeysException("You can not have duplicate keys for shared props.");
        }

        private bool KeyExists(string s) => _stringWithObjects.ContainsKey(s) || _stringWithDelegates.ContainsKey(s);


        public void Add(string s, object o)
        {
            CheckKey(s);
            _stringWithObjects.Add(s, o);
        }

        public void Add(string s, Func<object> func)
        {
            CheckKey(s);
            _stringWithDelegates.Add(s, func);
        }


        public object GetValue(string s)
        {
            if (_stringWithObjects.TryGetValue(s, out var o)) return o;

            if (_stringWithDelegates.TryGetValue(s, out var f)) return f();

            throw new KeyDoesNotExistException("The key for shared props does not exist.");
        }

        public Dictionary<string, object> Value =>
            _stringWithObjects
                .Concat(_stringWithDelegates
                    .ToDictionary(c => c.Key, c => c.Value()))
                .ToDictionary(c => c.Key, c => c.Value);
    }
}
