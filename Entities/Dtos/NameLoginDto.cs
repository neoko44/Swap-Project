using Core.Entities.Abstract;

namespace Entities.Dtos
{
    public class NameLoginDto : IDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
