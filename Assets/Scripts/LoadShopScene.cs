using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadShopScene : MonoBehaviour
{
    public static bool shouldAutoClick = false;

    public void LoadSceneAndSetFlag()
    {
        shouldAutoClick = true;
        SceneManager.LoadScene("ShopScene");
    }
}
