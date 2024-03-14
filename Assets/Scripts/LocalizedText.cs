using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    [TextArea(3, 10)]
    public string englishText = "Default English Text";
    [TextArea(3, 10)]
    public string italianText = "Testo italiano predefinito";
    [TextArea(3, 10)]
    public string spanishText = "Texto español predeterminado";
    [TextArea(3, 10)]
    public string frenchText = "Texte français par défaut";

    void Start()
    {
        UpdateTextLanguage();
    }

    public void UpdateTextLanguage()
    {
        string language = PlayerPrefs.GetString("Language", "en");
        string selectedText = englishText; // default to English

        switch (language)
        {
            case "it":
                selectedText = italianText;
                break;
            case "es":
                selectedText = spanishText;
                break;
            case "fr":
                selectedText = frenchText;
                break;
        }

        if (GetComponent<TextMesh>()) // For 3D TextMesh objects
        {
            GetComponent<TextMesh>().text = selectedText;
        }
        else if (GetComponent<Text>()) // For UI Text components
        {
            GetComponent<Text>().text = selectedText;
        }
    }
}
