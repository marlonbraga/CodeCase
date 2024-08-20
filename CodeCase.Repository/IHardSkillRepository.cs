using CodeCase.Domain;

namespace CodeCase.Repository
{
    public interface IHardSkillRepository
    {
        public IEnumerable<HardSkill> GetAll();
    }
}
