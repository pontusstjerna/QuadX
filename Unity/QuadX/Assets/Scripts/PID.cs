using UnityEngine;
using System.Collections;

public class PID {
    public float Kp { get; set; }
    public float Ki { get; set; }
    public float Kd { get; set; }

    private float lastError = 0;
    private float lastDtime = 0;
    private float errSum = 0;

    public PID(float Kp, float Ki, float Kd)
    {
        this.Kp = Kp;
        this.Ki = Ki;
        this.Kd = Kd;
    }

    public float GetOutput(float setPoint, float position, float dTime)
    {
        float error = GetError(setPoint, position, dTime);
        errSum += error*dTime;

        return Kp * error*dTime + Ki * errSum * dTime + Kd * GetDerivative(error, dTime);
    }
	
    public float GetError(float setPoint, float position, float dTime)
    {
      return (setPoint - position);
    }

    public float GetDerivative(float error, float dTime)
    {
        float derivative = dTime*(error*dTime - lastError) / 1;

        lastError = error;

        return derivative;
    }

    public float GetErrorSum()
    {
        return errSum;
    }
}
