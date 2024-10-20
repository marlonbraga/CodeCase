using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCase.Domain.Entities
{
    public class UserData
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public Guid Token { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
    }
}
