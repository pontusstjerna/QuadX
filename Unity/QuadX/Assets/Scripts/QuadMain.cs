using UnityEngine;
using System.Collections;

public class QuadMain : MonoBehaviour {

    public float Sensitivity;
    public float ControllerInput;

    public float Kp;
    public float Ki;
    public float Kd;

    private Vector3[] engines;
    private Vector3[] engineVectors = new Vector3[]
    {
        new Vector3(4.5f, 0, 4.5f),
        new Vector3(4.5f, 0, -4.5f),
        new Vector3(-4.5f, 0, -4.5f),
        new Vector3(-4.5f, 0, 4.5f)
    };
    private enum engineNames { FRONT_RIGHT, REAR_RIGHT, REAR_LEFT, FRONT_LEFT};
    private Rigidbody body;

    private const int ENGINE_MAX_PWR = 7;

    private float pitch, roll, yaw;
    private float height = 5;
    
    private PID pidRoll;
    private PID pidPitch;
    private PID pidAlt;

    private int engineTestIndex = 0;

    private GameObject[] engineMarkers;

    // Use this for initialization
    void Start () {
        engines = new Vector3[4];

        body = GetComponent<Rigidbody>();

        pidRoll = new PID(Kp, Ki, Kd);
        pidPitch = new PID(Kp, Ki, Kd);
        pidAlt = new PID(0.5f, Ki, Kd);

        engineMarkers = new GameObject[] {
            GameObject.CreatePrimitive(PrimitiveType.Sphere),
            GameObject.CreatePrimitive(PrimitiveType.Sphere),
            GameObject.CreatePrimitive(PrimitiveType.Sphere),
            GameObject.CreatePrimitive(PrimitiveType.Sphere)
            };
    }
	
	// Update is called once per frame
	void Update () {

        //Quit if escape
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        CheckUserInp();

        //Switch between test engine, where 4 is off
        if (Input.GetKeyUp(KeyCode.T))
        {
            engineTestIndex = (engineTestIndex + 1) % 5;
            print("Engine: " + engineTestIndex);
        }

        CheckAdjustments();
    }

    void FixedUpdate()
    {
        pidRoll.Update(Get180(roll), Get180(body.rotation.eulerAngles.z), Time.deltaTime);
        pidPitch.Update(Get180(pitch), Get180(body.rotation.eulerAngles.x), Time.deltaTime);
        pidAlt.Update(height, body.transform.position.y, Time.deltaTime);

        UpdateEnginePositions();

        if(engineTestIndex > 0)
        {
            SetPwr(engineTestIndex-1, 0.7f);
        }

        SetMotors();
        PaintEngines();
    }

    private void CheckUserInp()
    {
        pitch = Input.GetAxis("pitch")* Sensitivity;
        roll = Input.GetAxis("roll")* Sensitivity;
        yaw = Input.GetAxis("yaw")* Sensitivity;
        height += Input.GetAxis("alt") * Time.deltaTime * Sensitivity / 3 ;
    }

    public Rigidbody GetBody()
    {
        return body;
    }

    private void SetMotors()
    {
        SetPwr(0, pidAlt.GetOutput() + pidRoll.GetOutput() / 2 - pidPitch.GetOutput() / 2);
        SetPwr(1, pidAlt.GetOutput() + pidRoll.GetOutput() / 2 + pidPitch.GetOutput() / 2);
        SetPwr(2, pidAlt.GetOutput() - pidRoll.GetOutput() / 2 + pidPitch.GetOutput() / 2);
        SetPwr(3, pidAlt.GetOutput() - pidRoll.GetOutput() / 2 - pidPitch.GetOutput() / 2);
    }

    private void SetPwr(int engineIndex, float thrust)
    {
        if(thrust > 1)
        {
            thrust = 1;
        }else if(thrust < -1)
        {
            thrust = -1;
        }
        body.AddForceAtPosition(transform.TransformDirection(Vector3.up) * ENGINE_MAX_PWR * thrust, engines[engineIndex]);
    }

    private void UpdateEnginePositions()
    {
        for (int i = 0; i < engines.Length; i++)
            engines[i] = transform.TransformPoint(engineVectors[i]);
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

    private void PaintEngines()
    {
        for(int i = 0; i < engineMarkers.Length; i++)
        {
            engineMarkers[i].transform.position = engines[i];
        }
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
}
