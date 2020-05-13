﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding_Random : SteeringBehaviour
{
    public Vector3 velocity = Vector3.zero;
    public Vector3 acceleration = Vector3.zero;

    public float mass = 1.0f;

    //public float maxSpeed = 5;
    public float maxForce = 10;
    public int i = 0;

    public float speed = 0;
    public float dist;

    public Vector3 target; //The current waypoint position
    public List<Transform> targetTransforms; //The array of all waypoints
    private GameObject triggerObj;
    private Pathfinding_Trigger triggerScript;

    private bool arrived = false;
    public float changingDistance = 4.0f; //the distance from a waypoint at which the boid cycles to the next waypoint
    public float banking = 0.2f;

    public Transform wayPointsHolder;

    // Start is called before the first frame update
    void Start()
    {
        wayPointsHolder = GameObject.Find("WayPoints_Ships2").GetComponent<Transform>();
        triggerObj = GameObject.Find("Ships2_PathfindingTrigger");
        triggerScript = triggerObj.GetComponent<Pathfinding_Trigger>();
        i = Random.Range(0, targetTransforms.Count);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < targetTransforms.Capacity - 1; i++)
        {
            Gizmos.DrawLine(targetTransforms[i].position, targetTransforms[i + 1].position); //Drawing lines between each waypoint
        }
        for (int i = 0; i < targetTransforms.Capacity; i++)
        {
            Gizmos.DrawCube(targetTransforms[i].position, new Vector3(1, 1, 1)); //Drawing cubes at each waypoint to visualise them for easy editing
            Gizmos.DrawLine(targetTransforms[i].position, targetTransforms[0].position);
        }
        Gizmos.color = Color.red;
        for (int i = 0; i < targetTransforms.Capacity; i++)
        {
            Gizmos.DrawWireSphere(targetTransforms[i].position, changingDistance);
        }
    }

    Vector3 Seek(Vector3 target) //Gets the distance to the target and the speed the boid needs to be travelling at to reach the target
    {
        Vector3 toTarget = target - transform.position;
        Vector3 desired = toTarget.normalized * boid.maxSpeed;

        return desired - velocity;
    }
    public override Vector3 Calculate() //Calculates the force needed to move the boid
    {
        Vector3 toTarget = target - transform.position;
        dist = toTarget.magnitude;
        Vector3 force = Vector3.zero;
        force += Seek(target);
        return force;
    }

    public void Switching() //Detects if the boid is within the changing distance and has not arrived yet
    {
        Vector3 toTarget = target - transform.position;
        dist = toTarget.magnitude;
        if (dist <= changingDistance && !arrived)
        {
            arrived = true;
        }
        else
        {
            arrived = false;
        }
    }

    void FixedUpdate()
    {
      foreach(Transform child in wayPointsHolder)
        {
            targetTransforms.Add(child);
        }
        if (target != null)
        {
            target = targetTransforms[i].position;
        }
        force = Calculate();
        acceleration = force / mass;
        if (triggerScript.pathfindingEnabled == true)
        {
            velocity += acceleration * Time.deltaTime;
            transform.position += velocity * Time.deltaTime;
            speed = velocity.magnitude;
            transform.LookAt(target);
        }
        else
        {

        }
        if (speed > 0)
        {
            Vector3 tempUp = Vector3.Lerp(transform.up, Vector3.up + (acceleration * banking), Time.deltaTime * 3.0f);
            transform.LookAt(transform.position + velocity, tempUp); //Applies banking
        }
        Switching();
        if (arrived) //moves on to the next waypoint and sets the arrived bool to false
        {
            i = Random.Range(0, targetTransforms.Capacity);
            arrived = false;
        }
        else if (!arrived)
        {

        }
        if (i > (targetTransforms.Capacity - 1))
        {
            i = 0; //Resets the array index so that it does not go out of bounds
        }
    }
}