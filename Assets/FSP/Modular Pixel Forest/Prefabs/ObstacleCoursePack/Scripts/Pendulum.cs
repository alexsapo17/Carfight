using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MonoBehaviour
{
	public float speed = 1.5f;
	public float limit = 75f; //Limit in degrees of the movement
	public bool randomStart = false; //If you want to modify the start position
	private float random = 0;
	public PendulumAxis oscillationAxis = PendulumAxis.ZAxis;

public enum PendulumAxis
{
    XAxis,
    ZAxis
}

	// Start is called before the first frame update
	void Awake()
    {
		if(randomStart)
			random = Random.Range(0.8f, 1f);
	}

    // Update is called once per frame
void Update()
{
    float angle = limit * Mathf.Sin((Time.time * speed) + random);

    // Usa l'enum oscillationAxis per determinare lungo quale asse oscillare
    switch (oscillationAxis)
    {
        case PendulumAxis.XAxis:
            transform.localRotation = Quaternion.Euler(angle, 0, 0);
            break;
        case PendulumAxis.ZAxis:
            transform.localRotation = Quaternion.Euler(0, 0, angle);
            break;
    }
}


}
