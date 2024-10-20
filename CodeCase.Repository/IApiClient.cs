using CodeCase.Domain.DTOs;

namespace CodeCase.Repository
{
    public interface IApiClient
    {
        Task<IEnumerable<DtoResponse>> GetDto(int dtoId);
    }
}
