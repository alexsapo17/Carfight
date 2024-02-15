using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class PurchaseManager : MonoBehaviour
{
 public Text purchaseFailedText;
    public float displayDuration = 5.0f;

    public void Add2000CoinsOnPurchase()
    {
        // Qui chiami CurrencyManager per aggiungere le monete
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.ModifyCoins(2000);
        }
    }
        public void Add5000CoinsOnPurchase()
    {
        // Qui chiami CurrencyManager per aggiungere le monete
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.ModifyCoins(5000);
        }
    }
            public void Add50GemsOnPurchase()
    {
        // Qui chiami CurrencyManager per aggiungere le monete
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.ModifyGems(50);
        }
    }
            public void Add200GemsOnPurchase()
    {
        // Qui chiami CurrencyManager per aggiungere le monete
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.ModifyGems(200);
        }
    }
            public void Add500GemsOnPurchase()
    {
        // Qui chiami CurrencyManager per aggiungere le monete
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.ModifyGems(500);
        }
    }
        public void ShowPurchaseFailed()
    {
        if (purchaseFailedText != null)
        {
            StartCoroutine(ShowMessageTemporarily());
        }
        else
        {
            Debug.LogError("PurchaseFailedText non è stato assegnato nel PurchaseManager.");
        }
    }

    private IEnumerator ShowMessageTemporarily()
    {
        purchaseFailedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(displayDuration);
        purchaseFailedText.gameObject.SetActive(false);
    }

}
