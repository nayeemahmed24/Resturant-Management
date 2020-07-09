using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Configurations
{
    public class DatabaseSettings:IDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DataBaseName { get; set; }
    }

}
