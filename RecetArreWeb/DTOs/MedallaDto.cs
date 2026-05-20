namespace RecetArreWeb.DTOs
{
    public class MedallaInfoDto
    {
        public string Tipo { get; set; } = string.Empty;
        public string Icono { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime? FechaObtencion { get; set; }
        public bool Obtenida { get; set; }
    }

    public class VerificarMedallasResponseDto
    {
        public List<MedallaInfoDto> NuevasMedallas { get; set; } = new();
        public List<MedallaInfoDto> TodasMedallas { get; set; } = new();
    }
}
