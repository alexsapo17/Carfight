using UnityEngine;
using Photon.Pun;

public class ApplyCarParameters : MonoBehaviourPunCallbacks
{
    private PrometeoCarController carController;

    void Start()
    {
        // Assicurati di eseguire questo codice solo per la macchina controllata dal giocatore locale
        if (photonView.IsMine)
        {
            carController = GetComponent<PrometeoCarController>();
            ApplyParameters();
        }
    }

    void ApplyParameters()
    {
        string selectedCar = PlayerPrefs.GetString("SelectedCar", "DefaultCar");
        
        // Applica AccelerationMultiplier
        float accelerationMultiplier = PlayerPrefs.GetFloat($"{selectedCar}_accelerationMultiplier", carController.accelerationMultiplier);
        carController.accelerationMultiplier = (int)Mathf.Clamp(accelerationMultiplier, 1, 15);
        Debug.Log($"Parametri applicati alla macchina {selectedCar}: Acceleration Multiplier = {accelerationMultiplier}");

        // Applica MaxSpeed
        float maxSpeed = PlayerPrefs.GetFloat($"{selectedCar}_maxSpeed", carController.maxSpeed);
        carController.maxSpeed = (int)Mathf.Clamp(maxSpeed, 50, 250); 
        Debug.Log($"Parametri applicati alla macchina {selectedCar}: Max Speed = {maxSpeed}");
    }
}
