using System;
using EdFi.Dashboards.Infrastructure;

namespace EdFi.Dashboards.Resources.Application
{
    public interface IWarehouseAvailabilityProviderResource
    {
        bool Get();
    }

    public class WarehouseAvailabilityProviderResource : IWarehouseAvailabilityProviderResource
    {

        private readonly IConfigValueProvider configValueProvider;

        private static object lockThis = new object();
        private static bool? warehouseAvailable;

        public WarehouseAvailabilityProviderResource(IConfigValueProvider configValueProvider)
        {
            this.configValueProvider = configValueProvider;
        }

        public bool Get()
        {
            if (warehouseAvailable.HasValue)
                return warehouseAvailable.Value;

            lock (lockThis)
            {
                // maybe it got a value while we were waiting
                if (warehouseAvailable.HasValue)
                    return warehouseAvailable.Value;

                // see if app config setting has turned off warehouse
                try
                {
                    var configValue = configValueProvider.GetValue("LoadFromWarehouse");
                    warehouseAvailable = String.IsNullOrWhiteSpace(configValue) || Convert.ToBoolean(configValue);
                }
                catch
                {
                    warehouseAvailable = true;
                }

                return warehouseAvailable.Value;

            }
        }
    }
}
