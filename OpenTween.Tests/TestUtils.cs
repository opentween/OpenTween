using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;

namespace OpenTween
{
    class TestUtils
    {
        public static void CheckDeepCloning(object obj, object cloneObj)
        {
            Assert.That(cloneObj, Is.EqualTo(obj), obj.GetType().Name);
            Assert.That(cloneObj, Is.Not.SameAs(obj), obj.GetType().Name);

            foreach (var field in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var objValue = field.GetValue(obj);
                var cloneValue = field.GetValue(cloneObj);

                Assert.That(cloneValue, Is.EqualTo(objValue), field.Name);
                if (objValue == null && cloneValue == null) continue;
                if (field.FieldType.IsValueType || field.FieldType == typeof(string)) continue;

                Assert.That(cloneValue, Is.Not.SameAs(objValue), field.Name);
            }
        }
    }
}
