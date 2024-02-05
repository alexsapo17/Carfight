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
    public float shockwaveRadius = 500f; // Raggio dell'onda d'urto
    public float shockwaveForce = 10000f; // Forza dell'onda d'urto



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
        // Salva i valori originali
    originalSize = transform.localScale;
    Rigidbody rb = GetComponent<Rigidbody>();
    if (rb != null)
    {
        originalMass = rb.mass;
    }
    photonView.RPC("ApplySizeMassIncrease", RpcTarget.All, duration, sizeMultiplier, massMultiplier);
}
}


[PunRPC]
void ApplySizeMassIncrease(float duration, float sizeMultiplier, float massMultiplier)
{

        effectActive = true;
        StartCoroutine(SizeMassIncreaseEffect(duration, sizeMultiplier, massMultiplier));
    
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
void ApplyControlDisable(float duration, int playerWhoPickedUpID)
{
if (photonView.ViewID != playerWhoPickedUpID)
{
StartCoroutine(DisableControlsForDuration(duration));
Debug.Log("[PlayerEffects] Disabilitazione controlli per " + gameObject.name);
}
else
{
Debug.Log("[PlayerEffects] Ignorato disabilitazione controlli per il giocatore che ha raccolto il pickup: " + gameObject.name);
}
}
// Funzione per avviare l'effetto di aumento di velocità e attrito
public void StartSpeedAndFrictionEffect(float duration, float accelerationMultiplier)
{
       if (!effectActive)
    {
    photonView.RPC("ApplySpeedAndFrictionIncrease", RpcTarget.All, duration, accelerationMultiplier);
}
}

[PunRPC]
void ApplySpeedAndFrictionIncrease(float duration, float accelerationMultiplier)
{
 
        effectActive = true;
        StartCoroutine(SpeedAndFrictionEffect(duration, accelerationMultiplier));
    
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
// Aggiungi questo metodo dentro PlayerEffects
public void StartJump(float jumpForce)
{
    if (photonView.IsMine) // Assicurati che solo il giocatore locale possa saltare
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null && IsGrounded()) // Implementa IsGrounded in base alla tua logica specifica
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}

    bool IsGrounded()
    {
        WheelCollider[] wheelColliders = GetComponentsInChildren<WheelCollider>();
        foreach (var wheel in wheelColliders)
        {
            if (wheel.isGrounded)
            {
                Debug.Log("IsGrounded: true");
                return true; // Almeno una ruota tocca il suolo
            }
        }
        Debug.Log("IsGrounded: false");
        return false; // Nessuna ruota tocca il suolo
    }

private IEnumerator DisableControlsForDuration(float duration)
{
    var carController = GetComponent<PrometeoCarController>();

    if (carController != null) 
    {
        carController.DisableControls();  // Usa la stessa logica di GameManager

        // Istanzia l'effetto di disabilitazione dei controlli
        Instantiate(controlDisableEffectPrefab, transform.position, Quaternion.identity);

        // Attendi per la durata specificata
        yield return new WaitForSeconds(duration);

        carController.EnableControls();  // Riabilita i controlli
        Debug.Log("[PlayerEffects] Riabilitazione controlli per " + gameObject.name);
    }
    else
    {
        Debug.LogError("[PlayerEffects] Nessun carController trovato su " + gameObject.name);
    }
}


    // Funzione da chiamare per attivare l'effetto dell'onda d'urto
    public void StartShockwaveEffect()
    {
        photonView.RPC("ApplyShockwaveEffect", RpcTarget.All);
    }

    [PunRPC]
    void ApplyShockwaveEffect()
    {
        // Ottieni tutti i collider entro il raggio dell'onda d'urto
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, shockwaveRadius);
        foreach (var hitCollider in hitColliders)
        {
            // Controlla se il collider appartiene a un "Player"
            if (hitCollider.CompareTag("Player") && hitCollider.gameObject != gameObject) // Escludi il giocatore che ha attivato l'onda d'urto
            {
                // Ottieni il componente Rigidbody del giocatore colpito
                Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Calcola la direzione della forza da applicare
                    Vector3 forceDirection = hitCollider.transform.position - transform.position;
                    rb.AddForce(forceDirection.normalized * shockwaveForce, ForceMode.Impulse);
                }
            }
        }

        // Opzionale: Puoi istanziare un effetto visivo per l'onda d'urto qui
    }
}
