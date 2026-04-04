
using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services;

public class SponsorService : ISponsorService
{
    private readonly ISponsorRepository _sponsorRepository;
    private readonly ITournamentSponsorRepository _tournamentSponsorRepository;
    private readonly ILogger<SponsorService> _logger;

    public SponsorService(
        ISponsorRepository sponsorRepository,
        ITournamentSponsorRepository tournamentSponsorRepository,
        ILogger<SponsorService> logger)
    {
        _sponsorRepository = sponsorRepository;
        _tournamentSponsorRepository = tournamentSponsorRepository;
        _logger = logger;
    }

    public async Task<Sponsor> CreateAsync(Sponsor sponsor)
    {
        var existing = await _sponsorRepository.GetByNameAsync(sponsor.Name);
        if (existing.Any())
            throw new InvalidOperationException("El nombre del patrocinador ya existe.");

        sponsor.CreatedAt = DateTime.UtcNow;
        await _sponsorRepository.CreateAsync(sponsor);
        return sponsor;
    }

    public async Task<IEnumerable<Sponsor>> GetAllAsync()
    {
        return await _sponsorRepository.GetAllAsync();
    }

    public async Task<Sponsor?> GetByIdAsync(int id)
    {
        return await _sponsorRepository.GetByIdAsync(id);
    }

    public async Task UpdateAsync(int id, Sponsor sponsor)
    {
        var existing = await _sponsorRepository.GetByIdAsync(id);
        if (existing == null)
            throw new KeyNotFoundException("Patrocinador no encontrado.");

        existing.Name = sponsor.Name;
        existing.ContactEmail = sponsor.ContactEmail;
        existing.Phone = sponsor.Phone;
        existing.WebsiteUrl = sponsor.WebsiteUrl;
        existing.Category = sponsor.Category;
        existing.UpdatedAt = DateTime.UtcNow;

        await _sponsorRepository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        var sponsor = await _sponsorRepository.GetByIdAsync(id);

        if (sponsor == null)
            throw new KeyNotFoundException("Patrocinador no encontrada.");

        await _sponsorRepository.DeleteAsync(id);
    }


    public async Task<TournamentSponsor> AddSponsorToTournamentAsync(
        int sponsorId,
        int tournamentId,
        decimal contractAmount)
    {
        if (contractAmount <= 0)
            throw new InvalidOperationException("El importe del contrato debe ser mayor que cero..");

        var existing = await _tournamentSponsorRepository
            .GetByTournamentAndSponsorAsync(tournamentId, sponsorId);

        if (existing != null)
            throw new InvalidOperationException("Patrocinador ya vinculado al torneo.");

        var entity = new TournamentSponsor
        {
            SponsorId = sponsorId,
            TournamentId = tournamentId,
            ContractAmount = contractAmount,
            JoinedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await _tournamentSponsorRepository.CreateAsync(entity);
        return entity;
    }

    public async Task<IEnumerable<TournamentSponsor>> GetTournamentsAsync(int sponsorId)
    {
        return await _tournamentSponsorRepository.GetBySponsorIdAsync(sponsorId);
    }
}

