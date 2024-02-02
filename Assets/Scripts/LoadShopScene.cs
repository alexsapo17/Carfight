using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadShopScene : MonoBehaviour
{
  public static bool shouldAutoClickButton1 = false;
    public static bool shouldAutoClickButton2 = false;
public Animator transitionAnimator;

    public void LoadSceneAndSetFlagButton1()
    {
     transitionAnimator.SetTrigger("Start"); // Avvia l'animazione di transizione
    Invoke("LoadShopScene1", 1f); 
    }

    public void LoadShopScene1()
    {
        shouldAutoClickButton1 = true;
        shouldAutoClickButton2 = false;
        SceneManager.LoadScene("ShopScene");
    }


    public void LoadSceneAndSetFlagButton2()
    {
      transitionAnimator.SetTrigger("Start"); // Avvia l'animazione di transizione
    Invoke("LoadShopScene2", 1f); 
    }

        public void LoadShopScene2()
    {
        shouldAutoClickButton1 = false;
        shouldAutoClickButton2 = true;
        SceneManager.LoadScene("ShopScene");
    }
}
