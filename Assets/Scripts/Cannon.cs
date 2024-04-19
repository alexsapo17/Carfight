using UnityEngine;

public class Cannon : MonoBehaviour
{
    public GameObject cannonBallPrefab; // Prefab della palla di cannone
    public GameObject cannonFXPrefab; // Prefab dell'effetto visivo del cannone
    public Transform firePoint; // Punto di fuoco del cannone
    public float shootForce = 1000f; // Forza di sparare della palla
    public float shootAngleX = 45f; // Angolo di sparare X della palla
    public float shootAngleY = 45f; // Angolo di sparare Y della palla
    public float cannonBallSize = 1f; // Dimensione della palla di cannone
    public float cannonFXSize = 1f; // Dimensione dell'effetto visivo del cannone
    public float destroyDelay = 3f; // Tempo prima di distruggere la palla di cannone e l'effetto visivo

    // Visualizza la direzione di sparare nel Editor di Unity
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 shootDirection = Quaternion.Euler(shootAngleY, shootAngleX, 0f) * transform.forward;
        Gizmos.DrawLine(firePoint.position, firePoint.position + shootDirection * 2f); // Visualizza la direzione di sparare
    }

    public void FireCannon()
    {
        // Calcola la direzione di sparare con gli angoli specificati
        Vector3 shootDirection = Quaternion.Euler(shootAngleY, shootAngleX, 0f) * transform.forward;

        // Istanza della palla di cannone al punto di fuoco
        GameObject cannonBall = Instantiate(cannonBallPrefab, firePoint.position, Quaternion.identity);

        // Istanza dell'effetto visivo del cannone
        GameObject cannonFX = Instantiate(cannonFXPrefab, firePoint.position, Quaternion.identity);
        cannonFX.transform.localScale = Vector3.one * cannonFXSize; // Imposta la dimensione dell'effetto visivo

        // Imposta la dimensione della palla di cannone
        cannonBall.transform.localScale = Vector3.one * cannonBallSize;

        // Applica una forza alla palla di cannone nella direzione specificata
        cannonBall.GetComponent<Rigidbody>().AddForce(shootDirection * shootForce);

        // Distrugge la palla di cannone e l'effetto visivo dopo un certo periodo di tempo
        Destroy(cannonBall, destroyDelay);
        Destroy(cannonFX, destroyDelay);
    }
}
