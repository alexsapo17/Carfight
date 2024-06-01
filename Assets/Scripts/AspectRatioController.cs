using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AspectRatioController : MonoBehaviour
{
    public float aspectRatioThreshold = 1.5f; // Soglia del rapporto di aspetto (ad esempio 1.5 per 3:2)
    public GameObject joystick;
    public GameObject abilityButton1;
    public GameObject abilityButton2;
    public GameObject abilityButton3;
    public GameObject abilityButton4;
    public GameObject abilityButton5;
    public GameObject throttleButton;
    public GameObject handbrakeButton;
    public GameObject retroButton;

    private Vector3[] originalPositions; // Array per memorizzare le posizioni originali dei pulsanti
    private Vector3[] originalScale; 
private bool isScaled = false;
    void Start()
    {
                string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "GameScene")
        {

        // Memorizza le posizioni originali dei pulsanti
        originalPositions = new Vector3[9];
        originalPositions[0] = joystick.transform.position;
        originalPositions[1] = abilityButton1.transform.position;
        originalPositions[2] = abilityButton2.transform.position;
        originalPositions[3] = abilityButton3.transform.position;
        originalPositions[4] = abilityButton4.transform.position;
        originalPositions[5] = abilityButton5.transform.position;
        originalPositions[6] = throttleButton.transform.position;
        originalPositions[7] = handbrakeButton.transform.position;
        originalPositions[8] = retroButton.transform.position;

        originalScale = new Vector3[9];
        originalScale[0] = joystick.transform.localScale;
        originalScale[1] = abilityButton1.transform.localScale;
        originalScale[2] = abilityButton2.transform.localScale;
        originalScale[3] = abilityButton3.transform.localScale;
        originalScale[4] = abilityButton4.transform.localScale;
        originalScale[5] = abilityButton5.transform.localScale;
        originalScale[6] = throttleButton.transform.localScale;
        originalScale[7] = handbrakeButton.transform.localScale;
        originalScale[8] = retroButton.transform.localScale;

        if ((float)Screen.width / Screen.height < aspectRatioThreshold )
        {
                    if (!isScaled)
        { 
            isScaled = true;
            // Rimpicciolisci e sposta il joystick
            if (joystick != null)
            {
                joystick.transform.localScale = joystick.transform.localScale * 0.6f; // Rimpicciolisci del 20%
                joystick.transform.position += new Vector3(-30.5f, -20f, 0f); // Sposta leggermente in basso a sinistra
            }

            // Rimpicciolisci e sposta gli altri pulsanti
            MoveAndResizeButton(abilityButton1, 0.6f, new Vector3(70.5f, -50f, 0f)); // Ability Buttons
            MoveAndResizeButton(abilityButton2, 0.6f, new Vector3(70.5f, -50f, 0f)); // Ability Buttons
            MoveAndResizeButton(abilityButton3, 0.6f, new Vector3(70.5f, -50f, 0f)); // Ability Buttons
            MoveAndResizeButton(abilityButton4, 0.6f, new Vector3(70.5f, -50f, 0f)); // Ability Buttons
            MoveAndResizeButton(abilityButton5, 0.6f, new Vector3(70.5f, -50f, 0f)); // Ability Buttons
            MoveAndResizeButton(throttleButton, 0.6f, new Vector3(40.5f, -50f, 0f)); // Throttle Button
            MoveAndResizeButton(handbrakeButton, 0.6f, new Vector3(70.5f, -25f, 0f)); // Handbrake Button
            MoveAndResizeButton(retroButton, 0.6f, new Vector3(40.5f, -10f, 0f)); // Retro Button
        }
        }
        else
        {
            isScaled = false;
            // Ripristina le dimensioni e le posizioni originali dei pulsanti
            joystick.transform.localScale = originalScale[0];
            joystick.transform.position = originalPositions[0];

            abilityButton1.transform.position = originalPositions[1];
            abilityButton2.transform.position = originalPositions[2];
            abilityButton3.transform.position = originalPositions[3];
            abilityButton4.transform.position = originalPositions[4];
            abilityButton5.transform.position = originalPositions[5];
            throttleButton.transform.position = originalPositions[6];
            handbrakeButton.transform.position = originalPositions[7];
            retroButton.transform.position = originalPositions[8];
            abilityButton1.transform.localScale = originalScale[1];
            abilityButton2.transform.localScale = originalScale[2];
            abilityButton3.transform.localScale = originalScale[3];
            abilityButton4.transform.localScale = originalScale[4];
            abilityButton5.transform.localScale = originalScale[5];
            throttleButton.transform.localScale = originalScale[6];
            handbrakeButton.transform.localScale = originalScale[7];
            retroButton.transform.localScale = originalScale[8];
        } 
    }
        if (sceneName == "SinglePlayerScene")
        {

            
        // Memorizza le posizioni originali dei pulsanti
        originalPositions = new Vector3[9];
        originalPositions[0] = joystick.transform.position;
        originalPositions[1] = abilityButton1.transform.position;
        originalPositions[2] = abilityButton2.transform.position;
        originalPositions[3] = abilityButton3.transform.position;
        originalPositions[4] = abilityButton4.transform.position;
        originalPositions[5] = abilityButton5.transform.position;
        originalPositions[6] = throttleButton.transform.position;
        originalPositions[7] = handbrakeButton.transform.position;
        originalPositions[8] = retroButton.transform.position;

        originalScale = new Vector3[9];
        originalScale[0] = joystick.transform.localScale;
        originalScale[1] = abilityButton1.transform.localScale;
        originalScale[2] = abilityButton2.transform.localScale;
        originalScale[3] = abilityButton3.transform.localScale;
        originalScale[4] = abilityButton4.transform.localScale;
        originalScale[5] = abilityButton5.transform.localScale;
        originalScale[6] = throttleButton.transform.localScale;
        originalScale[7] = handbrakeButton.transform.localScale;
        originalScale[8] = retroButton.transform.localScale;

        if ((float)Screen.width / Screen.height < aspectRatioThreshold )
        {
                    if (!isScaled)
        { 
            isScaled = true;
            // Rimpicciolisci e sposta il joystick
            if (joystick != null)
            {
                joystick.transform.localScale = joystick.transform.localScale * 0.95f;  // Rimpicciolisci del 20%
                joystick.transform.position += new Vector3(-0.02f, -0.02f, 0f); // Sposta leggermente in basso a sinistra
            }

            // Rimpicciolisci e sposta gli altri pulsanti
            MoveAndResizeButton(abilityButton1, 0.95f, new Vector3(30.5f, -10f, 0f)); // Ability Buttons
            MoveAndResizeButton(abilityButton2, 0.95f, new Vector3(30.5f, -10f, 0f)); // Ability Buttons
            MoveAndResizeButton(abilityButton3, 0.95f, new Vector3(30.5f, -10f, 0f)); // Ability Buttons
            MoveAndResizeButton(abilityButton4, 0.95f, new Vector3(30.5f, -10f, 0f)); // Ability Buttons
            MoveAndResizeButton(abilityButton5, 0.95f, new Vector3(30.5f, -10f, 0f)); // Ability Buttons
            MoveAndResizeButton(throttleButton, 0.95f, new Vector3(0.5f, -10f, 0f)); // Throttle Button
            MoveAndResizeButton(handbrakeButton, 0.95f, new Vector3(30.5f, 5f, 0f)); // Handbrake Button
            MoveAndResizeButton(retroButton, 0.95f, new Vector3(0.5f, 10f, 0f)); // Retro Button
        }
        }
        else
        {
            isScaled = false;
            // Ripristina le dimensioni e le posizioni originali dei pulsanti
            joystick.transform.localScale = originalScale[0];
            joystick.transform.position = originalPositions[0];

            abilityButton1.transform.position = originalPositions[1];
            abilityButton2.transform.position = originalPositions[2];
            abilityButton3.transform.position = originalPositions[3];
            abilityButton4.transform.position = originalPositions[4];
            abilityButton5.transform.position = originalPositions[5];
            throttleButton.transform.position = originalPositions[6];
            handbrakeButton.transform.position = originalPositions[7];
            retroButton.transform.position = originalPositions[8];
            abilityButton1.transform.localScale = originalScale[1];
            abilityButton2.transform.localScale = originalScale[2];
            abilityButton3.transform.localScale = originalScale[3];
            abilityButton4.transform.localScale = originalScale[4];
            abilityButton5.transform.localScale = originalScale[5];
            throttleButton.transform.localScale = originalScale[6];
            handbrakeButton.transform.localScale = originalScale[7];
            retroButton.transform.localScale = originalScale[8];
        } 
 }
   }
    void Update()
    {
       
 
    }

private void MoveAndResizeButton(GameObject button, float scaleFactor, Vector3 offset)
{
    if (button != null)
    {
        button.transform.localScale = button.transform.localScale * scaleFactor; // Rimpicciolisci
        button.transform.localPosition = button.transform.localPosition + offset; // Sposta relativamente alla posizione originale
    }
}

}
