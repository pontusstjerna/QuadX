using UnityEngine;
using System.Collections;

public class PID {
    public float Kp { get; set; }
    public float Ki { get; set; }
    public float Kd { get; set; }

    private float error = 0;
    private float integral = 0;
    private float derivative = 0;

    private float lastError = 0;
    
    private float lastDtime = 0;
    
    private const float sampleTime = 0.01f; //100ms
    private float currentTime = 0;

    public PID(float Kp, float Ki, float Kd)
    {
        this.Kp = Kp;
        this.Ki = Ki;
        this.Kd = Kd;
    }

    public void Update(float setPoint, float position, float dTime)
    {
        if(currentTime >= sampleTime)
        {
            error = GetError(setPoint, position);
            derivative = GetDerivative(error);
            integral += error*sampleTime;
            currentTime = 0;
        }
        currentTime += dTime;
    }

    public float GetOutput()
    {
        return Kp * error + Ki * integral + Kd * derivative;
    }
	
    private float GetError(float setPoint, float position)
    {
      return (setPoint - position);
    }

    private float GetDerivative(float error)
    {
        float derivative = (error - lastError) / sampleTime;

        lastError = error;

        return derivative;
    }

    private float GetErrorSum()
    {
        return integral;
    }
}
