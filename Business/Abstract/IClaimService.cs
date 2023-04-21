using Core.Entities.Concrete;
using Core.Utilities.Results;

namespace Business.Abstract
{
    public interface IClaimService
    {
        IResult Add(UserOperation userOperation);
        void Update (UserOperation userOperation);
    }
}
