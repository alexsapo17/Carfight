using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class ActivateShockwaveFromButton : MonoBehaviour
{
    public Button shockwaveButton; // Assegna questo nell'Inspector
    public GameObject visualEffectPrefab; // Assegna il prefab dell'effetto visivo nell'Inspector

    void Start()
    {
        shockwaveButton.onClick.AddListener(ActivateShockwave);
    }

   public void ActivateShockwave()
    {
        // Cerca tra tutti i PhotonView presenti nella scena
        foreach (var pv in FindObjectsOfType<PhotonView>())
        {
            // Controlla se il PhotonView appartiene al giocatore locale e ha un componente PlayerEffects
            if (pv.IsMine && pv.gameObject.GetComponent<PlayerEffects>())
            {
                // Ottieni il componente PlayerEffects e attiva l'effetto
                var playerEffects = pv.gameObject.GetComponent<PlayerEffects>();
                playerEffects.StartShockwaveEffect();

                // Istanziare l'effetto visivo nel punto della macchina
                Instantiate(visualEffectPrefab, pv.transform.position, Quaternion.identity);

                break; // Interrompe il ciclo una volta trovato e attivato l'effetto
            }
        }
    }
}
