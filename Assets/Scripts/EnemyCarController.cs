using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyCarController : MonoBehaviour
{
    private Transform target; // Ora è privata
    public WheelCollider[] wheelColliders = new WheelCollider[4];
    public float maxMotorTorque = 1000f;
    public float maxSteeringAngle = 30f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        AssignWheelColliders();
        FindPlayerTarget(); // Trova il giocatore all'avvio
    }

    void AssignWheelColliders()
    {
        WheelCollider[] colliders = GetComponentsInChildren<WheelCollider>();
        for (int i = 0; i < colliders.Length && i < wheelColliders.Length; i++)
        {
            wheelColliders[i] = colliders[i];
        }
    }

    void FindPlayerTarget()
    {
        GameObject playerCar = GameObject.FindGameObjectWithTag("Player");
        if (playerCar != null)
        {
            target = playerCar.transform;
        }
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            FindPlayerTarget(); // Cerca di nuovo il target se non è stato trovato
            if (target == null) return; // Se ancora non c'è, esce
        }

        FollowTarget();
    }

    void FollowTarget()
    {
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
}
