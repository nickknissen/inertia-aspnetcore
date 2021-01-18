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
        public void CanAddOrUpdateObjectWithTheSameKey()
        {
            var shared = new SharedProps();

            const string Key = "Foo";

            shared.AddOrUpdate(Key, "Foo");
            shared.AddOrUpdate(Key, "Bar");

            Assert.Equal("Bar", shared.GetValue(Key));
        }

        [Fact]
        public void CanAddOrUpdateDelegateWithTheSameKey()
        {
            var shared = new SharedProps();

            const string Key = "Foo";

            shared.AddOrUpdate(Key, () => "Foo");
            shared.AddOrUpdate(Key, () => "Bar");

            Assert.Equal("Bar", shared.GetValue(Key));
        }

        [Fact]
        public void CanAddOrUpdateDelegateAndObjectWithTheSameKey()
        {
            var shared = new SharedProps();

            const string Key = "Foo";

            shared.AddOrUpdate(Key, "Foo");
            shared.AddOrUpdate(Key, () => "Bar");

            Assert.Equal("Bar", shared.GetValue(Key));
        }

        [Fact]
        public void CanAddStringWithObjectAndGetValue()
        {
            const string Key1 = "Foo1";
            const string Key2 = "Foo2";
            object value1 = "Bar1";
            object value2 = "Bar2";

            var shared = new SharedProps();

            shared.AddOrUpdate(Key1, value1);
            shared.AddOrUpdate(Key2, value2);

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

            shared.AddOrUpdate(Key1, value1);
            shared.AddOrUpdate(Key2, value2);

            Assert.Equal(2, shared.Value.Count);
            Assert.Equal(value1(), shared.GetValue(Key1));
            Assert.Equal(value2(), shared.GetValue(Key2));
        }


        [Fact]
        public void ValueIsDictionaryAndEmpty()
        {
            var shared = new SharedProps();

            Assert.IsType<Dictionary<string, object>>(shared.Value);
            Assert.Empty(shared.Value);
        }

        [Fact]
        public void ValueTest()
        {
            var shared = new SharedProps();

            shared.AddOrUpdate("Foo", "Bar");
            Assert.Single(shared.Value);
            Assert.Equal("Bar", shared.GetValue("Foo"));

            shared.AddOrUpdate("Foo", "Bar");
            Assert.Single(shared.Value);
            Assert.Equal("Bar", shared.GetValue("Foo"));

            shared.AddOrUpdate("Foo", "Bar1");
            Assert.Single(shared.Value);
            Assert.Equal("Bar1", shared.GetValue("Foo"));

            shared.AddOrUpdate("Foo", () => "Bar");
            Assert.Single(shared.Value);
            Assert.Equal("Bar", shared.GetValue("Foo"));

            shared.AddOrUpdate("Foo1", () => "Bar1");
            Assert.Equal(2, shared.Value.Count);
            Assert.Equal("Bar", shared.GetValue("Foo"));
            Assert.Equal("Bar1", shared.GetValue("Foo1"));
        }
    }
}
