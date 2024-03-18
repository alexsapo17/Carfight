using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using Photon.Pun;

public class PlayerEffects : MonoBehaviourPun
{
    private bool effectActive = false;
    private Material outlineMaterial;
    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    public GameObject invisibilityEffectPrefab;
    private float shockwaveRadius = 10f; // Raggio dell'onda d'urto
    private float shockwaveForce = 5000f; // Forza dell'onda d'urto
  private Vector3 cubeRelativeOffset;
public GameObject collisionEffectPrefab;



        void Start()
    {
        // Crea un materiale per l'outline
        outlineMaterial = new Material(Shader.Find("Unlit/Color"));
        outlineMaterial.color = Color.green; // Scegli il colore che preferisci
    }



    // Metodo da chiamare alla collisione
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") )
        {
            // Ottieni il punto di collisione
            Vector3 contactPoint = collision.contacts[0].point;

            // Attiva l'effetto FX nel punto di collisione
            Instantiate(collisionEffectPrefab, contactPoint, Quaternion.identity);
        }
                if (collision.gameObject.CompareTag("Enemy") )
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
    float groundCheckDistance = 2f; // Distanza di controllo dal suolo, regolabile a seconda delle esigenze

    foreach (var wheel in wheelColliders)
    {
        RaycastHit hit;
        // Calcola la distanza effettiva dal suolo partendo dal centro della ruota
        float currentDistance = wheel.suspensionDistance + wheel.radius;
        // Esegue un raycast per verificare la distanza effettiva dal suolo
        if (Physics.Raycast(wheel.transform.position, -Vector3.up, out hit, currentDistance + groundCheckDistance))
        {
            Debug.Log("IsGrounded: true, wheel: " + wheel.name);
            return true; // La ruota è considerata a terra se il raycast colpisce il suolo entro la distanza specificata
        }
    }

    Debug.Log("IsGrounded: false");
    return false; // Nessuna ruota è abbastanza vicina al suolo
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
        // Controlla se il collider appartiene a un "Player" o "Enemy"
        if ((hitCollider.CompareTag("Player") || hitCollider.CompareTag("Enemy")) && hitCollider.gameObject != gameObject) 
        {
            // Ottieni il componente Rigidbody del giocatore colpito
            Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Calcola la direzione della forza da applicare
                Vector3 forceDirection = hitCollider.transform.position - transform.position;
                // Aggiungi una componente verticale alla forza
                forceDirection += Vector3.up * shockwaveForce * 0.5f ; // Aggiusta il moltiplicatore per controllare l'intensità della spinta verticale
                // Aggiungi una componente orizzontale opposta alla direzione del giocatore che ha attivato l'onda d'urto
                forceDirection += (hitCollider.transform.position - transform.position).normalized * shockwaveForce ; 

                rb.AddForce(forceDirection.normalized * shockwaveForce, ForceMode.Impulse);
            }
        }
    }

    // Opzionale: Puoi istanziare un effetto visivo per l'onda d'urto qui
}

public void StartFlashbangEffect()
{
    photonView.RPC("ActivateFlashEffect", RpcTarget.Others);
}

 [PunRPC]
void ActivateFlashEffect()
{
    // Questo metodo verrà eseguito su tutti i client eccetto quello che ha invocato l'effetto.

    FlashbangManager.Instance.TriggerFlashbangEffect(0.2f, 1f); // Durata e tempo di fade out sono esempi
}
public void ActivateCubeEffect(GameObject cubePrefab, float cubeDuration, Vector3 offsetDistance, Vector3 cubeScale)
{
    // Calcola la posizione iniziale del cubo vicino al giocatore
    Vector3 cubePosition = transform.position + transform.forward * offsetDistance.z;

    // Calcola l'offset relativo del cubo rispetto al giocatore
    cubeRelativeOffset = cubePosition - transform.position;

    // Istanza il cubo vicino al giocatore
    GameObject cubeInstance = Instantiate(cubePrefab, cubePosition, Quaternion.identity);

    // Applica la scala al cubo
    cubeInstance.transform.localScale = cubeScale;

    // Avvia una coroutine per distruggere il cubo dopo un certo periodo di tempo
    StartCoroutine(DestroyCubeAfterDelay(cubeInstance, cubeDuration));

    // Avvia una coroutine per far seguire il giocatore al cubo
    StartCoroutine(FollowPlayer(cubeInstance));
}


private IEnumerator FollowPlayer(GameObject cube)
{
    while (true)
    {
        // Calcola la posizione desiderata del cubo mantenendo l'offset relativo rispetto al giocatore
        Vector3 targetPosition = transform.position + cubeRelativeOffset;

        // Imposta direttamente la posizione del cubo alla posizione desiderata
        cube.transform.position = targetPosition;

        // Imposta direttamente la rotazione del cubo alla rotazione del giocatore
        cube.transform.rotation = transform.rotation;

        yield return null; // Attendi fino al prossimo frame
    }
}



private IEnumerator DestroyCubeAfterDelay(GameObject cube, float delay)
{
    yield return new WaitForSeconds(delay);

    // Distruggi il cubo dopo il periodo di tempo specificato
    if (cube != null)
    {
        Destroy(cube);
    }
}

}