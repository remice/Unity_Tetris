using UnityEngine;
using TMPro;

public class TextManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private TextMeshProUGUI combo;
    [SerializeField]
    private TextMeshProUGUI line;
    [SerializeField]
    private TextMeshProUGUI difficulty;

    private int alpha;

    public static TextManager instance;

    private void Awake()
    {
        instance = GetComponent<TextManager>();
        alpha = 0;
        text.alpha = alpha;
    }

    public void ChangeText(string change)
    {
        alpha = 180;
        text.alpha = alpha / 255f;
        text.text = change;
    }

    public void ChangeCombo(int amount)
    {
        combo.text = amount.ToString();
    }

    public void ChangeLine(int amount)
    {
        line.text = amount.ToString();
    }

    public void ChangeDifficulty(int amount)
    {
        difficulty.text = amount.ToString();
    }

    private void FixedUpdate()
    {
        alpha--;
        text.alpha = alpha / 255f;
    }
}
