// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using EdFi.Dashboards.Testing.NBuilder;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Implementation;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Testing
{
    public abstract class TestFixtureBase
    {
        protected MockRepository mocks;

        [TestFixtureSetUp]
        public virtual void RunOnceBeforeAny()
        {
            // Initialize NBuilder settings
            BuilderSetup.SetDefaultPropertyNamer(new NonDefaultNonRepeatingPropertyNamer(new ReflectionUtil()));

            // Create a mock repository for new mocks
            mocks = new MockRepository();
            
            EstablishContext();

            // Stop recording
            mocks.ReplayAll();

            try
            {
                // Allow execution of code just prior to test execution
                BeforeExecuteTest();

                // Execute the test
                ExecuteTest();
            }
            finally
            {
                // Allow cleanup surrounding test execution, prior to final cleanup
                AfterExecuteTest();
            }
        }

        [TestFixtureTearDown]
        public virtual void RunOnceAfterAll()
        {
            // Make sure all objects are now in replay mode
            mocks.ReplayAll();

            // Make sure all defined mocks are satisfied
            mocks.VerifyAll();
        }
        
        protected virtual void EstablishContext() { }

        protected virtual void BeforeExecuteTest() { }
        protected virtual void AfterExecuteTest() { }

        /// <summary>
        /// Executes the code to be tested.
        /// </summary>
        protected abstract void ExecuteTest();

        ///// <summary>
        ///// Make sure that the test fixture isn't skipped because there is no actual test defined.  This happens in some of the
        ///// earlier test fixtures that only test interactions with external dependencies, which causes the test runners to skip
        ///// over them.  Once these fixtures can all be identified this method should be removed.
        ///// </summary>
        //[Test]
        //public void Ensure_test_fixture_execution()
        //{
        //}
    }
}
