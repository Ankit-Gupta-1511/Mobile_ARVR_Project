using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShoeCanvasController : MonoBehaviour
{
    public static ShoeCanvasController Instance;

    public Image shoeImageUI;
    public TextMeshProUGUI shoeNameText;
    public TextMeshProUGUI shoePriceText;
    public TextMeshProUGUI shoeDescriptionText;

    void Awake()
    {
        Instance = this;
    }

    public void ShowShoeInfo(string name, string price, Sprite image, string description)
    {
        shoeNameText.text = name;
        shoePriceText.text = price;
        shoeImageUI.sprite = image;
        shoeDescriptionText.text = description;
    }
}
