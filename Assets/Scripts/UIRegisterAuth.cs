using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRegisterAuth : MonoBehaviour
{
    [SerializeField] private ArenaGamesManager arenaManager;
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passField;
    [SerializeField] private Button submit;
    [SerializeField] private Button regState;
    [SerializeField] private Button authState;

    private bool isAuthState;

    private void Start()
    {
        ActiveAuthState();
        regState.onClick.AddListener(ActiveRegisterState);
        authState.onClick.AddListener(ActiveAuthState);
        submit.onClick.AddListener(Submit);
    }

    private void ActiveRegisterState()
    {
        isAuthState = false;
        authState.interactable = true;
        regState.interactable = false;
        emailField.gameObject.SetActive(true);
    }

    private void ActiveAuthState()
    {
        isAuthState = true;
        authState.interactable = false;
        regState.interactable = true;
        emailField.gameObject.SetActive(false);
    }

    private void Submit()
    {
        if (isAuthState)
        {
            arenaManager.Auth(passField.text, nameField.text);
        }
        else
        {
            arenaManager.Register(passField.text, nameField.text, emailField.text);
        }
    }

    private void OnDestroy()
    {
        regState.onClick.RemoveListener(ActiveRegisterState);
        authState.onClick.RemoveListener(ActiveAuthState);
        submit.onClick.RemoveListener(Submit);
    }
}
