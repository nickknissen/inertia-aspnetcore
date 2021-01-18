using System;
using System.Collections.Generic;
using InertiaAdapter.Core;
using InertiaAdapter.Exceptions;
using Xunit;

namespace Tests.Unit
{
    public class SharedDataTest
    {
        [Fact]
        public void KeyDoesNotExistException()
        {
            var shared = new SharedProps();

            Assert.Throws<KeyDoesNotExistException>(() => shared.GetValue("Key"));
        }

        [Fact]
        public void DuplicateKeysException()
        {
            var shared = new SharedProps();

            const string Key = "Foo";

            Assert.Throws<DuplicateKeysException>(() =>
            {
                shared.Add(Key, "Bar");
                shared.Add(Key, "Bar");
            });
        }

        [Fact]
        public void CanAddStringWithObjectAndGetValue()
        {
            const string Key1 = "Foo1";
            const string Key2 = "Foo2";
            object value1 = "Bar1";
            object value2 = "Bar2";

            var shared = new SharedProps();

            shared.Add(Key1, value1);
            shared.Add(Key2, value2);

            Assert.Equal(2, shared.Value.Count);
            Assert.Equal(value1, shared.GetValue(Key1));
            Assert.Equal(value2, shared.GetValue(Key2));
        }


        [Fact]
        public void CanAddStringWithDelegateAndGetValue()
        {
            const string Key1 = "Foo1";
            const string Key2 = "Foo2";
            Func<object> value1 = () => "Bar1";
            Func<object> value2 = () => "Bar2";

            var shared = new SharedProps();

            shared.Add(Key1, value1);
            shared.Add(Key2, value2);

            Assert.Equal(2, shared.Value.Count);
            Assert.Equal(value1(), shared.GetValue(Key1));
            Assert.Equal(value2(), shared.GetValue(Key2));
        }


        [Fact]
        public void FinalValueIsDictionaryAndEmpty()
        {
            var shared = new SharedProps();

            Assert.IsType<Dictionary<string, object>>(shared.Value);
            Assert.Empty(shared.Value);
        }
    }
}
