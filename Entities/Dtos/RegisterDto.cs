using Core.Entities.Abstract;

namespace Entities.Dtos
{
    public class RegisterDto : IDto
    {
        public string Password { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }


    }
}
