using System;

namespace ArenaGames
{
    [Serializable]
    internal class AGToken
    {
        public string Token { get; }
        public DateTime ExpiresIn { get; }

        public AGToken(string token, string expiresIn)
        {
            Token = token;
            DateTime date = (new DateTime(1970, 1, 1, 3, 0, 0)).AddMilliseconds(double.Parse(expiresIn));
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            DateTime dateTimeWithTimeZone = TimeZoneInfo.ConvertTime(date, localTimeZone);
            ExpiresIn = dateTimeWithTimeZone;
        }
    }
}
