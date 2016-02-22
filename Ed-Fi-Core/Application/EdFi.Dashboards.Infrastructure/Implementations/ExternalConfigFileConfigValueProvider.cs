using System.Configuration;

namespace EdFi.Dashboards.Infrastructure.Implementations
{
    /// <summary>
    /// Provides access to config values from a specific external configuration file.
    /// </summary>
    public class ExternalConfigFileConfigValueProvider : IConfigValueProvider
    {
        private readonly ExternalConfigFileSectionProvider configFileSectionProvider;

        /// <summary>
        /// Creates and initializes an instance of the <see cref="ExternalConfigFileConfigValueProvider"/> using 
        /// the supplied <see cref="ExternalConfigFileSectionProvider"/> instance.
        /// </summary>
        /// <param name="configFileSectionProvider">The previously initialized config section provider that provides
        /// access to a specific external configuration file by path.</param>
        public ExternalConfigFileConfigValueProvider(ExternalConfigFileSectionProvider configFileSectionProvider)
        {
            this.configFileSectionProvider = configFileSectionProvider;
        }

        /// <summary>
        /// Gets the specified appSettings value by name.
        /// </summary>
        /// <param name="name">The name of the appSettings value to be retrieved.</param>
        /// <returns>The value of appSettings entry.</returns>
        public string GetValue(string name)
        {
            var appSettingsSection = configFileSectionProvider.GetSection("appSettings") as AppSettingsSection;
            
            return appSettingsSection.Settings[name].Value;
        }
    }
}