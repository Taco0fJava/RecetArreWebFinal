using System.Net.Http.Json;
using RecetArreWeb.DTOs;

namespace RecetArreWeb.Services
{
    public interface IRankingService
    {
        Task<List<RankingDto>> ObtenerPorReceta(int recetaId);
        Task<MedallaDto?> ObtenerMedalla(int recetaId);
        Task<RankingDto?> ObtenerMiRanking(int recetaId);
        Task<RankingDto?> Crear(RankingCreacionDto dto);
        Task<bool> Actualizar(int id, RankingModificacionDto dto);
        Task<bool> Eliminar(int id);
    }

    public class RankingService : IRankingService
    {
        private readonly HttpClient httpClient;
        private const string endpoint = "api/Ranking";

        public RankingService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<RankingDto>> ObtenerPorReceta(int recetaId)
        {
            try
            {
                var lista = await httpClient.GetFromJsonAsync<List<RankingDto>>($"{endpoint}/receta/{recetaId}");
                return lista ?? new List<RankingDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener rankings: {ex.Message}");
                return new List<RankingDto>();
            }
        }

        public async Task<MedallaDto?> ObtenerMedalla(int recetaId)
        {
            try
            {
                return await httpClient.GetFromJsonAsync<MedallaDto>($"{endpoint}/receta/{recetaId}/medalla");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener medalla: {ex.Message}");
                return null;
            }
        }

        public async Task<RankingDto?> ObtenerMiRanking(int recetaId)
        {
            try
            {
                var response = await httpClient.GetAsync($"{endpoint}/receta/{recetaId}/mio");
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<RankingDto>();
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener mi ranking: {ex.Message}");
                return null;
            }
        }

        public async Task<RankingDto?> Crear(RankingCreacionDto dto)
        {
            var response = await httpClient.PostAsJsonAsync(endpoint, dto);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<RankingDto>();

            // Propaga el error real con el código HTTP para poder diagnosticar
            var cuerpo = await response.Content.ReadAsStringAsync();
            var codigo = (int)response.StatusCode;
            throw new Exception(codigo switch
            {
                401 => "No estás autenticado. Vuelve a iniciar sesión",
                403 => "No tienes permiso para realizar esta acción",
                409 => "Ya calificaste esta receta. Usa las estrellas para cambiarla",
                404 => $"Endpoint no encontrado (404) — el API puede necesitar reiniciarse",
                500 => $"Error interno del servidor (500): {cuerpo}",
                _   => $"Error {codigo}: {cuerpo}"
            });
        }

        public async Task<bool> Actualizar(int id, RankingModificacionDto dto)
        {
            try
            {
                var response = await httpClient.PutAsJsonAsync($"{endpoint}/{id}", dto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar ranking {id}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            try
            {
                var response = await httpClient.DeleteAsync($"{endpoint}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar ranking {id}: {ex.Message}");
                return false;
            }
        }
    }
}
