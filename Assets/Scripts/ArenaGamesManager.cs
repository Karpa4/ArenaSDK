using UnityEngine;
using UnityEngine.UI;
using ArenaGames;
using TMPro;

public class ArenaGamesManager : MonoBehaviour
{
    [SerializeField] private UILiderboard uiLiderboard;
    [SerializeField] private Button getLiderboard;
    [SerializeField] private TextMeshProUGUI errorText;

    [SerializeField] private string gameAlias;
    [SerializeField] private string leaderboardAlias;
    [SerializeField] private string xAuthServer;

    private ArenaConnector connector;

    private void Start()
    {
        connector = new ArenaConnector(gameAlias, leaderboardAlias, xAuthServer);
        connector.GetNewError += ShowError;
        getLiderboard.onClick.AddListener(ShowLiderboard);
        
    }

    private void ShowError(string message)
    {
        errorText.text = message;
    }

    private async void ShowLiderboard()
    {
        Lider[] liders = await connector.GetLiderboard();
        if (liders != null)
        {
            uiLiderboard.ShowNewLiderboard(liders);
        }
    }

    public void GuestAuth()
    {
        connector.GuestAuth();
    }

    public async void UpdateStats(int newScore)
    {
        await connector.UpdateStatistics(newScore);
    }

    public async void Register(string password, string username, string email)
    {
        await connector.RegisterUser(password, username, email, true);
    }

    public async void Auth(string password, string login)
    {
        await connector.Auth—lient(password, login);
    }

    private void OnDestroy()
    {
        connector.GetNewError -= ShowError;
        getLiderboard.onClick.RemoveListener(ShowLiderboard);
    }
}
