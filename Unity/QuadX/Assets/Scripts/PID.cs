﻿using UnityEngine;
using System.Collections;

public class PID {
    private float Kp = 0;
    private float Ki = 0;
    private float Kd = 0;

    private float lastError = 0;
    private float errSum = 0;

    public PID(float Kp, float Ki, float Kd)
    {
        this.Kp = Kp;
        this.Ki = Ki;
        this.Kd = Kd;
    }

    public float Output(float setPoint, float position, float dTime)
    {
        float error = GetError(setPoint, position, dTime);
        errSum += error * dTime;

        return Kp*error + Ki*errSum + Kd*GetDerivative(error, dTime);
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