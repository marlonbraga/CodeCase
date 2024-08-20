using CodeCase.Domain;
using CodeCase.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Case.TestContainers.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HardSkillController : ControllerBase
    {
        private IHardSkillRepository _hardSkillRepository;

        public HardSkillController(IHardSkillRepository hardSkillRepository)
        {
            _hardSkillRepository = hardSkillRepository;
        }  


        [HttpGet(Name = "GetAllHardSkills")]
        public IEnumerable<HardSkill> GetAll()
        {
            return _hardSkillRepository.GetAll();
        }
    }
}
