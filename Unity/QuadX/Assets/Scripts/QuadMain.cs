using UnityEngine;
using System.Collections;

public class QuadMain : MonoBehaviour {

    public float Sensitivity;
    public float ControllerInput;
    public float Threshold;

    public float Kp;
    public float Ki;
    public float Kd;

    private Vector3[] engines;
    private enum engineNames { FRONT_RIGHT, REAR_RIGHT, REAR_LEFT, FRONT_LEFT};
    private Rigidbody body;

    private const int ENGINE_MAX_PWR = 7;
    private const int PITCH_SENSITIVITY = 0;
    private const int ROLL_SENSITIVITY = 1;

    private float pitch, roll, yaw;
    private float height = 5;
    
    private PID pidController;

    private int engineTestIndex = 0;

	// Use this for initialization
	void Start () {
        engines = new Vector3[4];
        engines[0] = transform.TransformPoint(new Vector3(4.5f, 0, 4.5f));
        engines[1] = transform.TransformPoint(new Vector3(4.5f, 0, -4.5f));
        engines[2] = transform.TransformPoint(new Vector3(-4.5f, 0, -4.5f));
        engines[3] = transform.TransformPoint(new Vector3(-4.5f, 0, 4.5f));

        body = GetComponent<Rigidbody>();

        pidController = new PID(Kp, Ki, Kd, Threshold);
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
        
    }

    void FixedUpdate()
    {
        if(engineTestIndex > 0)
        {
            SetPwr(engineTestIndex-1, 0.7f);
        }

        SetMotors();
    }

    private void CheckUserInp()
    {
        pitch = Input.GetAxis("pitch")* Sensitivity;
        roll = Input.GetAxis("roll")* Sensitivity;
        yaw = Input.GetAxis("yaw")* Sensitivity;
        height += Input.GetAxis("alt");

        //print(pitch);
    }

    public Rigidbody GetBody()
    {
        return body;
    }

    private void SetMotors()
    {
        print(pidController.GetOutput(0, Get180(body.rotation.eulerAngles.z), Time.deltaTime));

        SetPwr(0, GetThrottle(height) + RollPid(roll)/2 - PitchPid(pitch)/2 + GetControlRoll()/2 - GetControlPitch()/2);
        SetPwr(1, GetThrottle(height) + RollPid(roll)/2 + PitchPid(pitch)/2 + GetControlRoll()/2 + GetControlPitch()/2);
        SetPwr(2, GetThrottle(height) - RollPid(roll)/2 + PitchPid(pitch)/2 - GetControlRoll()/2 + GetControlPitch()/2);
        SetPwr(3, GetThrottle(height) - RollPid(roll)/2 - PitchPid(pitch)/2 - GetControlRoll()/2 - GetControlPitch()/2);
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

    private float GetThrottle(float alt)
    {
        float pwr = 0;

        if (body.position.y < alt && body.velocity.y > 0)
        {
            pwr = (alt - body.position.y) / 4;
        }
        else if (body.position.y < alt && body.velocity.y < 0)
        {
            pwr = 1;
        }
        else
        {
            //Smooth out
            //pwr = 0.5f / (body.position.y - alt);
        }

        return pwr;
    }

    private float RollPid(float degrees)
    {
        return pidController.GetOutput(Get180(degrees), Get180(body.rotation.eulerAngles.z), Time.deltaTime);
    }

    private float PitchPid(float degrees)
    {
        return pidController.GetOutput(Get180(degrees), Get180(body.rotation.eulerAngles.x), Time.deltaTime);
    }

    private float GetControlRoll()
    {
        return roll*ControllerInput;
    }

    private float GetControlPitch()
    {
        return pitch*ControllerInput;
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
}
