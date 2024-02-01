using UnityEngine;
using Photon.Pun;

public class ArrowDirection : MonoBehaviourPun
{
    public HorizontalJoystick joystick; // Riferimento al joystick
    private Transform target; // La macchina o l'oggetto che la freccia deve seguire

    void Update()
    {
        if (!photonView.IsMine || target == null) return;

        float horizontal = joystick.GetHorizontal();
        float vertical = joystick.GetVertical();

        Vector3 direction = Quaternion.Euler(0, 0, 0) * (target.transform.forward * vertical + target.transform.right * horizontal);

        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5);
        }

        transform.position = target.position + Vector3.up * 2; // Mantieni la freccia sopra al target
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetJoystick(HorizontalJoystick newJoystick)
    {
        joystick = newJoystick;
    }
}
