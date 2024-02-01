using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AutoClickButton : MonoBehaviour
{
    public Button buttonToClick;

    IEnumerator Start()
    {
        if (LoadShopScene.shouldAutoClick)
        {
            yield return new WaitForSeconds(0.3f); // Aspetta me
            buttonToClick.onClick.Invoke();
            LoadShopScene.shouldAutoClick = false;
        }
    }
}
