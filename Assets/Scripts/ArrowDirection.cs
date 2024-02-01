using UnityEngine;

public class ArrowDirection : MonoBehaviour
{
    public HorizontalJoystick joystick; // Riferimento al joystick
    private Transform target; // La macchina o l'oggetto che la freccia deve seguire

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetJoystick(HorizontalJoystick newJoystick)
    {
        joystick = newJoystick;
    }

  void Update()
{
    if (target == null) return;

    float horizontal = joystick.GetHorizontal();
    float vertical = joystick.GetVertical();

    // Ruota la direzione di 90 gradi attorno all'asse y
    Vector3 direction = Quaternion.Euler(0, 0, 0) * (target.transform.forward * vertical + target.transform.right * horizontal);

    if (direction.magnitude > 0.1f)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5);
    }

    transform.position = target.position + Vector3.up * 2;
}

}
