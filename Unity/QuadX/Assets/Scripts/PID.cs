using UnityEngine;
using System.Collections;

public class PID {
    private float Kp = 0;
    private float Ki = 0;
    private float Kd = 0;

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
        errSum += error;

        return Kp * error + Ki * errSum + Kd * GetDerivative(error);
    }
	
    public float GetError(float setPoint, float position, float dTime)
    {
      return (setPoint - position);
    }

    public float GetDerivative(float error)
    {
        float derivative = (error - lastError) / 1;

        lastError = error;

        return derivative;
    }
}
