using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToPointAgent : Agent
{
    [SerializeField] private Transform checkpointr;
    public float speed;
    public override void OnActionReceived(ActionBuffers actions)
    {
        float x = actions.ContinuousActions[0];
        float z = actions.ContinuousActions[1];
        transform.position += new Vector3(x, 0, z) * Time.deltaTime * speed;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(checkpointr.position);
    }
    public override void OnEpisodeBegin()
    {
        
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
       
    }
}
