using SecureAuthLib.DataAccess.Abstract;
using SecureAuthLib.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureAuthLib.DataAccess.Concrete
{
    internal class EfUserDal : EfEntityRepositoryBase<User, NorthwindContext>, IUserDal
    {
       
    }
}
