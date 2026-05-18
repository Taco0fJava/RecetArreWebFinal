using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using RecetArreWeb;
using RecetArreWeb.Services;
using RecetArreWeb.Auth;
using RecetArreWeb.Handlers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurar HttpClient con handler de autorización
builder.Services.AddScoped<AuthorizationMessageHandler>();

builder.Services.AddScoped(sp => 
{
    var handler = sp.GetRequiredService<AuthorizationMessageHandler>();
    handler.InnerHandler = new HttpClientHandler();
    
    var httpClient = new HttpClient(handler)
    {
       //BaseAddress = new Uri("https://localhost:7019/")
       BaseAddress = new Uri("https://recetitas.somee.com/")
    };
    
    return httpClient;
});

// Registrar servicios
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IIngredienteService, IngredienteService>();
builder.Services.AddScoped<IRecetaService, RecetaService>();
builder.Services.AddScoped<IComentarioService, ComentarioService>();
builder.Services.AddScoped<IRankingService, RankingService>();

// Configurar autenticación
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

await builder.Build().RunAsync();
