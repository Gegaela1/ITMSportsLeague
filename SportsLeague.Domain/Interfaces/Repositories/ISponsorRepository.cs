
using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories;

public interface ISponsorRepository
    : IGenericRepository<Sponsor>
{
    Task<IEnumerable<Sponsor>> GetByNameAsync(string name);
}

