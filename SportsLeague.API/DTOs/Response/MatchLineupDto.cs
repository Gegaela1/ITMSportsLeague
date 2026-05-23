namespace SportsLeague.API.DTOs.Response;

// Datos que se retornan al cliente al consultar una alineación
public class MatchLineupDto
{
    public int Id { get; set; }
    public int MatchId { get; set; }
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public bool IsStarter { get; set; }
    // Posición asignada para este partido
    public string Position { get; set; } = string.Empty;
}