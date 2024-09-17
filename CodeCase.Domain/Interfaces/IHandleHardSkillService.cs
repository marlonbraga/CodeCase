using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCase.Domain.Interfaces
{
    public interface IHandleHardSkillService
    {
        void SendHardSkillToQueue(string message = "Hello World!");
    }
}
