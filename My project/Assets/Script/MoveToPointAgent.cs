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
    //Cuando recibe una acci�n
    public override void OnActionReceived(ActionBuffers actions)
    {
        //La posici�n 0 del array lo interpretamos como la posici�n del agente en el punto x
        //La posici�n 1 del array lo interpretamos como la posici�n del agente en el punto z
        float x = actions.ContinuousActions[0];
        float z = actions.ContinuousActions[1];
        //Se le suma esa posici�n al agente
        transform.position += new Vector3(x, 0, z) * Time.deltaTime * speed;
    }
    //Para obtener observaciones
    public override void CollectObservations(VectorSensor sensor)
    {
        //Se a�ade como observa
        sensor.AddObservation(transform.position);
        sensor.AddObservation(checkpointr.position);
    }
    public override void OnEpisodeBegin()
    {
        
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
       
    }
    private void OnTriggerEnter(Collider other)
    {
        //Si es una pared , se penaliza y se reinicia
        if (other.CompareTag("Wall"))
        {
            AddReward(-1f);
            EndEpisode();
        }
        //Si es el punto final , se recompensa y se reinicia
        else if (other.CompareTag("Finish"))
        {
            AddReward(1f);
            EndEpisode();
        }
    }
}
