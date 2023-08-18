using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpdateStats : MonoBehaviour
{
    [SerializeField] private ArenaGamesManager arenaManager;
    [SerializeField] private TMP_InputField scoreField;
    [SerializeField] private Button submit;
    [SerializeField] private Button guestAuth;

    private void Start()
    {
        submit.onClick.AddListener(UpdateStats);
        guestAuth.onClick.AddListener(GuestAuth);
    }

    private void UpdateStats()
    {
        if (Int32.TryParse(scoreField.text, out int result))
        {
            arenaManager.UpdateStats(result);
        }
    }

    private void GuestAuth()
    {
        arenaManager.GuestAuth();
    }

    private void OnDestroy()
    {
        submit.onClick.RemoveListener(UpdateStats);
        guestAuth.onClick.RemoveListener(GuestAuth);
    }
}
