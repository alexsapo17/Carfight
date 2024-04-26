using UnityEngine;
using UnityEngine.UI;

public class InputFieldController : MonoBehaviour
{
    public InputField inputField;
    public Text characterCountText;
    public int characterLimit = 500; // Limite di caratteri

    private void Start()
    {
        // Assicura che l'Input Field abbia il focus quando il gioco inizia
        inputField.Select();
        inputField.ActivateInputField();

        // Aggiorna il conteggio dei caratteri all'avvio
        UpdateCharacterCount(inputField.text);
        
        // Assegna gli eventi per il controllo del limite di caratteri
        inputField.onValueChanged.AddListener(OnValueChanged);
    }

    // Metodo chiamato ogni volta che il testo dell'Input Field cambia
    private void OnValueChanged(string newText)
    {
        // Controlla il limite di caratteri
        if (newText.Length > characterLimit)
        {
            // Tronca il testo se supera il limite
            inputField.text = newText.Substring(0, characterLimit);
        }

        // Aggiorna il conteggio dei caratteri
        UpdateCharacterCount(inputField.text);
    }

    // Aggiorna il testo del conteggio dei caratteri
    private void UpdateCharacterCount(string text)
    {
        int characterCount = text.Length;
        characterCountText.text = characterCount + "/" + characterLimit;
    }
}
