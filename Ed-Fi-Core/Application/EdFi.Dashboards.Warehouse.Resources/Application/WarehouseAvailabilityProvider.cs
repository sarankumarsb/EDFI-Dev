using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Warehouse.Data.Entities;
using log4net;

namespace EdFi.Dashboards.Warehouse.Resources.Application
{
    public interface IWarehouseAvailabilityProvider
    {
        bool Get();
    }

    public class WarehouseAvailabilityProvider : IWarehouseAvailabilityProvider
    {
        private readonly IRepository<LocalEducationAgencyMetricInstance> testWarehouseRepository;
        private readonly IConfigValueProvider configValueProvider;
        private readonly ILog logger = LogManager.GetLogger(typeof (WarehouseAvailabilityProvider));

        private static object lockThis = new object();
        private static bool? warehouseAvailable;

        public WarehouseAvailabilityProvider(IRepository<LocalEducationAgencyMetricInstance> testWarehouseRepository, IConfigValueProvider configValueProvider)
        {
            this.testWarehouseRepository = testWarehouseRepository;
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
                if (!warehouseAvailable.Value)
                    return warehouseAvailable.Value;


                // see if database is available
                try
                {
                    var result = (from t in testWarehouseRepository.GetAll()
                                  where 1 == 2
                                  select t).ToList();
                    if (result != null)
                        logger.Info("Data warehouse connection successfully tested");
                    warehouseAvailable = true;
                }
                catch (Exception e)
                {
                    logger.Error("Exception generated while testing data warehouse connection.", e);
                    warehouseAvailable = false;
                }
                return warehouseAvailable.Value;
            }
        }
    }
}
