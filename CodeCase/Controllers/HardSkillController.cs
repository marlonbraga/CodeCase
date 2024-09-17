using CodeCase.Domain;
using CodeCase.Domain.Interfaces;
using CodeCase.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Case.TestContainers.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HardSkillController : ControllerBase
    {
        private IHardSkillRepository _hardSkillRepository;
        private IHandleHardSkillService _handleHardSkillService;

        public HardSkillController(
            IHardSkillRepository hardSkillRepository,
            IHandleHardSkillService handleHardSkillService)
        {
            _hardSkillRepository = hardSkillRepository;
            _handleHardSkillService = handleHardSkillService;
        }
        
        [HttpGet(Name = "GetAllHardSkills")]
        public IEnumerable<HardSkill> GetAll()
        {
            return _hardSkillRepository.GetAll();
        }

        [HttpGet(Name = "GetAllHardSkills")]
        public void SendHardSkill(string message)
        {
            _handleHardSkillService.SendHardSkillToQueue(message);
        }
    }
}
