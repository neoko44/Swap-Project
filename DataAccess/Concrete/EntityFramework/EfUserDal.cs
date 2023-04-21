using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfUserDal : EfEntityRepositoryBase<User, SwapProjectDbContext>, IUserDal
    {

        public List<Operation> GetClaims(User user)
        {
            using (var context = new SwapProjectDbContext())
            {
                var result = from Operation in context.Operations
                             join UserOperation in context.UserOperations
                             on Operation.Id equals UserOperation.OperationId
                             where UserOperation.UserId == user.Id
                             select new Operation { Id = Operation.Id, Name = Operation.Name };
                return result.ToList();
            }
        }
    }
}
