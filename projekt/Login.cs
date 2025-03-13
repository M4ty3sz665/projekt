using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class Login
{


    public async Task<JsonResponse> Login(string username, string password)
    {
        string url = "http://127.0.0.1:3959/login";
        JsonResponse oneJsonResponse = new JsonResponse() { username = null };
        try
        {
            var jsonData = new
            {
                loginUsername = username,
                loginPassword = password
            };
            //JSON.stringify(jsonData)
            string stringifiedJson = JsonConvert.SerializeObject(jsonData);
            //System.Windows.MessageBox.Show(stringifiedJson);
            //Itt adjuk meg mit küldünk és mi lesz a headerben
            StringContent sendThis = new StringContent(stringifiedJson, Encoding.UTF8, "Application/JSON");
            //Õ a send és a ReadyState == XMLHttpRequest.DONE
            HttpResponseMessage response = await client.PostAsync(url, sendThis);
            //Megnézi, hogy a status code nem 4-el vagy 5-el kezdõdik
            response.EnsureSuccessStatusCode();
            //kiolvassuk a responsból hogy mit küldött a szerver
            string responseText = await response.Content.ReadAsStringAsync();
            //Átalakítjuk a kapott string információt a megfelelõ típusú objektumra, ami egyezik a küldött json objektummal
            JsonResponse responseJson = JsonConvert.DeserializeObject<JsonResponse>(responseText);
            //A szerver üzenetének kiírása
            //System.Windows.MessageBox.Show(responseJson.token,responseJson.message);
            responseJson.username = username;
            oneJsonResponse = responseJson;
            return oneJsonResponse;
            //Data.users.Add(responseJson);
        }
        catch (Exception e)
        {
            System.Windows.MessageBox.Show(e.Message);
            return oneJsonResponse;
        }


    }



    public async void Save(JsonResponse data, int result)
    {
        string url = "http://127.0.0.1:3959/save";
        try
        {
            var jsonData = new
            {
                nyerte = result
            };
            string stringifiedJson = JsonConvert.SerializeObject(jsonData);
            //System.Windows.MessageBox.Show(stringifiedJson);
            StringContent sendThis = new StringContent(stringifiedJson, Encoding.UTF8, "Application/JSON");
            //kell az authorization header: (setRequestHeader)
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", data.token);
            HttpResponseMessage response = await client.PutAsync(url, sendThis);
            response.EnsureSuccessStatusCode();
            string responseText = await response.Content.ReadAsStringAsync();
            JsonResponse responseJson = JsonConvert.DeserializeObject<JsonResponse>(responseText);
            //System.Windows.MessageBox.Show(responseJson.token, responseJson.message);
        }
        catch (Exception e)
        {
            System.Windows.MessageBox.Show(e.Message);
        }


    }

    public class JsonResponse
    {
        public string username { get; set; }
        public int lose { get; set; }
        public string message { get; set; }
        public string token { get; set; }
        public int win { get; set; }
        public int draw { get; set; }
    }
