using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Database;

public class RewardPanelController : MonoBehaviour
{
   
    public static RewardPanelController Instance;

    public GameObject rewardPanelPrefab; // Assegna il prefab del pannello di ricompensa

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
 

 
public void ShowRewardPanel(int coins, int gems)
{
    if (rewardPanelPrefab == null)
    {
        Debug.LogError("RewardPanel prefab is not assigned!");
        return;
    }

    GameObject rewardPanelInstance = Instantiate(rewardPanelPrefab);

    // Trova il Canvas nella scena e imposta come genitore del pannello
    Canvas canvas = FindObjectOfType<Canvas>();
    if (canvas != null)
    {
        rewardPanelInstance.transform.SetParent(canvas.transform, false);
    }
    else
    {
        Debug.LogError("Canvas not found in the scene!");
        return;
    }

    Text coinsText = rewardPanelInstance.transform.Find("CoinsText").GetComponent<Text>();
    Text gemsText = rewardPanelInstance.transform.Find("GemsText").GetComponent<Text>();
    Button closeButton = rewardPanelInstance.transform.Find("CloseButton").GetComponent<Button>();

     coinsText.text = coins.ToString(); 
    gemsText.text = gems.ToString(); 

    closeButton.onClick.AddListener(() => Destroy(rewardPanelInstance));

    UpdateUserRewards(coins, gems);
}

 private void UpdateUserRewards(int coins, int gems)
{
    CurrencyManager.Instance.ModifyCoins(coins);
    CurrencyManager.Instance.ModifyGems(gems);
}



}
