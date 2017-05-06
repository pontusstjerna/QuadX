using UnityEngine;
using System.Collections;

public class PID {
    private float Kp = 0;
    private float Ki = 0;
    private float Kd = 0;

    private float lastError = 0;
    private float lastDtime = 0;
    private float errSum = 0;
    private float threshold = 0;

    private float currOutput = 0;

    private const float SAMPLE_TIME = 1.0f;

    public PID(float Kp, float Ki, float Kd, float threshold)
    {
        this.Kp = Kp;
        this.Ki = Ki;
        this.Kd = Kd;
        this.threshold = threshold;
    }

    public float GetOutput(float setPoint, float position, float dTime)
    {
        float error = GetError(setPoint, position, dTime);
        errSum += error;

        currOutput = Kp * error + Ki * errSum + Kd * GetDerivative(error, dTime);
        return currOutput;
    }
	
    public float GetError(float setPoint, float position, float dTime)
    {
        float error = (setPoint - position);
        if (Mathf.Abs(error) > threshold)
            return error;
        else return 0;
    }

    public float GetDerivative(float error, float dTime)
    {
        float derivative = (error - lastError) / 1;

        lastError = error;

        return derivative;
    }
}
