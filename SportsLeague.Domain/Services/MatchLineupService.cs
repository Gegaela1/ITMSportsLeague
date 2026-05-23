using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services;

public class MatchLineupService : IMatchLineupService
{
    private readonly IMatchLineupRepository _lineupRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly ILogger<MatchLineupService> _logger;

    public MatchLineupService(
        IMatchLineupRepository lineupRepository,
        IMatchRepository matchRepository,
        IPlayerRepository playerRepository,
        ILogger<MatchLineupService> logger)
    {
        _lineupRepository = lineupRepository;
        _matchRepository = matchRepository;
        _playerRepository = playerRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId)
    {
        return await _lineupRepository.GetByMatchAsync(matchId);
    }

    public async Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId)
    {
        return await _lineupRepository.GetByMatchAndTeamAsync(matchId, teamId);
    }

    // agregar judador

    public async Task<MatchLineup> AddToLineupAsync(int matchId, MatchLineup lineup)
    {
        // El partido debe existir
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
        {
            _logger.LogWarning("Partido {MatchId} no encontrado al agregar alineación", matchId);
            throw new InvalidOperationException($"No se encontró el partido con ID {matchId}");
        }

        // El jugador debe existir
        var player = await _playerRepository.GetByIdAsync(lineup.PlayerId);
        if (player == null)
        {
            _logger.LogWarning("Jugador {PlayerId} no encontrado", lineup.PlayerId);
            throw new InvalidOperationException($"No se encontró el jugador con ID {lineup.PlayerId}");
        }

        // El jugador debe pertenecer al HomeTeam o al AwayTeam del partido
        bool playerBelongsToMatch = player.TeamId == match.HomeTeamId ||
                                    player.TeamId == match.AwayTeamId;
        if (!playerBelongsToMatch)
        {
            _logger.LogWarning(
                "Jugador {PlayerId} (TeamId={TeamId}) no pertenece a los equipos del partido (Local={Home}, Visitante={Away})",
                lineup.PlayerId, player.TeamId, match.HomeTeamId, match.AwayTeamId);
            throw new InvalidOperationException(
                "El jugador no pertenece a ninguno de los equipos del partido");
        }

        // El jugador no puede aparecer dos veces en la misma alineación
        bool alreadyRegistered = await _lineupRepository
            .ExistsByMatchAndPlayerAsync(matchId, lineup.PlayerId);
        if (alreadyRegistered)
        {
            _logger.LogWarning("Jugador {PlayerId} ya registrado en la alineación del partido {MatchId}",
                lineup.PlayerId, matchId);
            throw new InvalidOperationException(
                "El jugador ya está registrado en la alineación de este partido");
        }

        //Máximo 11 titulares por equipo por partido (solo aplica si IsStarter = true)
        if (lineup.IsStarter)
        {
            int starterCount = await _lineupRepository
                .CountStartersByMatchAndTeamAsync(matchId, player.TeamId);

            if (starterCount >= 11)
            {
                _logger.LogWarning(
                    "El equipo {TeamId} ya tiene 11 titulares en el partido {MatchId}",
                    player.TeamId, matchId);
                throw new InvalidOperationException(
                    "El equipo ya tiene 11 titulares registrados en este partido");
            }
        }

        // El partido debe estar en estado Scheduled
        if (match.Status != MatchStatus.Scheduled)
        {
            _logger.LogWarning("El partido {MatchId} está en estado {Status}, no se puede agregar alineación",
                matchId, match.Status);
            throw new InvalidOperationException(
                "Solo se pueden registrar alineaciones en partidos Scheduled");
        }

        lineup.MatchId = matchId;
        return await _lineupRepository.CreateAsync(lineup);
    }

    // Eliminar entrada alineación

    public async Task DeleteAsync(int matchId, int lineupId)
    {
        // Verificar que la entrada existe y pertenece a este partido
        var exists = await _lineupRepository.ExistsAsync(lineupId);
        if (!exists)
        {
            throw new KeyNotFoundException(
                $"No se encontró la alineación con ID {lineupId}");
        }

        await _lineupRepository.DeleteAsync(lineupId);
    }
}
