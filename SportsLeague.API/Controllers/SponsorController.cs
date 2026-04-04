
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SponsorController : ControllerBase
{
    private readonly ISponsorService _sponsorService;
    private readonly IMapper _mapper;

    public SponsorController(
        ISponsorService sponsorService,
        IMapper mapper)
    {
        _sponsorService = sponsorService;
        _mapper = mapper;
    }

    // GET: api/Sponsor
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SponsorResponseDTO>>> GetAll()
    {
        var sponsors = await _sponsorService.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<SponsorResponseDTO>>(sponsors));
    }

    // GET: api/Sponsor/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<SponsorResponseDTO>> GetById(int id)
    {
        var sponsor = await _sponsorService.GetByIdAsync(id);
        if (sponsor == null)
            return NotFound();

        return Ok(_mapper.Map<SponsorResponseDTO>(sponsor));
    }

    // POST: api/Sponsor
    [HttpPost]
    public async Task<ActionResult<SponsorResponseDTO>> Create(
        [FromBody] SponsorRequestDTO request)
    {
        var sponsor = _mapper.Map<Sponsor>(request);
        var created = await _sponsorService.CreateAsync(sponsor);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            _mapper.Map<SponsorResponseDTO>(created));
    }

    // PUT: api/Sponsor/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] SponsorRequestDTO request)
    {
        var sponsor = _mapper.Map<Sponsor>(request);
        await _sponsorService.UpdateAsync(id, sponsor);
        return NoContent();
    }

    // DELETE: api/Sponsor/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _sponsorService.DeleteAsync(id);
        return NoContent();
    }
    // GET: api/Sponsor/{id}/tournaments

    [HttpGet("{id}/tournaments")]
    public async Task<ActionResult<IEnumerable<TournamentSponsorResponseDTO>>> GetTournaments(
            int id)
    {
        var tournaments = await _sponsorService.GetTournamentsAsync(id);

        var response = tournaments.Select(ts => new TournamentSponsorResponseDTO
        {
            TournamentId = ts.TournamentId,
            TournamentName = ts.Tournament.Name,
            ContractAmount = ts.ContractAmount,
            JoinedAt = ts.JoinedAt
        });

        return Ok(response);
    }
    // POST: api/Sponsor/{id}/tournaments

    [HttpPost("{id}/tournaments")]
    public async Task<ActionResult<TournamentSponsorResponseDTO>> AddSponsorToTournament(
            int id,
            [FromBody] TournamentSponsorRequestDTO request)
    {
        var result = await _sponsorService.AddSponsorToTournamentAsync(
            id,
            request.TournamentId,
            request.ContractAmount);

        var response = new TournamentSponsorResponseDTO
        {
            TournamentId = result.TournamentId,
            TournamentName = result.Tournament.Name,
            ContractAmount = result.ContractAmount,
            JoinedAt = result.JoinedAt
        };

        return Created(string.Empty, response);
    }
    // DELETE: api/Sponsor/{id}/tournaments/{tid}

    [HttpDelete("{id}/tournaments/{tid}")]
    public async Task<IActionResult> RemoveSponsorFromTournament(
            int id,
            int tid)
    {
        await _sponsorService.RemoveSponsorFromTournamentAsync(id, tid);
        return NoContent();
    }
}
