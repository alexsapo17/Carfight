using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FontUpdater : MonoBehaviour
{
    public Font fontToUse;

    // Metodo da chiamare per aggiornare i font
    public void UpdateAllFonts()
    {
        if (fontToUse == null)
        {
            Debug.LogError("Font non assegnato!");
            return;
        }

        Text[] texts = FindObjectsOfType<Text>();
        foreach (Text text in texts)
        {
            text.font = fontToUse;
        }

        Debug.Log("Font aggiornati: " + texts.Length);
    }

#if UNITY_EDITOR
    // Metodo per eseguire lo script dall'editor
    [MenuItem("Tools/Update All Fonts")]
    private static void UpdateFontsMenu()
    {
        FontUpdater fontUpdater = FindObjectOfType<FontUpdater>();
        if (fontUpdater != null)
        {
            fontUpdater.UpdateAllFonts();
        }
    }
#endif
}
