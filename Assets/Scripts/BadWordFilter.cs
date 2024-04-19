using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class BadWordFilter : MonoBehaviour
{
    public InputField inputField;
    public TextAsset[] bannedWordsFiles; // Array di file di testo contenenti parole bandite

    private HashSet<string> bannedWords; // Set per memorizzare le parole bandite

    void Start()
    {
        LoadBannedWords();
        inputField.onValueChanged.AddListener(OnInputFieldValueChanged);
    }

    void LoadBannedWords()
    {
        bannedWords = new HashSet<string>();

        // Carica tutte le parole bandite dai file di testo
        foreach (TextAsset file in bannedWordsFiles)
        {
            if (file != null)
            {
                string[] words = file.text.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (string word in words)
                {
                    bannedWords.Add(word.ToLower()); // Aggiungi le parole bandite in minuscolo per una corrispondenza senza distinzione tra maiuscole e minuscole
                }
            }
        }

        Debug.Log("Parole bandite caricate correttamente. Numero totale: " + bannedWords.Count);
    }

void OnInputFieldValueChanged(string newText)
{
    if (bannedWords == null || bannedWords.Count == 0)
        return;

    // Rimuove gli spazi
    newText = newText.Replace(" ", "");

    // Censura le parole bandite mentre l'utente digita nell'InputField
    foreach (string word in bannedWords)
    {
        string pattern = Regex.Escape(word); // Escapare la parola bandita per evitare interpretazioni errate come espressione regolare
        newText = Regex.Replace(newText, pattern, "****", RegexOptions.IgnoreCase); // Sostituisci la parola bandita con ****
    }

    inputField.text = newText;
}



}
