using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using Photon.Pun;

public class PlayerEffects : MonoBehaviourPun
{
    private Vector3 originalSize;
    private float originalMass;
    private bool effectActive = false;
    private Material outlineMaterial;
    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    private int originalAccelerationMultiplier;
public GameObject collisionEffectPrefab;
    public GameObject controlDisableEffectPrefab;
    public GameObject sizeMassIncreaseEffectPrefab;
    public GameObject invisibilityEffectPrefab;
        void Start()
    {
        // Crea un materiale per l'outline
        outlineMaterial = new Material(Shader.Find("Unlit/Color"));
        outlineMaterial.color = Color.green; // Scegli il colore che preferisci
    }

    // Funzione per avviare l'effetto di aumento di grandezza/massa
    public void StartSizeMassIncreaseTimer(float duration, float sizeMultiplier, float massMultiplier)
    {
        if (!effectActive)
        {
            effectActive = true;
            originalSize = transform.localScale;
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null) originalMass = rb.mass;

            // Applica l'effetto
            ApplySizeMassIncrease(sizeMultiplier, massMultiplier);
            Instantiate(sizeMassIncreaseEffectPrefab, transform.position, Quaternion.identity);

            // Avvia il timer per la durata dell'effetto
            StartCoroutine(ResetSizeMassAfterDelay(duration));
        }
    }

    private void ApplySizeMassIncrease(float sizeMultiplier, float massMultiplier)
    {
        transform.localScale *= sizeMultiplier;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass *= massMultiplier;
        }
    }
    // Metodo da chiamare alla collisione
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Ottieni il punto di collisione
            Vector3 contactPoint = collision.contacts[0].point;

            // Attiva l'effetto FX nel punto di collisione
            Instantiate(collisionEffectPrefab, contactPoint, Quaternion.identity);
        }
    }
    IEnumerator ResetSizeMassAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Ripristina i valori originali
        transform.localScale = originalSize;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.mass = originalMass;

        effectActive = false;
    }

    // Funzione per avviare l'effetto di invisibilità
    public void StartInvisibilityTimer(float duration)
    {
        if (!effectActive)
        {
            effectActive = true;
            SetInvisibility(true);
            Instantiate(invisibilityEffectPrefab, transform.position, Quaternion.identity);

            // Avvia il timer per la durata dell'effetto
            StartCoroutine(ResetInvisibilityAfterDelay(duration));
        }
    }

     private void SetInvisibility(bool invisible)
    {
        if (photonView.IsMine)
        {
            // Applica il materiale outline se il player è controllato localmente
            ApplyOutlineEffect(invisible);
        }
        else
        {
            // Per gli altri giocatori, rendi il personaggio invisibile
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.enabled = !invisible;
            }
        }
    }

 private void ApplyOutlineEffect(bool apply)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            if (apply)
            {
                // Salva i materiali originali solo se non sono già stati salvati
                if (!originalMaterials.ContainsKey(renderer))
                {
                    originalMaterials[renderer] = renderer.materials;
                }

                // Aggiungi il materiale outline
                var materialsList = new List<Material>(renderer.materials) { outlineMaterial };
                renderer.materials = materialsList.ToArray();
            }
            else
            {
                // Ripristina i materiali originali, se presenti nel dizionario
                if (originalMaterials.TryGetValue(renderer, out Material[] savedMaterials))
                {
                    renderer.materials = savedMaterials;
                }
            }
        }
    }
// Aggiungi questo metodo allo script PlayerEffects

public void StartControlDisableTimer(float duration)
{
    StartCoroutine(DisableControlsForDuration(duration));
}
public void StartSpeedAndFrictionEffect(float duration, float accelerationMultiplier)
{
    if (!effectActive)
    {
        effectActive = true;
        var carController = GetComponent<PrometeoCarController>();
        if (carController != null)
        {
            originalAccelerationMultiplier = carController.accelerationMultiplier; // Memorizza il valore originale
        }
        ApplySpeedAndFrictionIncrease(accelerationMultiplier);
        StartCoroutine(ResetSpeedAndFrictionAfterDelay(duration));
    }
}
private void ApplySpeedAndFrictionIncrease(float accelerationMultiplier)
{
    var carController = GetComponent<PrometeoCarController>();
    if (carController != null)
    {
        carController.accelerationMultiplier = (int)(carController.accelerationMultiplier * accelerationMultiplier);
        carController.IncreaseTireFriction(accelerationMultiplier);
    }
}

IEnumerator ResetSpeedAndFrictionAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay);

    var carController = GetComponent<PrometeoCarController>();
    if (carController != null)
    {
        carController.accelerationMultiplier = originalAccelerationMultiplier; // Ripristina il valore originale
        carController.ResetTireFriction();
    }

    effectActive = false;
}


private IEnumerator DisableControlsForDuration(float duration)
{
    // Disabilita i controlli
    var controller = GetComponent<PrometeoCarController>();
    if (controller != null) controller.DisableControls();
            Instantiate(controlDisableEffectPrefab, transform.position, Quaternion.identity);


    yield return new WaitForSeconds(duration);

    // Riabilita i controlli
    if (controller != null) controller.EnableControls();
}

    IEnumerator ResetInvisibilityAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        SetInvisibility(false);
        effectActive = false;
    }
}
