using UnityEngine;
using UnityEngine.UI;

public class MaxSpeedSelector : MonoBehaviour
{
    public Slider maxSpeedSlider; // Riferimento allo slider di MaxSpeed
    public Text maxSpeedText; // Riferimento al testo che mostra il valore di MaxSpeed
    public string carName; // Il nome della macchina a cui questo slider è riferito
    public float defaultValue = 90f; // Il valore di default per questo slider  
    public float minValue = 20f; // Il valore minimo per questo slider
    public float maxValue = 190f; // Il valore massimo per questo slider

    private void Start()
    {
        // Verifica se lo slider è per la macchina selezionata
        string selectedCar = PlayerPrefs.GetString("SelectedCar", "DefaultCar");
        if(carName == selectedCar)
        {
            // Configura lo slider con i valori specifici
            maxSpeedSlider.minValue = minValue;
            maxSpeedSlider.maxValue = maxValue;
            maxSpeedSlider.value = PlayerPrefs.GetFloat($"{selectedCar}_maxSpeed", defaultValue);
        } 
        else
        {
            // Se non è la macchina selezionata, opzionalmente rendi lo slider non interagibile
            maxSpeedSlider.interactable = false;
        }

        UpdateMaxSpeedText(maxSpeedSlider.value);
        maxSpeedSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        // Salva il nuovo valore di MaxSpeed specifico per la macchina selezionata
        PlayerPrefs.SetFloat($"{carName}_maxSpeed", value);
        PlayerPrefs.Save(); // Non dimenticare di salvare le modifiche
        UpdateMaxSpeedText(value);
    }

    private void UpdateMaxSpeedText(float value)
    {
        maxSpeedText.text = $"Max Speed: {value:F0}";
    }
}
