using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UpdateTextColor : MonoBehaviour
{
    public string sceneName;
    private Text buttonText;
    private Button button;  // Aggiunta variabile per il componente Button

    void Start()
    {
        buttonText = GetComponentInChildren<Text>(); // Cambia in GetComponentInChildren<TextMeshProUGUI>() se usi TextMeshPro
        button = GetComponent<Button>();  // Ottiene il riferimento al componente Button
        UpdateColor();
    }

    void UpdateColor()
    {
        if (SceneManager.GetActiveScene().name == sceneName)
        {
            buttonText.color = Color.green;
            button.interactable = false;  // Disabilita il pulsante
        }
        else
        {
            buttonText.color = Color.white;
            button.interactable = true;   // Abilita il pulsante
        }
    }
}
