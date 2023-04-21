using Core.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class TokenInfoDto : IDto
    {
        public string NameSurname { get; set; }
        public string Id { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
    }
}
