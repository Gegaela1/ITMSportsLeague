using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers;

// Todos los endpoints de alineación están anidados bajo /api/match/{matchId}/lineup
[ApiController]
[Route("api/match/{matchId}/lineup")]
public class MatchLineupController : ControllerBase
{
    private readonly IMatchLineupService _lineupService;
    private readonly IMapper _mapper;
    private readonly ILogger<MatchLineupController> _logger;

    public MatchLineupController(
        IMatchLineupService lineupService,
        IMapper mapper,
        ILogger<MatchLineupController> logger)
    {
        _lineupService = lineupService;
        _mapper = mapper;
        _logger = logger;
    }

    // POST /api/match/{matchId}/lineup
    // Agrega un jugador a la alineación tras pasar todas las validaciones de negocio
    [HttpPost]
    [ProducesResponseType(typeof(MatchLineupDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MatchLineupDto>> AddToLineup(
        int matchId,
        [FromBody] CreateMatchLineupDto dto)
    {
        try
        {
            // Mapea el DTO a la entidad (el MatchId viene de la ruta, no del cuerpo)
            var lineup = _mapper.Map<MatchLineup>(dto);

            var created = await _lineupService.AddToLineupAsync(matchId, lineup);

            // Mapea la entidad creada al DTO de respuesta con datos de navegación
            var response = _mapper.Map<MatchLineupDto>(created);
            return CreatedAtAction(
                nameof(GetByMatch),
                new { matchId },
                response);
        }
        catch (InvalidOperationException ex)
        {
            // Falló una validación de negocio
            return Conflict(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // GET /api/match/{matchId}/lineup
    // Retorna la alineación completa de un partido
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MatchLineupDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MatchLineupDto>>> GetByMatch(int matchId)
    {
        var lineups = await _lineupService.GetByMatchAsync(matchId);
        var response = _mapper.Map<IEnumerable<MatchLineupDto>>(lineups);
        return Ok(response);
    }

    // GET /api/match/{matchId}/lineup/team/{teamId}
    // Retorna la alineación filtrada por equipo
    [HttpGet("team/{teamId}")]
    [ProducesResponseType(typeof(IEnumerable<MatchLineupDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MatchLineupDto>>> GetByMatchAndTeam(
        int matchId,
        int teamId)
    {
        var lineups = await _lineupService.GetByMatchAndTeamAsync(matchId, teamId);
        var response = _mapper.Map<IEnumerable<MatchLineupDto>>(lineups);
        return Ok(response);
    }

    // DELETE /api/match/{matchId}/lineup/{id}
    // Elimina un jugador de la alineación del partido
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int matchId, int id)
    {
        try
        {
            await _lineupService.DeleteAsync(matchId, id);
            return NoContent();  // 204 - Eliminado correctamente
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
