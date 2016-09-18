using UnityEngine;
using System.Collections;

public class QuadMain : MonoBehaviour {

    public float Sensitivity;

    public float Kp;
    public float Ki;
    public float Kd;

    public bool laughAtMe = false;

    private Vector3[] engines;
    private Rigidbody body;

    private const int ENGINE_MAX_PWR = 7;
    private const int PITCH_SENSITIVITY = 0;
    private const int ROLL_SENSITIVITY = 1;

    private float pitch, roll, yaw;
    private float height = 5;
    
    private PID pidController;

    private int engineTestI = 4;

	// Use this for initialization
	void Start () {
        engines = new Vector3[4];
        engines[0] = transform.TransformPoint(new Vector3(4.5f, 0, 4.5f));
        engines[1] = transform.TransformPoint(new Vector3(4.5f, 0, -4.5f));
        engines[2] = transform.TransformPoint(new Vector3(-4.5f, 0, -4.5f));
        engines[3] = transform.TransformPoint(new Vector3(-4.5f, 0, 4.5f));

        body = GetComponent<Rigidbody>();

        pidController = new global::PID(Kp, Ki, Kd);
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
            engineTestI = (engineTestI + 1) % 5;
            print("Engine: " + engineTestI);
        }
        
    }

    void FixedUpdate()
    {
        if(engineTestI < 4)
        {
            SetPwr(engineTestI, 1);
        }


        /*  if (!laughAtMe)
          {
              AltHold(height);
              KeepPitch(pitch);
              KeepRoll(roll);
          }
          else
          {
              AltHold(5);
              NaiveKeepPitch(0);
              NaiveKeepRoll(0);
          }*/

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

    private void SetMotors()
    {
        /*
         * engines.setEngineSpeed(LEFT_FRONT_MOTOR, throttle - rollAdjust - pitchAdjust + headingAdjust);
      engines.setEngineSpeed(RIGHT_FRONT_MOTOR, throttle + rollAdjust - pitchAdjust - headingAdjust);
      engines.setEngineSpeed(LEFT_REAR_MOTOR, throttle - rollAdjust + pitchAdjust - headingAdjust);
      engines.setEngineSpeed(RIGHT_REAR_MOTOR, throttle + rollAdjust + pitchAdjust + headingAdjust);
         */

        print(pidController.GetError(Get180(roll), Get180(body.rotation.eulerAngles.z), Time.deltaTime));

        SetPwr(0, GetThrottle(height) + RollPid(roll) - PitchPid(pitch));
        SetPwr(1, GetThrottle(height) + RollPid(roll) + PitchPid(pitch));
        SetPwr(2, GetThrottle(height) - RollPid(roll) + PitchPid(pitch));
        SetPwr(3, GetThrottle(height) - RollPid(roll) - PitchPid(roll));
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

    private void KeepAlt(float height)
    {
        float output = pidController.Output(height, body.position.y, Time.deltaTime);

        SetPwr(0, output);
        SetPwr(3, output);
        SetPwr(1, output);
        SetPwr(2, output);
    }

    private void KeepPitch(float degrees)
    {    
        float output = pidController.Output(Get180(degrees), Get180(body.rotation.eulerAngles.x), Time.deltaTime);

        SetPwr(0, -output);
        SetPwr(3, -output);
        SetPwr(1, output);
        SetPwr(2, output);
    }

    private void KeepRoll(float degrees)
    {
        float output = pidController.Output(Get180(degrees), Get180(body.rotation.eulerAngles.z), Time.deltaTime);

        SetPwr(0, output);
        SetPwr(1, output);
        SetPwr(2, -output);
        SetPwr(3, -output);
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
        return pidController.Output(Get180(degrees), Get180(body.rotation.eulerAngles.z), Time.deltaTime);
    }

    private float PitchPid(float degrees)
    {
        return pidController.Output(Get180(degrees), Get180(body.rotation.eulerAngles.x), Time.deltaTime);
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

    // ------------------OLD CODE TO LAUGH AT -------------------------------

    private void NaiveKeepPitch(float degrees)
    {
        if (Get180(body.rotation.eulerAngles.x - degrees) > PITCH_SENSITIVITY)
        {
            SetPwr(0, 0.5f);
            SetPwr(3, 0.5f);
        }
        else if (Get180(body.rotation.eulerAngles.x - degrees) < PITCH_SENSITIVITY)
        {
            SetPwr(0, 0);
            SetPwr(3, 0);
        }
    }

    private void NaiveKeepRoll(float degrees)
    {
        if (Get180(body.rotation.eulerAngles.z - degrees) > ROLL_SENSITIVITY)
        {
            SetPwr(0, 0);
            SetPwr(1, 0);
        }
        else if (Get180(body.rotation.eulerAngles.z - degrees) < ROLL_SENSITIVITY)
        {
            SetPwr(0, 0.5f);
            SetPwr(1, 0.5f);
        }
    }

    private void AltHold(float alt)
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

        for (int i = 0; i < engines.Length; i++)
        {
            SetPwr(i, pwr);
        }
    }
}
