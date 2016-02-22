using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EdFi.Dashboards.Presentation.Architecture.Mvc.Core;
using NUnit.Framework;

namespace EdFi.Dashboards.Presentation.Core.Tests.Routing
{
    [TestFixture]
    public class When_resolving_if_a_controller_has_a_GET_action_with_NO_HTTPMethod_Attributes
    {
        public class ControllerWithGetAction : Controller
        {
            public virtual ActionResult Get()
            {
                return null;
            }
        }

        [Test]
        public void Should_Return_True()
        {
            var controllerFactory = new EdFiControllerFactory();
            var controllerType = typeof(ControllerWithGetAction);

            Assert.That(controllerFactory.ControllerContainsAction(controllerType, "GET", "GET"), Is.True);
        }
    }

    [TestFixture]
    public class When_resolving_if_a_controller_has_a_NAMED_action_with_NO_HTTPMethod_Attributes
    {
        public class ControllerWithNamedAction : Controller
        {
            public virtual ActionResult NamedActionResult()
            {
                return null;
            }
        }

        [Test]
        public void Should_Return_True()
        {
            var controllerFactory = new EdFiControllerFactory();
            var controllerType = typeof(ControllerWithNamedAction);

            Assert.That(controllerFactory.ControllerContainsAction(controllerType, "NamedActionResult", "GET"), Is.True);
        }
    }

    [TestFixture]
    public class When_resolving_if_a_controller_has_a_NAMED_action_with_ANY_HTTPMethod_Attributes
    {
        readonly EdFiControllerFactory controllerFactory = new EdFiControllerFactory();
        readonly Type controllerType = typeof(ControllerWithNamedActionAndAnyAttribute);

        public class ControllerWithNamedActionAndAnyAttribute : Controller
        {
            [HttpGet]
            [HttpPost, HttpPut]
            [HttpDelete]
            public virtual ActionResult NamedActionResult()
            {
                return null;
            }
        }

        [Test]
        public void Should_Return_True_for_POST()
        {
            Assert.That(controllerFactory.ControllerContainsAction(controllerType, "NamedActionResult", "POST"), Is.True);
        }

        [Test]
        public void Should_Return_True_for_PUT()
        {
            Assert.That(controllerFactory.ControllerContainsAction(controllerType, "NamedActionResult", "PUT"), Is.True);
        }

        [Test]
        public void Should_Return_True_for_DELETE()
        {
            Assert.That(controllerFactory.ControllerContainsAction(controllerType, "NamedActionResult", "DELETE"), Is.True);
        }

        [Test]
        public void Should_Return_True_for_GET()
        {
            Assert.That(controllerFactory.ControllerContainsAction(controllerType, "NamedActionResult", "GET"), Is.True);
        }
    }

    /// <summary>
    /// This covers the scenario for Student Lists where we have a Get method attributed with a [HttpPost]
    /// </summary>
    [TestFixture]
    public class When_resolving_if_a_controller_has_a_GET_action_attributed_with_HttpPost
    {
        public class ControllerWithGetActionAttributedWithHttpPost : Controller
        {
            [HttpPost]
            public virtual ActionResult Get()
            {
                return null;
            }
        }

        [Test]
        public void Should_Return_True()
        {
            var controllerFactory = new EdFiControllerFactory();
            var controllerType = typeof(ControllerWithGetActionAttributedWithHttpPost);

            Assert.That(controllerFactory.ControllerContainsAction(controllerType, "GET", "POST"), Is.True);
        }
    }

    [TestFixture]
    public class When_resolving_if_a_controller_does_NOT_have_a_GET_action_attributed_with_HttpPost
    {
        public class ControllerWithGetAction : Controller
        {
            public virtual ActionResult Get()
            {
                return null;
            }
        }

        [Test]
        public void Should_Return_False()
        {
            var controllerFactory = new EdFiControllerFactory();
            var controllerType = typeof(ControllerWithGetAction);

            Assert.That(controllerFactory.ControllerContainsAction(controllerType, "GET", "POST"), Is.False);
        }
    }

    /// <summary>
    /// This covers the Scenario where we have a custom controller that does not have a DELETE action method which should be resolved by the ServicePassthroughController.
    /// Note: the routed action is mostly allways defaulted to GET becuase of our REST approach. I.E. you would access the LEA resource as ~/LEA/1 and not ~/LEA/Get/1
    /// </summary>
    [TestFixture]
    public class When_resolving_if_a_controller_does_NOT_have_a_DELETE_action
    {
        public class ControllerWithGetAction : Controller
        {
            public virtual ActionResult Get()
            {
                return null;
            }
        }

        [Test]
        public void Should_Return_False()
        {
            var controllerFactory = new EdFiControllerFactory();
            var controllerType = typeof(ControllerWithGetAction);

            Assert.That(controllerFactory.ControllerContainsAction(controllerType, "GET", "DELETE"), Is.False);
        }
    }
}
