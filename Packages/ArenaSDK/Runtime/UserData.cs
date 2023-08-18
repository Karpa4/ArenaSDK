namespace ArenaGames
{
    public class PlayfabData
    {
        public string PlayfabId { get; }
        public string Token { get; }
        public long TokenExpired { get; }

        public PlayfabData(string playfabId, string token, long tokenExpired)
        {
            PlayfabId = playfabId;
            Token = token;
            TokenExpired = tokenExpired;
        }
    }

    public class UserData
    {
        public string Id { get; }
        public string Username { get; }
        public PlayfabData Playfab { get; }
        public string Email { get; }
        public string Avatar { get; }

        public UserData(string id, string username, PlayfabData playfab, string email, string avatar)
        {
            Id = id;
            Username = username;
            Playfab = playfab;
            Email = email;
            Avatar = avatar;
        }
    }
}
