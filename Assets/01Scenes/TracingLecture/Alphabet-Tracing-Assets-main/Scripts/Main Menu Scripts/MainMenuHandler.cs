using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    public static MainMenuHandler Instance;
    public Sprite[] shapeSprites, lineSPrites;
    public Transform itemsContent;
    public GameObject itemPrefab_TextBased, itemPrefab_SpriteBased;
    [HideInInspector] public string panelName;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {

    }

    public void ShowTracingItems(string category)
    {
        foreach (Transform child in itemsContent)
        {
            Destroy(child.gameObject);
        }
        switch (category)
        {
            case "alphabet":
                for (int i = 0; i < 26; i++)
                {
                    int num = i + 65;
                    SetItemText(i, num);
                }
                break;
            case "number":
                for(int i = 0; i<=9; i++)
                {
                    int num = i + 48;
                    SetItemText(i, num);
                }
                break;
            case "shape":
                for (int i = 0; i < shapeSprites.Length; i++)
                {
                    SetItemImage(i, shapeSprites);
                }
                break;
            case "line":
                for (int i = 0; i < lineSPrites.Length; i++)
                {
                    SetItemImage(i, lineSPrites);
                }
                break;
        }
    }


    private void SetItemText(int i, int num)
    {
        GameObject _item = Instantiate(itemPrefab_TextBased, itemsContent);
        _item.GetComponentInChildren<TextMeshProUGUI>().text = Convert.ToChar(num).ToString();
    }
    private void SetItemImage(int i, Sprite[] spritesItem)
    {
        GameObject _item = Instantiate(itemPrefab_SpriteBased, itemsContent);
        _item.transform.GetChild(0).GetComponent<Image>().sprite = spritesItem[i];

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
