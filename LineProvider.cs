using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

public class LineProvider
{
    protected string _ChannelID { get; set; }
    protected string _Url { get { return "https://api.line.me/v1/"; } }
    protected string _Secret { get; set; }

    public LineProvider(string ChannelID, string Secret)
    {
        _ChannelID = ChannelID;
        _Secret = Secret;
    }

    public class ValidityAccessTokenInfo
    {
        public string mid { get; set; }
        public string channelId { get; set; }
    }

    public class RetrievedAccessTokenInfo
    {
        public string mid { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string scope { get; set; }
    }

    public class RetrievedReissuingAccessTokenInfo
    {
        public string mid { get; set; }
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
        public string expire { get; set; }
    }

    public class RetrievedProfile
    {
        public string displayName { get; set; }
        public string mid { get; set; }
        public string pictureUrl { get; set; }
        public string statusMessage { get; set; }
    }

    public class RetrievingAccessTokenInfo
    {
        public string grant_type { get { return "authorization_code"; } }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string code { get; set; }
        public string redirect_uri { get; set; }

        public RetrievingAccessTokenInfo(string _client_id, string _client_secret, string _code, string _redirect_uri)
        {
            client_id = _client_id;
            client_secret = _client_secret;
            code = _code;
            redirect_uri = _redirect_uri;
        }
    }

    public async Task<Tuple<bool, RetrievedAccessTokenInfo>> RetrieveAccessToken(string authorizationCode, string redirectUri)
    {
        bool IsReplyReceived = false;
        var result = new RetrievedAccessTokenInfo();

        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(_Url);
            var content = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("grant_type", "authorization_code")
                    , new KeyValuePair<string, string>("client_id", _ChannelID)
                    , new KeyValuePair<string, string>("client_secret", _Secret)
                    , new KeyValuePair<string, string>("code", authorizationCode)
                    , new KeyValuePair<string, string>("redirect_uri", redirectUri)
                });
            var response = await client.PostAsync("oauth/accessToken", content);
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsAsync<RetrievedAccessTokenInfo>();
                IsReplyReceived = true;
            }
        }

        return new Tuple<bool, RetrievedAccessTokenInfo>(IsReplyReceived, result);
    }

    public async Task<Tuple<bool, RetrievedReissuingAccessTokenInfo>> RetrievedReissuingAccessToken(string access_token, string refreshToken)
    {
        bool IsReplyReceived = false;
        var result = new RetrievedReissuingAccessTokenInfo();

        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(_Url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("X-Line-ChannelToken", access_token);
            var content = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("refreshToken", refreshToken)
                    , new KeyValuePair<string, string>("channelSecret", _Secret)
                });
            var response = await client.PostAsync("oauth/accessToken", content);
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsAsync<RetrievedReissuingAccessTokenInfo>();
                IsReplyReceived = true;
            }
        }

        return new Tuple<bool, RetrievedReissuingAccessTokenInfo>(IsReplyReceived, result);
    }

    public async Task<Tuple<bool, RetrievedProfile>> RetrieveProfile(string access_token)
    {
        bool IsReplyReceived = false;
        var result = new RetrievedProfile();

        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(_Url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {access_token}");
            var response = await client.GetAsync("profile");
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsAsync<RetrievedProfile>();
                IsReplyReceived = true;
            }
        }

        return new Tuple<bool, RetrievedProfile>(IsReplyReceived, result);
    }

    public async Task<Tuple<bool, ValidityAccessTokenInfo>> ValidityAccessToken(string access_token)
    {
        bool IsReplyReceived = false;
        var result = new ValidityAccessTokenInfo();

        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(_Url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("X-Line-ChannelToken", access_token);
            var response = await client.GetAsync("oauth/verify");
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsAsync<ValidityAccessTokenInfo>();
                IsReplyReceived = true;
            }
        }

        return new Tuple<bool, ValidityAccessTokenInfo>(IsReplyReceived, result);
    }

    public async Task<bool> Logout(string access_token)
    {
        bool IsReplyReceived = false;

        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(_Url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("X-Line-ChannelToken", access_token);
            var response = await client.DeleteAsync("oauth/logout");
            if (response.IsSuccessStatusCode)
            {
                IsReplyReceived = true;
            }
        }

        return IsReplyReceived;
    }
}