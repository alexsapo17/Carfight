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
    private List<Renderer> lastObstructingObjects = new List<Renderer>(); // Lista degli oggetti che ostruivano la vista

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
                // Raycasting per verificare se la vista è ostruita
        CheckObstructions();
    }

    // Funzione per regolare gli offset attraverso l'Inspector di Unity
    public void SetCameraOffsets(Vector3 newInitialCameraPosition, Vector3 newAbsoluteInitCameraPosition)
    {
        initialCameraPosition = newInitialCameraPosition;
        absoluteInitCameraPosition = newAbsoluteInitCameraPosition;
    }
void CheckObstructions()
    {
        // Ripristina la trasparenza degli oggetti precedentemente ostruenti
        foreach (Renderer r in lastObstructingObjects)
        {
            SetTransparency(r, 1.0f); // Imposta l'opacità completa
        }
        lastObstructingObjects.Clear();

        // Sparare un raggio dalla camera al personaggio
        RaycastHit[] hits;
        Vector3 direction = carTransform.position - transform.position;
        hits = Physics.RaycastAll(transform.position, direction, direction.magnitude);

        // Controlla se ci sono oggetti che ostruiscono la vista
        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null)
            {
                SetTransparency(rend, 0.5f); // Imposta la semi-trasparenza
                lastObstructingObjects.Add(rend);
            }
        }
    }

    void SetTransparency(Renderer renderer, float alpha)
    {
        if (renderer.material.HasProperty("_Color"))
        {
            Color color = renderer.material.color;
            color.a = alpha;
            renderer.material.color = color;
        }
        // Considera di aggiungere supporto per altri tipi di materiali/shader se necessario
    }
}
