using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CDMS.Entity;
using CDMS.Utility;

namespace CDMS.Data
{
    public interface IMenuDataSourceRepository : IRepository<MenuDataSource>
    {

    }

    internal class MenuDataSourceRepository : RepositoryBase<MenuDataSource>, IMenuDataSourceRepository
    {

    }
}
