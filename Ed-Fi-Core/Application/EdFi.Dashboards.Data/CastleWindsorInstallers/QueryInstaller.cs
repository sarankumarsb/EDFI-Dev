// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace EdFi.Dashboards.Data.CastleWindsorInstallers
{
	/// <summary>
	/// Registers the Query classes with the Castle Windsor Inversion of Control container.
	/// </summary>
	public class QueryInstaller<TMarker> : IWindsorInstaller
	{
		public virtual void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(
				Classes
					.FromAssemblyContaining<TMarker>()
                    .Where(t => t.Name.EndsWith("Query") && t.BaseType != typeof(MulticastDelegate))
				);
		}
	}
}
