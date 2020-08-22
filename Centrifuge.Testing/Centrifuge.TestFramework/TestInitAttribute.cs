using System;

namespace Centrifuge.TestFramework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestInitAttribute : Attribute
    {
    }
}