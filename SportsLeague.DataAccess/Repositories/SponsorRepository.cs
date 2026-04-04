
using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;

namespace SportsLeague.DataAccess.Repositories
{

    public class SponsorRepository
        : GenericRepository<Sponsor>, ISponsorRepository
    {
        public SponsorRepository(LeagueDbContext context)
            : base(context) { }

        public async Task<IEnumerable<Sponsor>> GetByNameAsync(string name)
        {
            return await _dbSet
                .Where(s => s.Name.ToLower() == name.ToLower())
                .ToListAsync();
        }
    }
}



