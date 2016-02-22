using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.Models.Plugins;
using EdFi.Dashboards.Resources.Plugins;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Resources.Tests.Plugins
{
	public class PluginManifestTest : IPluginManifest
	{
		private readonly string _name;
		private readonly string _version;
		private readonly IEnumerable<PluginMenu> _menus;

		public string Name
		{
			get { return _name; }
		}

		public string Version 
		{
			get { return _version; }
		}

		public IEnumerable<PluginMenu> PluginMenus
		{
			get { return _menus; }
		}

		internal PluginManifestTest(string name, string version, IEnumerable<PluginMenu> menus)
		{
			_name = name;
			_version = version;
			_menus = menus;
		}
	}

	[TestFixture]
	public class PluginResourcesProviderFixture
	{
		[Test]
		public void PluginResourcesProvider_Get_NoManifestsPresent_NotAssigned()
		{
			//Arrange
			PluginResourcesProvider provider = new PluginResourcesProvider();
			List<ResourceModel> retVal = new List<ResourceModel>();

			//Act
			retVal = provider.Get("").ToList();

			//Assert
			Assert.NotNull(retVal);
			Assert.IsTrue(retVal.Count == 0);
		}

		[Test]
		public void PluginResourcesProvider_Get_NoManifestsPresent_AssignedAsNull()
		{
			//Arrange
			PluginResourcesProvider provider = new PluginResourcesProvider();
			provider.PluginManifests = null;
			List<ResourceModel> retVal = new List<ResourceModel>();

			//Act
			retVal = provider.Get("").ToList();

			//Assert
			Assert.NotNull(retVal);
			Assert.IsTrue(retVal.Count == 0);
		}

		[Test]
		public void PluginResourcesProvider_Get_NoManifestsPresentForArea()
		{
			//Arrange
			List<PluginManifestTest> manifests = new List<PluginManifestTest> { new PluginManifestTest("test", "1.0", new List<PluginMenu> { new PluginMenu { Area = "testArea", ResourceModels = new List<ResourceModel> { new ResourceModel { Text = "testModel", Url = "testUrl" } } } }) };

			PluginResourcesProvider provider = new PluginResourcesProvider();
			provider.PluginManifests = manifests.ToArray();

			List<ResourceModel> retVal = new List<ResourceModel>();

			//Act
			retVal = provider.Get("foo").ToList();

			//Assert
			Assert.NotNull(retVal);
			Assert.IsTrue(retVal.Count == 0);
		}

		[Test]
		public void PluginResourcesProvider_Get_ManifestsPresentForArea()
		{
			//Arrange
			List<PluginManifestTest> manifests = new List<PluginManifestTest> 
														{ 
															new PluginManifestTest("test", "1.0", new List<PluginMenu> 
																{ 
																	new PluginMenu 
																	{ 
																		Area = "testArea", 
																		ResourceModels = new List<ResourceModel> 
																		{ 
																			new ResourceModel 
																			{ 
																				Text = "testModel", Url = "testUrl" 
																			} 
																		} 
																	} 
																}),
																new PluginManifestTest("test2", "1.0", new List<PluginMenu>
																	{
																		new PluginMenu
																		{
																			Area = "notTestArea",
																			ResourceModels = new List<ResourceModel>
																			{
																				new ResourceModel
																				{
																					Text = "testModel2", Url = "testUrl"
																				}
																			}
																		}
																	})
														};

			PluginResourcesProvider provider = new PluginResourcesProvider();
			provider.PluginManifests = manifests.ToArray();

			List<ResourceModel> retVal = new List<ResourceModel>();

			//Act
			retVal = provider.Get("testArea").ToList();

			//Assert
			Assert.NotNull(retVal);
			Assert.IsTrue(retVal.Count == 1);
		}

		[Test]
		public void PluginResourcesProvider_Get_ManifestPluginMenuIsNull()
		{
			//Arrange
			List<PluginManifestTest> manifests = new List<PluginManifestTest> { new PluginManifestTest("test", "1.0", null) };
			
			PluginResourcesProvider provider = new PluginResourcesProvider();
			provider.PluginManifests = manifests.ToArray();

			List<ResourceModel> retVal = new List<ResourceModel>();

			//Act
			retVal = provider.Get("").ToList();

			//Assert
			Assert.NotNull(retVal);
			Assert.IsTrue(retVal.Count == 0);
		}

		[Test]
		public void PluginResourcesProvider_Get_ManifestPluginMenuResourceModelsIsNull()
		{
			//Arrange
			List<PluginManifestTest> manifests = new List<PluginManifestTest> { new PluginManifestTest("test", "1.0", new List<PluginMenu> { new PluginMenu { Area = "testArea", ResourceModels = null } }) };
			
			PluginResourcesProvider provider = new PluginResourcesProvider();
			provider.PluginManifests = manifests.ToArray();
			
			List<ResourceModel> retVal = new List<ResourceModel>();

			//Act
			retVal = provider.Get("testArea").ToList();

			//Assert
			Assert.NotNull(retVal);
			Assert.IsTrue(retVal.Count == 0);
		}
	}
}