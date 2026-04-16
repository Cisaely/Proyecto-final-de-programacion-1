using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NexBusiness.Data;
using NexBusiness.Interfaces;
using NexBusiness.Models;

namespace NexBusiness.Services
{
    public interface IChatService
    {
        Task<string> ProcesarMensajeIA(string sessionId, Negocio negocio, string mensajeUsuario);
    }

    public class ChatService : IChatService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly AppDbContext _db;
        private const string GeminiApiKey = "AIzaSyDdgmhvsWwupXcY-xI2R047gyr7hQo_RZM";

        public ChatService(HttpClient http, IConfiguration config, AppDbContext db)
        {
            _http = http;
            _config = config;
            _db = db;
        }

        public async Task<string> ProcesarMensajeIA(string sessionId, Negocio negocio, string mensajeUsuario)
        {
            // 1. Obtener o crear conversación
            var conversacion = await _db.Conversaciones
                .Include(c => c.Mensajes)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId && c.NegocioId == negocio.Id);

            if (conversacion == null)
            {
                conversacion = new Conversacion { SessionId = sessionId, NegocioId = negocio.Id };
                _db.Conversaciones.Add(conversacion);
            }

            // 2. Guardar mensaje del usuario
            var msjUser = new Mensaje { Texto = mensajeUsuario, EsUsuario = true };
            conversacion.Mensajes.Add(msjUser);
            await _db.SaveChangesAsync(); // Se guarda en BD

            // 3. Preparar contexto base
            string contextoBase = $@"Eres el asistente virtual AI de: {negocio.Nombre}.
Categoría: {negocio.Categoria}.
Descripción: {negocio.Descripcion}.
Horario: {negocio.Horario}.
Dirección: {negocio.Direccion}. Teléfono: {negocio.Telefono}.
Reglas: Responde directo, amable, conciso. Utiliza la información disponible.
[Fin de las instrucciones internas del sistema]";

            // 4. Preparar historial para Gemini API
            var contentsList = new List<object>();
            
            // Primer mensaje simula ser system (lo inyectamos en el primer mensaje de usuario)
            contentsList.Add(new { role = "user", parts = new[] { new { text = contextoBase } } });
            contentsList.Add(new { role = "model", parts = new[] { new { text = "Entendido, actuaré como el asistente." } } });

            // Agregar historial
            foreach (var m in conversacion.Mensajes.OrderBy(x => x.FechaCreacion))
            {
                contentsList.Add(new
                {
                    role = m.EsUsuario ? "user" : "model",
                    parts = new[] { new { text = m.Texto } }
                });
            }

            var requestBody = new { contents = contentsList };
            var jsonBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent?key={GeminiApiKey}";
            string textResponse;

            try
            {
                var response = await _http.PostAsync(endpoint, content);
                var rawResponse = await response.Content.ReadAsStringAsync();
                
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("GEMINI ERROR: " + rawResponse);
                    textResponse = $"Error de IA: {rawResponse}";
                }
                else
                {
                    using JsonDocument doc = JsonDocument.Parse(rawResponse);
                    textResponse = doc.RootElement
                                          .GetProperty("candidates")[0]
                                          .GetProperty("content")
                                          .GetProperty("parts")[0]
                                          .GetProperty("text")
                                          .GetString() ?? "No pude generar respuesta.";
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.Message);
                textResponse = $"Servicio no disponible: {ex.Message}";
            }

            // 5. Guardar respuesta de la IA en la BD
            var msjIA = new Mensaje { Texto = textResponse, EsUsuario = false };
            conversacion.Mensajes.Add(msjIA);
            await _db.SaveChangesAsync();

            return textResponse;
        }
    }
}
