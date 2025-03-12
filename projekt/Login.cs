using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class Login
{
    private readonly HttpClient _client = new HttpClient();

    public async Task<bool> LoginAsync(string username, string password)
    {
        var url = "http://127.0.0.1:3959/api/login"; // IP cím használata

        var requestData = new
        {
            username = username,
            password = password
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

        try
        {
            var response = await _client.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Szerver válasza: {responseContent}");

            var result = JsonConvert.DeserializeObject<LoginResponse>(responseContent);

            if (response.IsSuccessStatusCode && result != null && result.Success)
            {
                Properties.Settings.Default.UserToken = result.Token;
                Properties.Settings.Default.Save();

                return true;
            }
            else
            {
                Console.WriteLine($"Hiba: {result?.Message ?? "Ismeretlen hiba"}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hiba történt: {ex.Message}");
            throw; // Hogy az UI-ban is megjelenjen
        }
    }

    public class LoginResponse
    {
    public string Token { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
    }
