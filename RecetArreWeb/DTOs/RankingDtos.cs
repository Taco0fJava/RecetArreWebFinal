using System.ComponentModel.DataAnnotations;

namespace RecetArreWeb.DTOs
{
    public class RankingDto
    {
        public int Id { get; set; }
        public int Valor { get; set; }
        public int RecetaId { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime? ModificadoUtc { get; set; }
    }

    public class RankingCreacionDto
    {
        [Required]
        public int RecetaId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Valor { get; set; }
    }

    public class RankingModificacionDto
    {
        [Required]
        [Range(1, 5)]
        public int Valor { get; set; }
    }

    public class MedallaDto
    {
        public int RecetaId { get; set; }
        public double PromedioEstrellas { get; set; }
        public int TotalVotos { get; set; }
        public string Medalla { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}
