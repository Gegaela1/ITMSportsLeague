namespace SportsLeague.API.DTOs.Request;

// Datos que el cliente envía al agregar un jugador a la alineación
public class CreateMatchLineupDto
{
    // El ID del jugador a agregar
    public int PlayerId { get; set; }

    // true = Titular, false = Suplente
    public bool IsStarter { get; set; }

    // Posición asignada para este partido: "GK", "CB", "LB", "RB", "CDM", "CM", "CAM", "LW", "RW", "ST"
    public string Position { get; set; } = string.Empty;
}
