using UnityEngine;
using UnityEngine.UI;

public class LocalizedTextFixer : MonoBehaviour
{
    [ContextMenu("Fix Localized Text")]
    void FixLocalizedText()
    {
        Text[] allTextComponents = FindObjectsOfType<Text>(true); // Trova tutti i componenti Text nella scena, incluso quelli disattivati
        int textsChangedCount = 0;

        foreach (Text textComponent in allTextComponents)
        {
            LocalizedText localizedText = textComponent.gameObject.GetComponent<LocalizedText>();

            if (localizedText != null && localizedText.spanishText == "ATRAS")
            {
                localizedText.spanishText = "ATRÁS";
                localizedText.UpdateTextLanguage(); // Aggiorna il testo nella lingua corrente
                textsChangedCount++;
                Debug.Log("Testo cambiato in " + textComponent.gameObject.name);
            }
        }

        Debug.Log("Numero totale di testi cambiati: " + textsChangedCount);
    }
}
