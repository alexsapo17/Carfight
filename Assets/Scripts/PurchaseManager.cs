using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class PurchaseManager : MonoBehaviour
{
 public GameObject purchaseFailedText;
  public GameObject purchasedText;

    public float displayDuration = 5.0f;

    public void Add2000CoinsOnPurchase()
    {
        // Qui chiami CurrencyManager per aggiungere le monete
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.ModifyCoins(2000);
                    if (purchasedText != null)
        {
            StartCoroutine(ShowMessageTemporarilyOK());
        }
        else
        {
            Debug.LogError("PurchasedText non è stato assegnato nel PurchaseManager.");
        }
        }
    }
        public void Add5000CoinsOnPurchase()
    {
        // Qui chiami CurrencyManager per aggiungere le monete
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.ModifyCoins(5000);
                               if (purchasedText != null)
        {
            StartCoroutine(ShowMessageTemporarilyOK());
        }
        else
        {
            Debug.LogError("PurchasedText non è stato assegnato nel PurchaseManager.");
        }
        }
    }
            public void Add50GemsOnPurchase()
    {
        // Qui chiami CurrencyManager per aggiungere le monete
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.ModifyGems(50);
                               if (purchasedText != null)
        {
            StartCoroutine(ShowMessageTemporarilyOK());
        }
        else
        {
            Debug.LogError("PurchasedText non è stato assegnato nel PurchaseManager.");
        }
        }
    }
            public void Add200GemsOnPurchase()
    {
        // Qui chiami CurrencyManager per aggiungere le monete
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.ModifyGems(200);
                               if (purchasedText != null)
        {
            StartCoroutine(ShowMessageTemporarilyOK());
        }
        else
        {
            Debug.LogError("PurchasedText non è stato assegnato nel PurchaseManager.");
        }
        }
    }
            public void Add500GemsOnPurchase()
    {
        // Qui chiami CurrencyManager per aggiungere le monete
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.ModifyGems(500);
                               if (purchasedText != null)
        {
            StartCoroutine(ShowMessageTemporarilyOK());
        }
        else
        {
            Debug.LogError("PurchasedText non è stato assegnato nel PurchaseManager.");
        }
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
    private IEnumerator ShowMessageTemporarilyOK()
    {
        purchasedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(displayDuration);
        purchasedText.gameObject.SetActive(false);
    }
}
