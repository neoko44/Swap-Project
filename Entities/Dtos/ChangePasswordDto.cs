using Core.Entities.Abstract;

namespace Entities.Dtos
{
    public class ChangePasswordDto : IDto
    {
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        //public string Token { get; set; }
    }
}
