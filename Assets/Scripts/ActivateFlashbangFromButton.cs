using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ActivateFlashbangFromButton : MonoBehaviour
{
    public Button flashbangButton; // Assegna questo nell'Inspector

    void Start()
    {
        flashbangButton.onClick.AddListener(ActivateFlashbang);
    }
 
    void ActivateFlashbang()
    {
        // Lo stesso approccio di ActivateShockwaveFromButton
        foreach (var pv in FindObjectsOfType<PhotonView>())
        {
            if (pv.IsMine && pv.gameObject.GetComponent<PlayerEffects>())
            {
                var playerEffects = pv.gameObject.GetComponent<PlayerEffects>();
                playerEffects.StartFlashbangEffect();
                break;
            }
        }
    }
}
