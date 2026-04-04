using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Services
{
    public interface ISponsorService
    {
        Task<Sponsor> CreateAsync(Sponsor sponsor);
        Task<IEnumerable<Sponsor>> GetAllAsync();
        Task<Sponsor?> GetByIdAsync(int id);
        Task UpdateAsync(int id, Sponsor sponsor);
        Task DeleteAsync(int id);
        Task<TournamentSponsor> AddSponsorToTournamentAsync(int sponsorId, int tournamentId, decimal contractAmount);
        Task<IEnumerable<TournamentSponsor>> GetTournamentsAsync(int sponsorId);
        Task RemoveSponsorFromTournamentAsync(int sponsorId, int tournamentId); // Se agrega nuevo método para poder desvincular un sponsor de un torneo
    }
}
