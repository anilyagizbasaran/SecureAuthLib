using SecureAuthLib.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureAuthLib.DataAccess.Concrete
{
    public class NorthwindContext:DbContext
    {
        public DbSet<User> Users { get; set; }
    }
}
