using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    public string abilityName;
    public int coinsCost;
    public int gemsCost;
    public GameObject blockedImage;
    public Button buyButton;

    private bool isUnlocked = false;

    private void Start()
    {
        // Controlla se il pulsante è stato precedentemente sbloccato e aggiorna lo stato di sblocco
        isUnlocked = IsButtonUnlocked();

        // Se il pulsante è già stato sbloccato, disattiva l'immagine bloccata e abilita il pulsante
        if (isUnlocked)
        {
            blockedImage.SetActive(false);
            buyButton.gameObject.SetActive(false);
            GetComponent<Button>().interactable = true;
        }
    }

    public void PurchaseAbility()
    {
        // Verifica se il pulsante è già stato sbloccato
        if (!isUnlocked)
        {
            // Verifica se il giocatore ha abbastanza monete o gemme per l'acquisto
            if (CurrencyManager.Instance.HasEnoughCoins(coinsCost) && CurrencyManager.Instance.HasEnoughGems(gemsCost))
            {
                // Sottrai il costo dall'inventario del giocatore
                CurrencyManager.Instance.ModifyCoins(-coinsCost);
                CurrencyManager.Instance.ModifyGems(-gemsCost);

                // Disattiva l'immagine bloccata e abilita il pulsante
                blockedImage.SetActive(false);
                buyButton.gameObject.SetActive(false);
                GetComponent<Button>().interactable = true;

                // Salva lo sblocco del pulsante nelle PlayerPrefs
                PlayerPrefs.SetInt(GetButtonUnlockKey(), 1);
                PlayerPrefs.Save();

                Debug.Log("Pulsante abilità acquistato con successo!");
                isUnlocked = true;
            }
            else
            {
                Debug.Log("Fondi insufficienti per l'acquisto del pulsante abilità!");
            }
        }
        else
        {
            Debug.Log("Il pulsante abilità è già stato sbloccato!");
        }
    }

    private bool IsButtonUnlocked()
    {
        // Controlla se il pulsante è stato sbloccato precedentemente
        return PlayerPrefs.GetInt(GetButtonUnlockKey(), 0) == 1;
    }

    private string GetButtonUnlockKey()
    {
        // Genera una chiave univoca per il salvataggio dello sblocco del pulsante
        return $"{abilityName}_Unlocked";
    }
}
