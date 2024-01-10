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
        // Salva i valori originali
    originalSize = transform.localScale;
    Rigidbody rb = GetComponent<Rigidbody>();
    if (rb != null)
    {
        originalMass = rb.mass;
    }
    photonView.RPC("ApplySizeMassIncrease", RpcTarget.All, duration, sizeMultiplier, massMultiplier);

}


[PunRPC]
void ApplySizeMassIncrease(float duration, float sizeMultiplier, float massMultiplier)
{
    if (!effectActive)
    {
        effectActive = true;
        StartCoroutine(SizeMassIncreaseEffect(duration, sizeMultiplier, massMultiplier));
    }
}
IEnumerator SizeMassIncreaseEffect(float duration, float sizeMultiplier, float massMultiplier)
{
        Instantiate(sizeMassIncreaseEffectPrefab, transform.position, Quaternion.identity);

    transform.localScale *= sizeMultiplier;
    Rigidbody rb = GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.mass *= massMultiplier;
    }

    // Attendi la durata dell'effetto
       yield return new WaitForSeconds(duration);

        // Ripristina i valori originali
        transform.localScale = originalSize;
        if (rb != null) rb.mass = originalMass;

        effectActive = false;
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


// Funzione per avviare l'effetto di invisibilità
public void StartInvisibilityTimer(float duration)
{
    photonView.RPC("ApplyInvisibility", RpcTarget.All, duration);
}

[PunRPC]
void ApplyInvisibility(float duration)
{
    StartCoroutine(InvisibilityEffect(duration));
}

IEnumerator InvisibilityEffect(float duration)
{
        Instantiate(invisibilityEffectPrefab, transform.position, Quaternion.identity);

    SetInvisibility(true);
    yield return new WaitForSeconds(duration);
    SetInvisibility(false);
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


public void StartControlDisableTimer(float duration)
{
    photonView.RPC("ApplyControlDisable", RpcTarget.All, duration);
}

[PunRPC]
void ApplyControlDisable(float duration)
{
    StartCoroutine(DisableControlsForDuration(duration));
}

// Funzione per avviare l'effetto di aumento di velocità e attrito
public void StartSpeedAndFrictionEffect(float duration, float accelerationMultiplier)
{
    photonView.RPC("ApplySpeedAndFrictionIncrease", RpcTarget.All, duration, accelerationMultiplier);
}

[PunRPC]
void ApplySpeedAndFrictionIncrease(float duration, float accelerationMultiplier)
{
    if (!effectActive)
    {
        effectActive = true;
        StartCoroutine(SpeedAndFrictionEffect(duration, accelerationMultiplier));
    }
}

IEnumerator SpeedAndFrictionEffect(float duration, float accelerationMultiplier)
{
    var carController = GetComponent<PrometeoCarController>();
    if (carController != null)
    {
        carController.accelerationMultiplier = (int)(carController.accelerationMultiplier * accelerationMultiplier);
        carController.IncreaseTireFriction(accelerationMultiplier);
    }

    // Attendi la durata dell'effetto
    yield return new WaitForSeconds(duration);

    // Ripristina i valori originali
    if (carController != null)
    {
        carController.accelerationMultiplier = originalAccelerationMultiplier;
        carController.ResetTireFriction();
    }

    effectActive = false;
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
    var carController = GetComponent<PrometeoCarController>();

    if (carController != null) 
    {
        // Disabilita i controlli impostando enableControls su false
        carController.controlsEnabled = false;

        // Istanzia l'effetto di disabilitazione dei controlli
        Instantiate(controlDisableEffectPrefab, transform.position, Quaternion.identity);

        // Attendi per la durata specificata
        yield return new WaitForSeconds(duration);

        // Riabilita i controlli impostando enableControls su true
        carController.controlsEnabled = true;
    }
}



}
