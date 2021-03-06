﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    List<SteeringBehaviour> strBehaviours = new List<SteeringBehaviour>();
    public Vector3 vel = Vector3.zero;
    public Vector3 accel = Vector3.zero;
    public Vector3 force;

    public float mass = 1.0f;

    public float maxSpeed = 1.5f;
    public float maxForce = 10;

    public float dist;
    public float damping = 0.1f;

    private float banking = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        SteeringBehaviour[] strBehaviours = GetComponents<SteeringBehaviour>(); 
        foreach (SteeringBehaviour b in strBehaviours)
        {
            this.strBehaviours.Add(b); //Adding all instances of the SteeringBehaviour script to the list
        }
    }

    public Vector3 SeekingForce(Vector3 target)
    {
        Vector3 desired = target - transform.position; //Distance to the target
        desired = desired / desired.magnitude; //Normalizing to get the direction to the target
        desired = desired * maxSpeed; //Calculates the force in the direction of the normalized desired vector at the speed specified by the maxSpeed
        return desired - vel;
    }

    public Vector3 ArrivingForce(Vector3 target, float slowingDist = 15.0f)
    {
        Vector3 toTarg = target - transform.position;
 
        if (toTarg.magnitude < 0.1f)
        {
            return Vector3.zero;
        }
        float rampMult = dist / slowingDist;
        float ramped = maxSpeed * rampMult;

        float clamped = Mathf.Min(ramped, maxSpeed);
        Vector3 clampMult = toTarg / dist;
        Vector3 desired = clamped * (clampMult);

        return desired - vel;
    }

    Vector3 CalculateForce()
    {
        force = Vector3.zero;
        foreach (SteeringBehaviour b in strBehaviours)
        {
            if (b.isActiveAndEnabled)
            {
                force += b.Calculate() * b.boidWeight;
                if (force.magnitude >= maxForce)
                {
                    force = Vector3.ClampMagnitude(force, maxForce);
                    break;
                }
            }
        }
        return force;
    }

    void FixedUpdate()
    {
        force = CalculateForce();
        float speed = vel.magnitude;
        Vector3 newAccel = force / mass;
        accel = Vector3.Lerp(accel, newAccel, Time.deltaTime);
        vel += accel * Time.deltaTime;
        vel = Vector3.ClampMagnitude(vel, maxSpeed);

        if (speed > 0)
        {
            Vector3 tempUp = Vector3.Lerp(transform.up, Vector3.up + (accel * banking), Time.deltaTime * 3.0f);
            transform.LookAt(transform.position + vel, tempUp);

            transform.position += vel * Time.deltaTime;
            vel *= (1.0f - (damping * Time.deltaTime));
        }
    }
}
