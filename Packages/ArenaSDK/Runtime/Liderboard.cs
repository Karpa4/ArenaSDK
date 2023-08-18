using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ArenaGames
{
    internal class Liderboard
    {
        private readonly HttpClient httpClient;
        private readonly string boardURL;
        private readonly string updateURL;
        private readonly string xAuthServer;
        private const string query = "aroundPlayerLimit=10&isAroundPlayer=true&limit=20&offset=0";

        public event Action<string> GetErrorStatusCode;
        public event Action<DateTime> Gettime;

        public Liderboard(HttpClient httpClient, string gameAlias, string leaderboardAlias, string xAuthServer) 
        {
            this.xAuthServer = xAuthServer;
            this.httpClient = httpClient;
            boardURL = ArenaConnector.MainURL + "client/" + gameAlias + "/leaderboard/" + leaderboardAlias;
            updateURL = ArenaConnector.MainURL + "server/" + gameAlias + "/leaderboard/" + leaderboardAlias + "/score";
            
        }

        internal async Task<Lider[]> GetLiderboardAsync(string token)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(boardURL + "?" + query),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("access-token", token);

            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                Lider[] result =  ParseLidersArray(content);
                return result;
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Gettime?.Invoke(DateTime.Now);
                GetErrorStatusCode?.Invoke(errorContent);
                return new Lider[0];
            }
        }

        internal async Task<bool> UpdateStatistics(string profileId, int score)
        {
            string body = BuildJson(profileId, score);
            
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(updateURL),
                Method = HttpMethod.Patch,
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("x-auth-server", xAuthServer);

            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                GetErrorStatusCode?.Invoke(errorContent);
                return false;
            }
        }

        private string BuildJson(string profileId, int score)
        {
            var jsonData = new
            {
                profileId = profileId,
                value = score
            };
            string json = JsonConvert.SerializeObject(jsonData);
            return json;
        }

        private Lider[] ParseLidersArray(string content)
        {
            JObject json = JObject.Parse(content);
            string lidersJSON = json.SelectToken("leaderboards").ToString();
            Lider[] result = JsonConvert.DeserializeObject<Lider[]>(lidersJSON);
            return result;
        }
    }
}
