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

            GameObject animatedImagePrefab = Resources.Load<GameObject>("HitMiscATest");
if (animatedImagePrefab != null)
{
    Canvas canvas = FindObjectOfType<Canvas>();
    Camera camera = canvas.worldCamera; // Ottieni la camera usata dal Canvas

    // Ottieni la posizione del bottone rispetto alla camera
    Vector3 buttonPositionOnScreen = camera.WorldToScreenPoint(buyButton.transform.position);

    // Calcola una posizione lungo la direzione della camera, a una certa distanza davanti ad essa
    Vector3 positionInFrontOfCamera = camera.ScreenToWorldPoint(new Vector3(buttonPositionOnScreen.x, buttonPositionOnScreen.y, camera.nearClipPlane + 0.5f)); // Sostituisci '1' con la distanza che preferisci

    // La rotazione che punta verso la camera
    Quaternion rotationTowardsCamera = Quaternion.LookRotation(camera.transform.position - positionInFrontOfCamera);

    // Istanzia il prefab con la posizione e la rotazione calcolate
    GameObject animatedImageInstance = Instantiate(animatedImagePrefab, positionInFrontOfCamera, rotationTowardsCamera);

    // Assicurati che l'oggetto sia parented correttamente se necessario
    // Potresti dover parentarlo al canvas o ad un altro oggetto, a seconda delle tue esigenze
animatedImageInstance.transform.SetParent(this.transform, true);
animatedImageInstance.transform.localScale = new Vector3(1, 1, 1); // Imposta la scala desiderata

    // Distrugge l'oggetto dopo 1 secondo
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
