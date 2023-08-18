using System.Collections.Generic;
using UnityEngine;
using ArenaGames;

public class UILiderboard : MonoBehaviour
{
    [SerializeField] private Transform pointToLiderItem;
    [SerializeField] private UILiderItem liderItemPrefab;

    List<UILiderItem> liderItems = new List<UILiderItem>();

    public void ShowNewLiderboard(Lider[] liders)
    {
        for (int i = 0; i < liders.Length; i++)
        {
            if (liderItems.Count < i)
            {
                liderItems[i].Construct(liders[i]);
            }
            else
            {
                UILiderItem newItem = Instantiate(liderItemPrefab, pointToLiderItem);
                newItem.Construct(liders[i]);
                liderItems.Add(newItem);
            }
        }
    }
}
