// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;

namespace EdFi.Dashboards.Common.Utilities.CastleWindsorInstallers
{
    /// <summary>
    /// Chain of Responsibility order of execution
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ChainOfResponsibilityOrderAttribute : Attribute
    {
        public ChainOfResponsibilityOrderAttribute(int order)
		{
			Order = order;
		}

		public int Order { get; private set; }
    }
}