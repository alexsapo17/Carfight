using UnityEngine;
using UnityEngine.UI;

public class ParameterSliderUnlocker : MonoBehaviour
{
    public string parameterName;
    public int coinsCost;
    public int gemsCost;
    public GameObject blockedImage;
    public Button buyButton;

    private Slider parameterSlider;
    private bool isUnlocked = false;

    private void Start()
    {
        parameterSlider = GetComponent<Slider>();
        isUnlocked = IsSliderUnlocked();

        if (isUnlocked)
        {
            blockedImage.SetActive(false);
            buyButton.gameObject.SetActive(false);
            parameterSlider.interactable = true;
        }
        else
        {
            parameterSlider.interactable = false;
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
        parameterSlider.interactable = true;

        PlayerPrefs.SetInt(GetSliderUnlockKey(), 1);
        PlayerPrefs.Save();
        Debug.Log($"Slider {parameterName} sbloccato con successo!");
        isUnlocked = true;

        // Istanzia il prefab come figlio del GameObject attuale e con la stessa posizione del buyButton
        GameObject animatedImagePrefab = Resources.Load<GameObject>("GoldExplosionAnim");
        if (animatedImagePrefab != null)
        {
            // Nota: Qui viene usata la posizione globale del buyButton per istanziare il prefab, ma poi il prefab diventa figlio di this.transform
            GameObject animatedImageInstance = Instantiate(animatedImagePrefab, buyButton.transform.position, Quaternion.identity, this.transform);
Destroy(animatedImageInstance, 1f);
            // Dopo l'istanziazione, la posizione locale del prefab potrebbe dover essere aggiustata
            // per mantenere la stessa posizione visiva che aveva rispetto al buyButton.
            // Questo passaggio potrebbe non essere necessario se vuoi che il prefab appaia esattamente dove si trova il GameObject dello script.
            // Se necessario, aggiusta la posizione locale qui. Ad esempio:
            // animatedImageInstance.transform.localPosition = Vector3.zero; // Adegua questi valori per posizionarlo correttamente
        }
        else 
        {
            Debug.LogError("Il prefab AnimatedImagePrefab non è stato trovato nella cartella Resources.");
        }
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
