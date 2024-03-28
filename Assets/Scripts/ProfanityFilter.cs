using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class ProfanityFilter : MonoBehaviour
{
    public InputField inputField;
    public TextAsset bannedWordsJSON;

    private HashSet<string> bannedWords;
    private Coroutine censorCoroutine;

    void Start()
    {
        if (inputField == null || bannedWordsJSON == null)
        {
            Debug.LogError("InputField or Banned Words JSON not assigned.");
            return;
        }

        // Carica le parole bandite dal file JSON
        bannedWords = LoadBannedWords(bannedWordsJSON);

        // Avvia la coroutine per la censura in background
        censorCoroutine = StartCoroutine(CensorInputField());
    }

    IEnumerator CensorInputField()
    {
        while (true)
        {
            // Attendere che l'utente inserisca del testo
            yield return new WaitUntil(() => inputField.isFocused);

            // Censura le parole bandite nel testo dell'InputField
            string censoredText = CensorProfanity(inputField.text);

            // Aggiorna il testo dell'InputField con quello censurato
            inputField.text = censoredText;

            // Imposta il cursore alla fine del testo censurato
            inputField.caretPosition = censoredText.Length;

            // Attendere un frame prima di verificare di nuovo l'input dell'utente
            yield return null;
        }
    }

    string CensorProfanity(string text)
    {
        // Utilizza un'espressione regolare per censurare tutte le occorrenze delle parole bandite
        foreach (string bannedWord in bannedWords)
        {
            // Utilizza l'opzione RegexOptions.IgnoreCase per rendere il confronto case-insensitive
            string pattern = @"\b" + Regex.Escape(bannedWord) + @"\b"; // \b indica i bordi delle parole
            text = Regex.Replace(text, pattern, new string('*', bannedWord.Length), RegexOptions.IgnoreCase);
        }

        return text;
    }

    HashSet<string> LoadBannedWords(TextAsset jsonFile)
    {
        // Deserializza il file JSON in un HashSet di stringhe
        string[] words = jsonFile.text.Split(new char[] { '\n', '\r', ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        HashSet<string> loadedWords = new HashSet<string>(words);
        return loadedWords;
    }
}
