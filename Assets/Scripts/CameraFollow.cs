using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public Transform carTransform;
    [Range(1, 10)]
    public float followSpeed = 2;
    [Range(1, 10)]
    public float lookSpeed = 5;

    [Header("Camera Offsets")]
    public Vector3 initialCameraPosition;
    public Vector3 absoluteInitCameraPosition;

    void Start()
    {
        if (carTransform == null)
        {
            Debug.LogError("Car Transform not assigned. Please assign it in the Inspector.");
            return;
        }

        // Inizializza gli offset
        initialCameraPosition = transform.position;
        absoluteInitCameraPosition = initialCameraPosition - carTransform.position;
    }

    void FixedUpdate()
    {
        // Look at car
        Vector3 lookDirection = carTransform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, lookSpeed * Time.deltaTime);

        // Move to car
        Vector3 targetPos = absoluteInitCameraPosition + carTransform.position;
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }

    // Funzione per regolare gli offset attraverso l'Inspector di Unity
    public void SetCameraOffsets(Vector3 newInitialCameraPosition, Vector3 newAbsoluteInitCameraPosition)
    {
        initialCameraPosition = newInitialCameraPosition;
        absoluteInitCameraPosition = newAbsoluteInitCameraPosition;
    }
}
