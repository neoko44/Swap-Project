using Core.Entities.Concrete;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Constants
{
    public class Messages
    {
        public static string UserNameNull = "Kullanıcı adı boş";
        public static string PasswordNull = "Şifre boş";
        public static string EmailNull = "Email boş";
        public static string FirstNameNull = "Adınız boş";
        public static string LastNameNull = "Soyadınız boş";
        public static string MailExists = "Mail sistemde mevcut";
        public static string UserNameExists = "Kullanıcı adı sistemde mevcut";
        public static string UserRegistered = "Kullanıcı kayıt oldu";
        public static string AccessTokenCreated = "Access token üretildi";
        public static string RoleAdded = "Rol eklendi";

        public static string AccessTokenError = "Token Hatası";
        public static string AccessTokenNotFound = "Token Bulunamadı";
        public static string LibraryCreated = "Kitaplık Üretildi";
        public static string NewPassMustDifferent = "Yeni şifreniz eskisiyle aynı olamaz";
        public static string PassChangeSuccess = "Şifre değiştirme başarılı";
        public static string PasswordNotMatch = "Şifreler Eşleşmiyor";
        public static string UserNotFound = "Kullanıcı Bulunamadı";
        public static string PasswordError = "Şifre Hatalı";
        public static string SuccessfulLogin = "Giriş Başarılı";

        public static string Failed = "Hata";
        public static string SuccessfullyUpdated = "Başarıyla Güncellendi";

        public static string PhoneNull = "Telefon numarası boş olamaz";
        public static string PhoneExists = "Telefon numarası sistemde mevcut";

        public static string BalanceInvalid="Tutar Geçersiz";
        public static string MoneyAdded = "Tutar hesabınıza eklendi";
        public static string BalanceUpdated = "Tutar güncellendi";

        public static string ParityNotFound = "Parite bulunamadı";

        public static string CryptoNotFound = "Kripto bulunamadı";

        public static string IdInvalid = "Id değeri geçersiz";

        public static string WalletNotFound = "Cüzdan bulunamadı";

        public static string NotEnoughBalance = "Yetersiz bakiye";

        public static string InvalidValue = "Geçersiz değer";

        public static string ValidValue = "Değer geçerli";

        public static BuyOrder BuyOrdered { get; internal set; }
    }
}
