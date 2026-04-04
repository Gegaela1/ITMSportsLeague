using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;

namespace SportsLeague.Domain.Interfaces.Repositories;
{
public interface ITournamentSponsorRepository
{
    Task<IEnumerable<TournamentSponsor>> GetBySponsorIdAsync(int sponsorId);
    Task<TournamentSponsor?> GetByTournamentAndSponsorAsync(int tournamentId, int sponsorId);
    Task<TournamentSponsor?> GetByIdWithDetailsAsync(int id);
}



