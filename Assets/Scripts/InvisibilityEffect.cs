using UnityEngine;
using Photon.Pun;

public class InvisibilityEffect : MonoBehaviourPunCallbacks
{
    public float effectDuration = 5f;
    
    

    public void ActivateInvisibility(PlayerEffects playerEffects)
    {
        if (playerEffects != null)
        {
            playerEffects.StartInvisibilityTimer(effectDuration);
            
        }
    }

 
 


}
