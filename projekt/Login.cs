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
        var url = "http://localhost:3959/api/login"; // Cseréld ki az API elérhetõségére

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

            var result = JsonConvert.DeserializeObject<LoginResponse>(responseContent);

            if (response.IsSuccessStatusCode && result.Success)
            {
                // Token mentése pl. Properties.Settings vagy Secure Storage segítségével
                Properties.Settings.Default.UserToken = result.Token;
                Properties.Settings.Default.Save();

                Console.WriteLine("Sikeres bejelentkezés!");
                return true;
            }
            else
            {
                Console.WriteLine($"Hiba: {result.Message}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hiba történt: {ex.Message}");
            return false;
        }
    }
}

public class LoginResponse
{
    public string Token { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
}
