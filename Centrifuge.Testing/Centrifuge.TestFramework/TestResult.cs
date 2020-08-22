using System;

namespace Centrifuge.TestFramework
{
    public class TestResult
    {
        public string Name { get; internal set; }
        public bool Passed { get; internal set; }
        
        public Exception Exception { get; internal set; }
    }
}