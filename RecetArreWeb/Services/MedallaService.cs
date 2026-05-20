using System.Net.Http.Json;
using RecetArreWeb.DTOs;

namespace RecetArreWeb.Services
{
    public interface IMedallaService
    {
        Task<List<MedallaInfoDto>> ObtenerMisMedallas();
        Task<VerificarMedallasResponseDto?> VerificarMedallas();
    }

    public class MedallaService : IMedallaService
    {
        private readonly HttpClient httpClient;
        private const string endpoint = "api/Medallas";

        public MedallaService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<MedallaInfoDto>> ObtenerMisMedallas()
        {
            try
            {
                var medallas = await httpClient.GetFromJsonAsync<List<MedallaInfoDto>>($"{endpoint}/mis-medallas");
                return medallas ?? new List<MedallaInfoDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener medallas: {ex.Message}");
                return new List<MedallaInfoDto>();
            }
        }

        public async Task<VerificarMedallasResponseDto?> VerificarMedallas()
        {
            try
            {
                var response = await httpClient.PostAsync($"{endpoint}/verificar", null);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<VerificarMedallasResponseDto>();
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar medallas: {ex.Message}");
                return null;
            }
        }
    }
}
