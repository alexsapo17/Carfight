using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AutoClickButton : MonoBehaviour
{
    public Button buttonToClick1;
    public Button buttonToClick2;

    IEnumerator Start()
    {
        // Aspetta un momento per permettere alla scena di caricarsi
        yield return new WaitForSeconds(0.2f);

        if (LoadShopScene.shouldAutoClickButton1) // Modifica qui
        {
            buttonToClick1.onClick.Invoke();
            // Resetta il flag per evitare clic successivi non desiderati
            LoadShopScene.shouldAutoClickButton1 = false; // Modifica qui
        }
        else if (LoadShopScene.shouldAutoClickButton2) // Modifica qui
        {
            buttonToClick2.onClick.Invoke();
            // Resetta il flag per evitare clic successivi non desiderati
            LoadShopScene.shouldAutoClickButton2 = false; // Modifica qui
        }
    }
}
