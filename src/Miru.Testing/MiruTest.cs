using System;
using Miru.Foundation.Logging;
using NUnit.Framework;
using Serilog;
using Serilog.Core;

namespace Miru.Testing
{
    public abstract class MiruTest
    {
        private static readonly Lazy<ILogger> logger = new Lazy<ILogger>(Serilog.Log.ForContext<MiruTest>);

        public static ILogger Log => logger.Value;

        private readonly TestConfigRunner _testConfigRunner;

        protected readonly TestFixture _;

        public MiruTest()
        {
            var app = TestMiruHost.StartOrGetApp(GetType().Assembly);

            _ = app.Get<TestFixture>();
            
            _testConfigRunner = app.Get<TestConfigRunner>();
        }

        [OneTimeSetUp]
        public void BaseOneTimeSetup()
        {
            _testConfigRunner.RunBeforeAll(GetType());
        }
        
        [SetUp]
        public void BaseTestSetup()
        {
            _testConfigRunner.RunBeforeEach(GetType());
        }

        [TearDown]
        public void BaseTestTeardown()
        {
            _testConfigRunner.RunAfterEach(GetType());
        }
        
        [OneTimeTearDown]
        public void BaseTestOneTimeTeardown()
        {
            _testConfigRunner.RunAfterAll(GetType());
        }
    }
}