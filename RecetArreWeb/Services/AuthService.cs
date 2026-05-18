using System.Net.Http.Json;
using RecetArreWeb.DTOs;

namespace RecetArreWeb.Services
{
    public interface IAuthService
    {
        Task<RespuestaAutenticacion?> Login(CredencialesUsuario credenciales);
        Task<RespuestaAutenticacion?> Registrar(CredencialesUsuario credenciales);
        Task<RespuestaAutenticacion?> RenovarToken();
        Task Logout();
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient httpClient;
        private readonly ITokenService tokenService;
        private const string endpoint = "api/Cuentas";

        public AuthService(HttpClient httpClient, ITokenService tokenService)
        {
            this.httpClient = httpClient;
            this.tokenService = tokenService;
        }

        public async Task<RespuestaAutenticacion?> Login(CredencialesUsuario credenciales)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync($"{endpoint}/Login", credenciales);

                if (response.IsSuccessStatusCode)
                {
                    var respuesta = await response.Content.ReadFromJsonAsync<RespuestaAutenticacion>();
                    
                    if (respuesta != null)
                    {
                        await tokenService.GuardarToken(respuesta.Token, respuesta.Expiracion);
                        return respuesta;
                    }
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error en login: {error}");
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al hacer login: {ex.Message}");
                return null;
            }
        }

        public async Task<RespuestaAutenticacion?> Registrar(CredencialesUsuario credenciales)
        {
            var response = await httpClient.PostAsJsonAsync($"{endpoint}/registrar", credenciales);

            if (response.IsSuccessStatusCode)
            {
                var respuesta = await response.Content.ReadFromJsonAsync<RespuestaAutenticacion>();
                if (respuesta != null)
                {
                    await tokenService.GuardarToken(respuesta.Token, respuesta.Expiracion);
                    return respuesta;
                }
            }

            // Leer y propagar el error real que devuelve la API
            var errorJson = await response.Content.ReadAsStringAsync();
            var mensaje = ExtraerMensajeError(errorJson);
            throw new Exception(mensaje);
        }

        // Extrae las descripciones del array de IdentityError que devuelve la API
        private static string ExtraerMensajeError(string json)
        {
            try
            {
                var errores = System.Text.Json.JsonSerializer.Deserialize<List<IdentityErrorDto>>(json,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (errores != null && errores.Count > 0)
                    return string.Join(" | ", errores.Select(e => TraducirError(e.Description ?? e.Code ?? "Error desconocido")));
            }
            catch { /* si no es JSON de Identity, usar el texto plano */ }

            return string.IsNullOrWhiteSpace(json) ? "Error desconocido al crear la cuenta" : json;
        }

        // Traduce los mensajes de Identity al español
        private static string TraducirError(string mensaje) => mensaje switch
        {
            var m when m.Contains("already taken")          => "El email ya está registrado",
            var m when m.Contains("non alphanumeric")       => "La contraseña debe tener al menos un carácter especial (!@#...)",
            var m when m.Contains("digit")                  => "La contraseña debe contener al menos un número",
            var m when m.Contains("uppercase")              => "La contraseña debe tener al menos una mayúscula",
            var m when m.Contains("lowercase")              => "La contraseña debe tener al menos una minúscula",
            var m when m.Contains("least") && m.Contains("characters") => "La contraseña debe tener al menos 6 caracteres",
            var m when m.Contains("Invalid email")          => "El formato del email no es válido",
            _ => mensaje
        };

        private class IdentityErrorDto
        {
            public string? Code { get; set; }
            public string? Description { get; set; }
        }

        public async Task<RespuestaAutenticacion?> RenovarToken()
        {
            try
            {
                var token = await tokenService.ObtenerToken();
                
                if (string.IsNullOrEmpty(token))
                    return null;

                // Agregar el token actual al header
                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.GetAsync($"{endpoint}/RenovarToken");

                if (response.IsSuccessStatusCode)
                {
                    var respuesta = await response.Content.ReadFromJsonAsync<RespuestaAutenticacion>();
                    
                    if (respuesta != null)
                    {
                        await tokenService.GuardarToken(respuesta.Token, respuesta.Expiracion);
                        return respuesta;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al renovar token: {ex.Message}");
                return null;
            }
        }

        public async Task Logout()
        {
            await tokenService.EliminarToken();
        }
    }
}
