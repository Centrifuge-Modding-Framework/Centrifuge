using System;
using Centrifuge.TestFramework;
using Reactor.API.Attributes;
using Reactor.API.Interfaces.Systems;
using Reactor.API.Logging;

namespace Centrifuge.Tests
{
    [ModEntryPoint(ModID)]
    public class Mod
    {
        internal const string ModID = "eu.vddcore.Centrifuge.Tests";

        public static IManager Manager { get; private set; }
        public static Mod Instance { get; private set; }

        public static bool InitFinishedCalled { get; private set; }

        public void Initialize(IManager manager)
        {
            Instance = this;
            Manager = manager;
            Manager.InitFinished += (sender, args) => InitFinishedCalled = true;
        }

        public void Load()
        {
            var testResults = TestRunner.RunAll();
            Console.WriteLine(TestRunner.BuildTestSummary(testResults));
        }
    }
}