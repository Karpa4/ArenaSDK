using UnityEngine;
using TMPro;
using ArenaGames;

public class UILiderItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI positionText;

    public void Construct(Lider lider)
    {
        scoreText.text = lider.Score.ToString();
        nameText.text = lider.Username.ToString();
        positionText.text = lider.Position.ToString();
    }
}
