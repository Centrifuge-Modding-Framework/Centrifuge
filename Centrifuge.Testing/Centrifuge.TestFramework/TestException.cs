using System;

namespace Centrifuge.TestFramework
{
    public class TestException : Exception
    {
        public TestException(string message) : base(message)
        {
        }
    }
}