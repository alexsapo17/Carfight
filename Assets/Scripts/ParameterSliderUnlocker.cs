using UnityEngine;
using UnityEngine.UI;

public class ParameterSliderUnlocker : MonoBehaviour
{
    public string parameterName; // Nome del parametro, es. "AccelerationMultiplier"
    public int coinsCost; // Costo in monete per sbloccare lo slider
    public int gemsCost; // Costo in gemme per sbloccare lo slider
    public GameObject blockedImage; // Immagine che indica se lo slider è bloccato
    public Button buyButton; // Pulsante per acquistare lo sblocco dello slider

    private Slider parameterSlider; // Lo slider del parametro
    private bool isUnlocked = false; // Stato di sblocco dello slider

    private void Start()
    {
        parameterSlider = GetComponent<Slider>();

        // Controlla se lo slider è stato precedentemente sbloccato e aggiorna lo stato di sblocco
        isUnlocked = IsSliderUnlocked();

        if (isUnlocked)
        {
            // Se lo slider è già stato sbloccato, disattiva l'immagine bloccata e il pulsante di acquisto
            blockedImage.SetActive(false);
            buyButton.gameObject.SetActive(false);
            parameterSlider.interactable = true; // Rendi lo slider utilizzabile
        }
        else
        {
            parameterSlider.interactable = false; // Rendi lo slider non utilizzabile finché non viene sbloccato
        }
    }

    public void PurchaseParameterUnlock()
    {
        if (!isUnlocked && CurrencyManager.Instance.HasEnoughCoins(coinsCost) && CurrencyManager.Instance.HasEnoughGems(gemsCost))
        {
            CurrencyManager.Instance.ModifyCoins(-coinsCost);
            CurrencyManager.Instance.ModifyGems(-gemsCost);

            blockedImage.SetActive(false);
            buyButton.gameObject.SetActive(false);
            parameterSlider.interactable = true; // Rendi lo slider utilizzabile

            PlayerPrefs.SetInt(GetSliderUnlockKey(), 1);
            PlayerPrefs.Save();

            Debug.Log($"Slider {parameterName} sbloccato con successo!");
            isUnlocked = true;
        }
        else
        {
            Debug.Log("Non è possibile sbloccare lo slider, controlla se è già sbloccato o se hai abbastanza fondi.");
        }
    }

    private bool IsSliderUnlocked()
    {
        return PlayerPrefs.GetInt(GetSliderUnlockKey(), 0) == 1;
    }

    private string GetSliderUnlockKey()
    {
        return $"{parameterName}_Unlocked";
    }
}
