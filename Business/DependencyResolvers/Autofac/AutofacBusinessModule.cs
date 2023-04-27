using Autofac;
using Business.Abstract;
using Business.Concrete;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;

namespace Business.DependencyResolvers.Autofac
{
    public class AutofacBusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AuthManager>().As<IAuthService>();
            builder.RegisterType<EfUserDal>().As<IUserDal>();

            builder.RegisterType<JwtHelper>().As<ITokenHelper>();

            builder.RegisterType<ClaimManager>().As<IClaimService>();
            builder.RegisterType<EfUserOperationDal>().As<IUserOperationDal>();

            builder.RegisterType<ParityManager>().As<IParityService>();

            builder.RegisterType<WalletManager>().As<IWalletService>();
            builder.RegisterType<EfWalletDal>().As<IWalletDal>();

            builder.RegisterType<CryptoManager>().As<ICryptoService>();
            builder.RegisterType<EfCryptoDal>().As<ICryptoDal>();

            builder.RegisterType<CompanyWalletManager>().As<ICompanyWalletService>();
            builder.RegisterType<EfCompanyWalletDal>().As<ICompanyWalletDal>();

            builder.RegisterType<OrderManager>().As<IOrderService>();
            builder.RegisterType<EfBuyOrderDal>().As<IBuyOrderDal>();
            builder.RegisterType<EfSellOrderDal>().As<ISellOrderDal>();




            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

        }
    }
}
