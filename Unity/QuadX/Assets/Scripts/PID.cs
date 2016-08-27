using UnityEngine;
using System.Collections;

public class PID {
    private float Kp = 0;
    private float Ki = 0;
    private float Kd = 0;

    private float lastError = 0;
    private float lastDtime = 0;
    private float errSum = 0;

    private float currOutput = 0;
    private float timeChange = 0;

    private const int SAMPLE_TIME = 1000;

    public PID(float Kp, float Ki, float Kd)
    {
        this.Kp = Kp;
        this.Ki = Ki;
        this.Kd = Kd;
    }

    public float Output(float setPoint, float position, float dTime)
    {
        if(timeChange >= SAMPLE_TIME)
        {
            float error = GetError(setPoint, position, dTime);
            errSum = (errSum + error * timeChange) / Time.frameCount;

            timeChange = 0;
            currOutput = Kp * error + Ki * errSum + Kd * GetDerivative(error, dTime);
            return currOutput;
        }

        timeChange += dTime*1000f;
        return currOutput;
    }
	
    private float GetError(float setPoint, float position, float dTime)
    {
        return (setPoint - position);
    }

    private float GetDerivative(float error, float dTime)
    {
        float derivative = (error - lastError) / dTime;

        lastError = error;

        return derivative;
    }
}
