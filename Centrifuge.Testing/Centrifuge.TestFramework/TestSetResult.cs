using System;
using System.Collections.Generic;
using System.Linq;

namespace Centrifuge.TestFramework
{
    public class TestSetResult
    {
        public List<TestResult> IndividualTestResults { get; } = new List<TestResult>();
        
        public string Name { get; internal set; }
        public bool Passed { get; internal set; }
        
        public TestSetStage Stage { get; internal set; }
        public Exception Exception { get; internal set; }
    }
}