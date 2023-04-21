using Business.Abstract;
using Business.Constants;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;

namespace Business.Concrete
{
    public class ClaimManager : IClaimService
    {
        private IUserOperationDal _roleclaim;

        public ClaimManager(IUserOperationDal roleClaim)
        {
            _roleclaim = roleClaim;
        }

        public IResult Add(UserOperation userOperationClaim)
        {
            _roleclaim.Add(userOperationClaim);
            return new SuccessDataResult<UserOperation>(Messages.RoleAdded);
        }

        public void Update(UserOperation userOperationClaim)
        {
            _roleclaim.Update(userOperationClaim);
        }
    }
}
