using UnityEngine;
using UnityEngine.UI;

public class AccelerationMultiplierSelector : MonoBehaviour
{
    public Slider accelerationSlider;
    public Text multiplierText;
    // Rimuovi la lista di configurazioni e usa variabili singole invece
    public string carName; // Il nome della macchina a cui questo slider è riferito
    public float defaultValue = 2f; // Il valore di default per questo slider
    public float minValue = 1f; // Il valore minimo per questo slider
    public float maxValue = 15f; // Il valore massimo per questo slider

    private void Start()
    {
        // Verifica se lo slider è per la macchina selezionata
        string selectedCar = PlayerPrefs.GetString("SelectedCar", "DefaultCar");
        if(carName == selectedCar)
        {
            // Applica la configurazione allo slider
            accelerationSlider.minValue = minValue;
            accelerationSlider.maxValue = maxValue;
            // Carica il valore specifico per questa macchina, se esiste, altrimenti usa il defaultValue
            accelerationSlider.value = PlayerPrefs.GetFloat($"{selectedCar}_accelerationMultiplier", defaultValue);
        }
        else
        {
            // Se non è la macchina selezionata, puoi scegliere di disabilitare lo slider o di impostare valori di default
            accelerationSlider.interactable = false; // Opzionale, basato sulla tua logica di gioco
        }

        UpdateMultiplierText(accelerationSlider.value);
        accelerationSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        // Assumi che questo slider sia sempre per la macchina selezionata attualmente
        PlayerPrefs.SetFloat($"{carName}_accelerationMultiplier", value);
        PlayerPrefs.Save(); // Assicurati di salvare dopo aver modificato le PlayerPrefs
        UpdateMultiplierText(value);
    }

    private void UpdateMultiplierText(float value)
    {
        multiplierText.text = $"Acceleration Multiplier: {value:F0}";
    }
}
