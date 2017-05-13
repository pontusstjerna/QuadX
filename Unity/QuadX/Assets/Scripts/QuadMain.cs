using UnityEngine;
using System.Collections;

public partial class QuadMain : MonoBehaviour {

    public float Sensitivity;

    public float Kp;
    public float Ki;
    public float Kd;

    public bool ShowEngineMarkers;

    public Rigidbody body { get; private set;}

    private Vector3[] enginePositions;
    private Vector3[] engineVectors = new Vector3[]
    {
        new Vector3(4.3f, 0, 4.3f),
        new Vector3(4.3f, 0, -4.3f),
        new Vector3(-4.3f, 0, -4.3f),
        new Vector3(-4.3f, 0, 4.3f)
    };

    private Vector3[] torques = new Vector3[]
    {
        new Vector3(0,0,-1),
        new Vector3(0,0,1)
    };

    private enum engineNames { FRONT_RIGHT, REAR_RIGHT, REAR_LEFT, FRONT_LEFT};

    private const int ENGINE_MAX_PWR = 7;

    private float pitch, roll, yaw;
    private float height = 5;
    
    private PID pidRoll;
    private PID pidPitch;
    private PID pidAlt;
    
    private GameObject[] engineMarkers;

    // Use this for initialization
    void Start () {
        enginePositions = new Vector3[4];

        body = GetComponent<Rigidbody>();

        pidRoll = new PID(Kp, Ki, Kd);
        pidPitch = new PID(Kp, Ki, Kd);
        pidAlt = new PID(0.5f, Ki, 0.3f);

        engineMarkers = new GameObject[] {
            GameObject.CreatePrimitive(PrimitiveType.Sphere),
            GameObject.CreatePrimitive(PrimitiveType.Sphere),
            GameObject.CreatePrimitive(PrimitiveType.Sphere),
            GameObject.CreatePrimitive(PrimitiveType.Sphere)
            };

        foreach (GameObject obj in engineMarkers)
        {
            obj.GetComponent<Collider>().enabled = false;
            obj.transform.localScale = body.transform.localScale;
        }
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
        UpdateEnginePositions();
        SetMotors();

        if(ShowEngineMarkers)
            PaintEngines();
    }

    private void CheckUserInp()
    {
        pitch = Input.GetAxis("pitch")* Sensitivity;
        roll = Input.GetAxis("roll")* Sensitivity;
        yaw = Input.GetAxis("yaw")* Sensitivity;
        height += Input.GetAxis("alt") * Time.deltaTime * Sensitivity / 3 ;
    }

    private void UpdatePIDs()
    {
        pidRoll.Update(Get180(roll), Get180(body.rotation.eulerAngles.z), Time.deltaTime);
        pidPitch.Update(Get180(pitch), Get180(body.rotation.eulerAngles.x), Time.deltaTime);
        pidAlt.Update(height, body.transform.position.y, Time.deltaTime);
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
        body.AddForceAtPosition(transform.TransformDirection(Vector3.up) * ENGINE_MAX_PWR * thrust, enginePositions[engineIndex]);
        AddTorqueAtPosition(torques[engineIndex % 2]*thrust*250, enginePositions[engineIndex]);
    }

    private void UpdateEnginePositions()
    {
        for (int i = 0; i < enginePositions.Length; i++)
            enginePositions[i] = transform.TransformPoint(engineVectors[i]);
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
            engineMarkers[i].transform.position = enginePositions[i];
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

    //TURNING
    private void AddTorqueAtPosition(Vector3 torque, Vector3 position)
    {
        //http://forum.unity3d.com/threads/torque-at-offset.187297/
        Vector3 torqueAxis = torque.normalized;
        Vector3 ortho = new Vector3(1, 0, 0);
        // prevent torqueAxis and ortho from pointing in the same direction
        if ((torqueAxis - ortho).sqrMagnitude < float.Epsilon)
        {
            ortho = new Vector3(0, 1, 0);
        }

        var orthoNorm = OrthoNormalize(torqueAxis, ortho);


        // calculate force
        Vector3 force = Vector3.Cross(0.5f * torque, orthoNorm);
        body.AddForceAtPosition(force, position + orthoNorm);
        body.AddForceAtPosition(-force, position - orthoNorm);
    }

    private Vector3 OrthoNormalize(Vector3 vector1, Vector3 vector2)
    {
        vector1.Normalize();
        Vector3 temp = Vector3.Cross(vector1, vector2);
        temp.Normalize();
        vector2 = Vector3.Cross(temp, vector2);
        return vector2;
        //var output = new Vector3[] {vector1, vector2};
        //return output;
    }
}
