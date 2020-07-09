using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Configurations
{
    public interface IDatabaseSettings
    {
        string ConnectionString { get; set; }
        string DataBaseName { get; set; }
    }
}
