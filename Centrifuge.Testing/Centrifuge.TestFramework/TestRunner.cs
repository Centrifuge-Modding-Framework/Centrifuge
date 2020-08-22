using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Centrifuge.TestFramework
{
    public static class TestRunner
    {
        public static List<TestSetResult> RunAll()
        {
            var testedAssembly = Assembly.GetCallingAssembly();

            var testTypes = testedAssembly.GetExportedTypes().Where(
                t => t.GetCustomAttributes(true).SingleOrDefault(x => (x is TestSetAttribute)) != null);

            var testSetInstances = new List<object>();

            foreach (var testType in testTypes)
            {
                try
                {
                    testSetInstances.Add(Activator.CreateInstance(testType));
                }
                catch (Exception)
                {
                }
            }

            var testSetResults = new List<TestSetResult>();

            foreach (var testSet in testSetInstances)
            {
                var testSetResult = new TestSetResult
                {
                    Name = testSet.GetType().Name,
                    Passed = true
                };

                var initMethod = testSet.GetType().GetMethods().SingleOrDefault(
                    m => m.GetCustomAttributes(true).SingleOrDefault(x => (x is TestInitAttribute)) != null);

                var exitMethod = testSet.GetType().GetMethods().SingleOrDefault(
                    m => m.GetCustomAttributes(true).SingleOrDefault(x => (x is TestExitAttribute)) != null);

                var testMethods = testSet.GetType().GetMethods().Where(
                    m => m.GetCustomAttributes(true).SingleOrDefault(x => (x is TestAttribute)) != null);

                try
                {
                    initMethod?.Invoke(testSet, new object[] { });
                }
                catch (Exception e)
                {
                    testSetResult.Stage = TestSetStage.Init;
                    testSetResult.Exception = e;
                    testSetResult.Passed = false;

                    testSetResults.Add(testSetResult);
                    continue;
                }

                foreach (var execMethod in testMethods)
                {
                    var testResult = new TestResult
                    {
                        Name = execMethod.Name,
                        Passed = true
                    };

                    try
                    {
                        execMethod?.Invoke(testSet, new object[] { });
                    }
                    catch (Exception e)
                    {
                        testResult.Passed = false;
                        testResult.Exception = e;
                    }

                    testSetResult.IndividualTestResults.Add(testResult);
                }

                try
                {
                    exitMethod?.Invoke(testSet, new object[] { });
                }
                catch (Exception e)
                {
                    testSetResult.Stage = TestSetStage.Exit;
                    testSetResult.Passed = false;
                    testSetResult.Exception = e;

                    testSetResults.Add(testSetResult);
                    continue;
                }

                testSetResult.Stage = TestSetStage.Finish;
                testSetResult.Passed = testSetResult.IndividualTestResults.All(x => x.Passed)
                                       || !testSetResult.IndividualTestResults.Any();

                testSetResults.Add(testSetResult);
            }

            return testSetResults;
        }

        public static string BuildTestSummary(List<TestSetResult> testSetResults)
        {
            var sb = new StringBuilder();

            foreach (var testSetResult in testSetResults)
            {
                sb.AppendLine($"[{(testSetResult.Passed ? "PASS" : "FAIL")}] {testSetResult.Name}");

                if (testSetResult.Stage == TestSetStage.Init)
                {
                    sb.AppendIndentedLine("The test failed prematurely - at initialization phase.", 2)
                        .AppendException(testSetResult.Exception);
                }
                else if (testSetResult.Stage == TestSetStage.Exit)
                {
                    sb.AppendIndentedLine("The test failed at exit phase.", 2)
                        .AppendException(testSetResult.Exception);
                }
                else
                {
                    foreach (var testResult in testSetResult.IndividualTestResults)
                    {
                        sb.AppendIndentedLine($"[{(testResult.Passed ? "PASS" : "FAIL")}] {testResult.Name}", 2);

                        if (!testResult.Passed)
                        {
                            sb.AppendException(testResult.Exception, 2);
                            sb.AppendLine();
                        }
                    }
                }
            }

            sb.AppendLine()
                .AppendLine($"Total: {testSetResults.Count(x => x.Passed)}/{testSetResults.Count} test sets passed.")
                .AppendLine(
                    $"{testSetResults.Count(x => x.Stage == TestSetStage.Finish)}/{testSetResults.Count} test sets made it to the end.");

            return sb.ToString();
        }

        private static StringBuilder AppendIndentedLine(this StringBuilder sb, string s, int indent)
            => sb.AppendLine(s.PadLeft(indent, ' '));

        private static StringBuilder AppendException(this StringBuilder sb, Exception e, int indent = 0)
        {
            sb.AppendIndentedLine($"Exception: {e.Message}", indent + 2);

            foreach (var line in e.StackTrace.Split('\n').Select(s => s.TrimEnd()))
                sb.AppendIndentedLine(line, indent + 4);

            return sb;
        }
    }
}