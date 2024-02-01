using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadShopScene : MonoBehaviour
{
  public static bool shouldAutoClickButton1 = false;
    public static bool shouldAutoClickButton2 = false;

    public void LoadSceneAndSetFlagButton1()
    {
        shouldAutoClickButton1 = true;
        shouldAutoClickButton2 = false;
        SceneManager.LoadScene("ShopScene");
    }

    public void LoadSceneAndSetFlagButton2()
    {
        shouldAutoClickButton1 = false;
        shouldAutoClickButton2 = true;
        SceneManager.LoadScene("ShopScene");
    }
}
