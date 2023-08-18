using System;

namespace ArenaGames
{
    [Serializable]
    public class Lider
    {
        public string ProfileId { get; }
        public string Username { get; }
        public int Score { get; }
        public int Position { get; }
        public long CreatedAt { get; }

        public Lider(string profileId, string username, int score, int position, long createdAt)
        {
            ProfileId = profileId;
            Username = username;
            Score = score;
            Position = position;
            CreatedAt = createdAt;
        }
    }
}
