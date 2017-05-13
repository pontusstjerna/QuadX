using UnityEngine;
using System.Collections;

public class Propeller : MonoBehaviour {

    Rigidbody body;
    private Vector3[] torques = new Vector3[]
    {
        new Vector3(0,1,0),
        new Vector3(0,-1,0)
    };

    private const int MAX_ENGINE_PWR = 7;
    private int engineIndex;
    private float thrust = 1;

    // Use this for initialization
    void Start () {
        body = GetComponent<Rigidbody>();
        body.maxAngularVelocity = 40;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {

        body.AddRelativeTorque(torques[engineIndex % 2]*thrust*MAX_ENGINE_PWR*20);
    }

    public void SetPosition(Vector3 position, int engineIndex)
    {
        transform.position = position;
        this.engineIndex = engineIndex;
    }

    public void SetPwr(float thrust)
    {
        this.thrust = thrust;
        if (thrust > 1)
        {
            thrust = 1;
        }
        else if (thrust < -1)
        {
            thrust = -1;
        }
        body.AddForce(transform.TransformDirection(Vector3.up) * MAX_ENGINE_PWR * thrust);
    }
}
