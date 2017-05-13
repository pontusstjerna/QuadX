using UnityEngine;
using System.Collections;

public partial class QuadMain : MonoBehaviour {

    public float Sensitivity;

    public float Kp;
    public float Ki;
    public float Kd;
    
    public Rigidbody body { get; private set;}

    private bool enginesOn = false;
    
    private Vector3[] engineVectors = new Vector3[]
    {
        new Vector3(4.3f, 0, 4.3f),
        new Vector3(4.3f, 0, -4.3f),
        new Vector3(-4.3f, 0, -4.3f),
        new Vector3(-4.3f, 0, 4.3f)
    };

    private Propeller[] propellers = new Propeller[4];

    private enum engineNames { FRONT_RIGHT, REAR_RIGHT, REAR_LEFT, FRONT_LEFT};
    
    private float pitch, roll, yaw;
    private float height = 5;
    
    private PID pidRoll;
    private PID pidPitch;
    private PID pidAlt;
    private PID pidYaw;

    // Use this for initialization
    void Start () {
        body = GetComponent<Rigidbody>();

        pidRoll = new PID(Kp, Ki, Kd);
        pidPitch = new PID(Kp, Ki, Kd);
        pidAlt = new PID(0.5f, Ki, 0.3f);
        pidYaw = new PID(0.5f, 0.0f, 0.5f);

        propellers[0] = GameObject.Find("propFrontLeft").GetComponent<Propeller>();
        propellers[1] = GameObject.Find("propFrontRight").GetComponent<Propeller>();
        propellers[2] = GameObject.Find("propRearRight").GetComponent<Propeller>();
        propellers[3] = GameObject.Find("propRearLeft").GetComponent<Propeller>();

        for(int i = 0; i < propellers.Length; i++)
        {
            propellers[i].SetPosition(transform.TransformPoint(engineVectors[i]), i);
        }

        Reset();
    }
	
	// Update is called once per frame
	void Update () {

        //Quit if escape
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        CheckUserInp();
        CheckAdjustments();
    }

    void FixedUpdate()
    {
        UpdatePIDs();
        SetMotors();
    }

    private void CheckUserInp()
    {
        pitch = Input.GetAxis("pitch")* Sensitivity;
        roll = Input.GetAxis("roll")* Sensitivity;
        yaw = Get180(yaw + Input.GetAxis("yaw") * Sensitivity * Time.deltaTime * 2);
        height = height + Input.GetAxis("alt") * Time.deltaTime * Sensitivity / 3;

        if (Input.GetKeyUp(KeyCode.Space))
        {
            enginesOn = !enginesOn;
            if (enginesOn) Reset();
        }

        print(yaw);
    }

    private void UpdatePIDs()
    {
        pidRoll.Update(Get180(roll), Get180(body.rotation.eulerAngles.z), Time.deltaTime);
        pidPitch.Update(Get180(pitch), Get180(body.rotation.eulerAngles.x), Time.deltaTime);
        pidAlt.Update(height, body.transform.position.y, Time.deltaTime);
        pidYaw.Update(yaw, Get180(body.transform.rotation.eulerAngles.y), Time.deltaTime);
    }

    private void SetMotors()
    {
        if (enginesOn)
        {
            propellers[0].SetPwr(pidAlt.GetOutput() + pidRoll.GetOutput() - pidPitch.GetOutput() + pidYaw.GetOutput());
            propellers[1].SetPwr(pidAlt.GetOutput() + pidRoll.GetOutput() + pidPitch.GetOutput() - pidYaw.GetOutput());
            propellers[2].SetPwr(pidAlt.GetOutput() - pidRoll.GetOutput() + pidPitch.GetOutput() + pidYaw.GetOutput());
            propellers[3].SetPwr(pidAlt.GetOutput() - pidRoll.GetOutput() - pidPitch.GetOutput() - pidYaw.GetOutput());
        }
        else
        {
            foreach(Propeller p in propellers)
            {
                p.SetPwr(0);
            }
        }
    }

    private float Get180(float angle)
    {
        angle = angle % 360;
        if (angle >= 180 && angle > 0)
        {
            return angle - 360;
        }
        else if (angle <= -180)
        {
            return angle + 360;
        }
        return angle;
    }

    private void CheckAdjustments()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyUp(KeyCode.P))
                pidRoll.Kp -= 0.1f;

            if (Input.GetKeyUp(KeyCode.I))
                pidRoll.Ki -= 0.01f;

            if (Input.GetKeyUp(KeyCode.O))
                pidRoll.Kd -= 0.1f;
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.P))
                pidRoll.Kp += 0.1f;

            if (Input.GetKeyUp(KeyCode.I))
                pidRoll.Ki += 0.01f;

            if (Input.GetKeyUp(KeyCode.O))
                pidRoll.Kd += 0.1f;
        }


        Kp = pidRoll.Kp;
        Ki = pidRoll.Ki;
        Kd = pidRoll.Kd;
    }

    private void Reset()
    {
        roll = 0;
        pitch = 0;
        yaw = body.rotation.eulerAngles.z;
        height = 0;
    }
}
