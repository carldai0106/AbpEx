using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Dapper.Configuration
{
    public interface IAbpDapperModuleConfiguration
    {
        IDapperConfiguration DapperConfiguration { get; set; }
    }
}
