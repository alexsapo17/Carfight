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
    if (!isUnlocked)
    {
        if (CurrencyManager.Instance.HasEnoughCoins(coinsCost) && CurrencyManager.Instance.HasEnoughGems(gemsCost))
        {
            CurrencyManager.Instance.ModifyCoins(-coinsCost);
            CurrencyManager.Instance.ModifyGems(-gemsCost);

            blockedImage.SetActive(false);
            buyButton.gameObject.SetActive(false);
            GetComponent<Button>().interactable = true;

            PlayerPrefs.SetInt(GetButtonUnlockKey(), 1);
            PlayerPrefs.Save();

            Debug.Log("Pulsante abilità acquistato con successo!");
            isUnlocked = true;

            GameObject animatedImagePrefab = Resources.Load<GameObject>("GoldExplosionAnim");
            if (animatedImagePrefab != null)
            {
                Canvas canvas = FindObjectOfType<Canvas>();
                Camera camera = canvas.worldCamera; // Ottieni la camera usata dal Canvas
                
                // Calcola la rotazione per far sì che il prefab guardi verso la camera.
                Quaternion rotationTowardsCamera = Quaternion.LookRotation(camera.transform.forward);

                // Istanzia il prefab con la rotazione calcolata
                GameObject animatedImageInstance = Instantiate(animatedImagePrefab, buyButton.transform.position, rotationTowardsCamera, this.transform);
                Destroy(animatedImageInstance, 1f);
            }
            else
            {
                Debug.LogError("Il prefab AnimatedImagePrefab non è stato trovato nella cartella Resources.");
            }
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
