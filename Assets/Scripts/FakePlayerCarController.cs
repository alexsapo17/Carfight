using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class FakePlayerCarController : MonoBehaviour
{
    public WheelCollider[] wheelColliders = new WheelCollider[4];
    public float maxMotorTorque = 1000f;
    public float maxSteeringAngle = 30f;
    public float maxSpeed = 50f;
    private Rigidbody rb;
    public bool raceStarted = false;

    // Waypoints
    public List<Transform>[] waypointPaths; // Array di liste di waypoint 
    private List<Transform> currentPath; // Il percorso attuale scelto
    private int currentWaypointIndex = 0; // Indice del waypoint corrente nel percorso

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        AssignWheelColliders();
        rb.centerOfMass += new Vector3(0, -0.5f, 0); // Regola il valore Y per abbassare ulteriormente il baricentro

        ChooseRandomPath(); // Scegli un percorso casuale all'inizio
    }

    void AssignWheelColliders()
    {
        WheelCollider[] colliders = GetComponentsInChildren<WheelCollider>();
        for (int i = 0; i < colliders.Length && i < wheelColliders.Length; i++)
        {
            wheelColliders[i] = colliders[i];
        }
    }

void ChooseRandomPath()
{
    if (WaypointManager.Instance.waypointPaths.Count > 0) // Usa .Count qui per le liste
    {
        int pathIndex = Random.Range(0, WaypointManager.Instance.waypointPaths.Count); // Usa .Count anche qui
        currentPath = WaypointManager.Instance.waypointPaths[pathIndex].waypoints; // Assicurati che waypoints sia accessibile
        currentWaypointIndex = 0;
    }
}

void FixedUpdate()
{
    if (!raceStarted || currentPath == null) return;

    Vector3 targetWaypoint = currentPath[currentWaypointIndex].position;
    Vector3 directionToTarget = targetWaypoint - transform.position;
    directionToTarget.y = 0; // Ignora l'asse Y per il calcolo della sterzata

    float steering = 0f;
    if (directionToTarget.magnitude > 1f) // Assicurati che la direzione sia valida
    {
        // Calcola l'angolo tra la direzione della macchina e la direzione del waypoint
        float angleToTarget = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);

        if (angleToTarget > 5f) // Se il waypoint è a destra
        {
            steering = maxSteeringAngle; // Sterza a destra
        }
        else if (angleToTarget < -5f) // Se il waypoint è a sinistra
        {
            steering = -maxSteeringAngle; // Sterza a sinistra
        }
    }

    ApplySteering(steering);

    // Graduale riduzione della velocità quando ci avviciniamo al waypoint
    float distanceToTarget = directionToTarget.magnitude;
    float brakeTorque = 0f;
    if (distanceToTarget < 10f)
    {
        brakeTorque = maxMotorTorque * (10f - distanceToTarget) / 10f; // Gradualmente aumenta il brakeTorque man mano che ci si avvicina al waypoint
    }
    ApplyBrakeTorque(brakeTorque);

    if (distanceToTarget < 15f) // Se vicini al waypoint, passa al prossimo
    {
        currentWaypointIndex++;
        if (currentWaypointIndex >= currentPath.Count)
        {
            // Se hai raggiunto l'ultimo waypoint, ricomincia il giro dal primo
            currentWaypointIndex = 0;
        }
    }

    if (rb.velocity.magnitude < maxSpeed)
    {
        ApplyMotorTorque(maxMotorTorque); // Applica torque solo se sotto la velocità massima
    }
    else
    {
        ApplyMotorTorque(0); // Non applicare torque se alla velocità massima
    }
}


void ApplyBrakeTorque(float torque)
{
    foreach (var wheelCollider in wheelColliders)
    {
        wheelCollider.brakeTorque = torque;
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
}
