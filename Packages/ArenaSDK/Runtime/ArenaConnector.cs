using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;
using System;
using System.Threading.Tasks;

namespace ArenaGames
{
    public class ArenaConnector
    {
        private HttpClient httpClient;
        private Liderboard liderboard;
        private AGToken accessToken;
        private AGToken refreshToken;
        private readonly string gameAlias;
        private UserData userData;

        internal static string MainURL = @"https://api.arenavs.com/api/v2/gamedev/";
        private const string AccessTokenURL = @"client/auth/guest/sign-up";
        private const string ProfileURL = @"client/my-profile";
        
        public event Action<string> GetNewError;

        public ArenaConnector(string gameAlias, string leaderboardAlias, string xAuthServer)
        {
            this.gameAlias = gameAlias;
            httpClient = new HttpClient();
            liderboard = new Liderboard(httpClient, gameAlias, leaderboardAlias, xAuthServer);
            liderboard.GetErrorStatusCode += ParseError;
        }

        public async Task<bool> UpdateStatistics(int newScore)
        {
            if (await CheckTokens())
            {
                return await liderboard.UpdateStatistics(userData.Id, newScore);
            }
            else
            {
                return false;
            }
        }

        public async Task<Lider[]> GetLiderboard()
        {
            if (await CheckTokens())
            {
                Lider[] result = await liderboard.GetLiderboardAsync(accessToken.Token);
                return result;
            }
            else
            {
                return null;
            }
        }

        public async void GuestAuth()
        {
            await GetGuestAccessToken();
        }

        private async Task<AGToken> GetGuestAccessToken()
        {
            var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
            var r = await httpClient.PostAsync(MainURL + AccessTokenURL, content);
            string fullText = await r.Content.ReadAsStringAsync();

            if (r.IsSuccessStatusCode)
            {
                JObject json = JObject.Parse(fullText);
                string tokenText = json.SelectToken("accessToken").ToString();
                accessToken = JsonConvert.DeserializeObject<AGToken>(tokenText);
                return accessToken;
            }
            else
            {
                ParseError(fullText);
                return null;
            }
        }

        private async Task<AGToken> RefreshToken()
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(MainURL + ProfileURL),
                Method = HttpMethod.Post,
                Content = new StringContent(string.Empty, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("refresh-token", refreshToken.Token);

            var response = await httpClient.SendAsync(request);
            string fullText = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                JObject json = JObject.Parse(fullText);
                string tokenText = json.SelectToken("accessToken").ToString();
                accessToken = JsonConvert.DeserializeObject<AGToken>(tokenText);
                return accessToken;
            }
            else
            {
                ParseError(fullText);
                return null;
            }
        }

        public async Task<bool> AuthСlient(string password, string login)
        {
            var jsonData = new
            {
                password = password,
                login = login
            };
            string body = JsonConvert.SerializeObject(jsonData);
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(MainURL + "client/auth/sign-in", content);
            string fullText = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                JObject json = JObject.Parse(fullText);
                string accessTokenJson = json.SelectToken("accessToken").ToString();
                accessToken = JsonConvert.DeserializeObject<AGToken>(accessTokenJson);
                string refreshTokenJson = json.SelectToken("refreshToken").ToString();
                refreshToken = JsonConvert.DeserializeObject<AGToken>(refreshTokenJson);
                return true;
            }
            else
            {
                ParseError(fullText);
                return false;
            }
        }

        public async Task<bool> RegisterUser(string password, string username, string email, bool needToAuth)
        {
            var jsonData = new
            {
                email = email,
                password = password,
                username = username
            };
            string body = JsonConvert.SerializeObject(jsonData);
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(MainURL + "client/" + gameAlias + "/user-registration", content);
            string fullText = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                userData = JsonConvert.DeserializeObject<UserData>(fullText);
                if (needToAuth)
                {
                    bool isAuth = await AuthСlient(password, username);
                    return isAuth;
                }
                return true;
            }
            else
            {
                ParseError(fullText);
                return false;
            }
        }

        private async Task<UserData> GetProfile()
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(MainURL + ProfileURL),
                Method = HttpMethod.Get
            };
            request.Headers.Add("access-token", accessToken.Token);

            var response = await httpClient.SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                UserData user = JsonConvert.DeserializeObject<UserData>(content);
                userData = user;
                return user;
            }
            else
            {
                ParseError(content);
                return null;
            }
        }

        private async Task<bool> CheckTokens()
        {
            if (accessToken == null)
            {
                GetNewError?.Invoke("Вы не авторизованы");
                return false;
            }
            else
            {
                if (DateTime.Now >= accessToken.ExpiresIn)
                {
                    if (refreshToken != null)
                    {
                        await RefreshToken();
                        return true;
                    }
                    else
                    {
                        GetNewError?.Invoke("Срок действия токена доступа закончился");
                        return false;
                    }
                }
                return true;
            }
        }

        private void ParseError(string content)
        {
            JObject json = JObject.Parse(content);
            string errorText = json.SelectToken("message").ToString();
            GetNewError?.Invoke(errorText);
        }
    }
}