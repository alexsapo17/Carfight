using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class FakePlayerCarController : MonoBehaviour
{
    private Transform target;
    public WheelCollider[] wheelColliders = new WheelCollider[4];
    public float maxMotorTorque = 1000f;
    public float maxSteeringAngle = 30f;
    public bool raceStarted = false;
    private Rigidbody rb;
    public float avoidanceStrength = 30f;
    public float raycastDistance = 5f;
public float maxSpeed = 50f;
    private Coroutine currentRoutine = null;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        AssignWheelColliders();
    }

    void AssignWheelColliders()
    {
        WheelCollider[] colliders = GetComponentsInChildren<WheelCollider>();
        for (int i = 0; i < colliders.Length && i < wheelColliders.Length; i++)
        {
            wheelColliders[i] = colliders[i];
        }
    }


    bool IsGroundAhead()
    {
        RaycastHit hit;
        Vector3 position = transform.position + transform.forward * 2; // Regola questo offset in base alla dimensione della tua macchina
        bool hasGround = Physics.Raycast(position, -Vector3.up, out hit, 2f);
        Debug.DrawRay(position, -Vector3.up * 2, hasGround ? Color.green : Color.red);
        return hasGround;
    }
void FixedUpdate()
{
    FindClosestTarget();

    if (target == null || !raceStarted) return;

    if (!CheckForEdgeAndAdjust())
    {
        FollowTarget();
    }
        if (rb.velocity.magnitude > maxSpeed)
    {
        // Normalizza il vettore velocità per mantenerne la direzione e moltiplicalo per la velocità massima
        rb.velocity = rb.velocity.normalized * maxSpeed;
    }
}

bool CheckForEdgeAndAdjust()
{
    Vector3[] directions = new Vector3[]{
        transform.forward,
        (transform.forward + transform.right).normalized,
        (transform.forward - transform.right).normalized
    };

    bool edgeDetected = false;

    foreach (var dir in directions)
    {
        if (!IsGroundInDirection(dir))
        {
            edgeDetected = true;
            break;
        }
    }

    if (edgeDetected && currentRoutine == null)
    {
        currentRoutine = StartCoroutine(AdjustCourse());
        return true;
    }
    else if (!edgeDetected && currentRoutine != null)
    {
        StopCoroutine(currentRoutine);
        currentRoutine = null;
        ApplyMotorTorque(maxMotorTorque);
    }

    return edgeDetected;
}

bool IsGroundInDirection(Vector3 direction)
{
    // Calcola un offset in avanti basato sulla velocità attuale della macchina e una costante di tua scelta.
    float forwardOffset = 10.0f; // Ad esempio, 5 metri davanti alla macchina. Ajusta questo valore come necessario.

    // Calcola la posizione di partenza del raycast
    Vector3 raycastStart = transform.position + transform.forward * forwardOffset + transform.up * 0.2f; // Alza leggermente il punto di partenza per evitare collisioni con il terreno

    // Direzione del raycast verso il basso per rilevare il terreno
    Vector3 raycastDirection = -transform.up;

    RaycastHit hit;
    bool hasGround = Physics.Raycast(raycastStart, raycastDirection, out hit, 4f); // 2f è la distanza del raycast verso il basso
    Debug.DrawRay(raycastStart, raycastDirection * 2f, hasGround ? Color.green : Color.red); // Visualizza il raycast nell'editor

    return hasGround;
}


IEnumerator AdjustCourse()
{
    // Adjust the car's course. This might include steering adjustments, speed reduction, or a combination.
    while (!IsGroundAhead())
    {
        // Example: steer away from the edge
        ApplySteering(maxSteeringAngle * (Random.Range(0, 2) * 2 - 1)); // Randomly choose left or right
        ApplyMotorTorque(maxMotorTorque * 0.2f); // Slow down to give time for adjustments
        yield return new WaitForFixedUpdate();
    }
}


    void ApplyMotorTorque(float torque)
    {
        foreach (var wheelCollider in wheelColliders)
        {
            wheelCollider.motorTorque = torque;
        }
    }

    void ApplySteering(float steeringAngle)
    {
        wheelColliders[0].steerAngle = steeringAngle;
        wheelColliders[1].steerAngle = steeringAngle;
    }

    void FollowTarget()
    {
        if (!raceStarted) return;

        Vector3 relativeVector = transform.InverseTransformPoint(target.position.x, transform.position.y, target.position.z);
        float steering = (relativeVector.x / relativeVector.magnitude) * maxSteeringAngle;
        float motor = maxMotorTorque;

        wheelColliders[0].steerAngle = steering;
        wheelColliders[1].steerAngle = steering;

        foreach (var wheelCollider in wheelColliders)
        {
            wheelCollider.motorTorque = motor;
        }
    }

    void FindClosestTarget()
    {
        // Unisci gli array di GameObject con tag "Player" e "Enemy"
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<GameObject> allTargets = new List<GameObject>();
        allTargets.AddRange(players);
        allTargets.AddRange(enemies);

        // Rimuove sé stesso dalla lista di potenziali target
        allTargets.Remove(gameObject);

        // Trova il GameObject più vicino
        float closestDistance = Mathf.Infinity;
        GameObject closestTarget = null;

        foreach (GameObject potentialTarget in allTargets)
        {
            if (potentialTarget == gameObject) continue; // Ignora sé stesso

            float distance = (potentialTarget.transform.position - transform.position).sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = potentialTarget;
            }
        }

        // Imposta il target più vicino come bersaglio
        target = closestTarget?.transform;
    }
    
    
}
