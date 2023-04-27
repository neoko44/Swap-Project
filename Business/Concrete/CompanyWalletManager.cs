using Business.Abstract;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class CompanyWalletManager : ICompanyWalletService
    {
        ICompanyWalletDal _companyWalletDal;

        public CompanyWalletManager(ICompanyWalletDal companyWalletDal)
        {
            _companyWalletDal = companyWalletDal;
        }

        public void Add(CompanyWallet companyWallet)
        {
            _companyWalletDal.Add(companyWallet);
        }

        public IDataResult<CompanyWallet> GetById(int cryptoId)
        {
            var result = _companyWalletDal.Get(cw => cw.CryptoId == cryptoId);
            if (result == null)
            {
                CompanyWallet companyWallet = new()
                {
                    CryptoId = cryptoId,
                    Total = 0
                };

                _companyWalletDal.Add(companyWallet);
                result = companyWallet;
                return new SuccessDataResult<CompanyWallet>(result);
            }

            return new SuccessDataResult<CompanyWallet>(result);
        }

        public void Update(CompanyWallet companyWallet)
        {
            _companyWalletDal.Update(companyWallet);
        }
    }
}
