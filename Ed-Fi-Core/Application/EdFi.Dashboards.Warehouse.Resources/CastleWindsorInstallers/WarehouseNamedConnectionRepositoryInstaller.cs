using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Resources.CastleWindsorInstallers;

namespace EdFi.Dashboards.Warehouse.Resources.CastleWindsorInstallers
{
    public class WarehouseNamedConnectionRepositoryInstaller<T> : NamedConnectionRepositoryInstaller<T>
    {
        public WarehouseNamedConnectionRepositoryInstaller()
        {
            DatabaseDataProvider = "DataWarehouseIDataProvider";
        }
    }
}
