using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToPointAgent : Agent
{
    [SerializeField] private Transform checkpointr;
    [SerializeField] private GameObject floor;
    public float speed;
    //Cuando recibe una acción
    public override void OnActionReceived(ActionBuffers actions)
    {
        //La posición 0 del array lo interpretamos como la posición del agente en el punto x
        //La posición 1 del array lo interpretamos como la posición del agente en el punto z
        float x = actions.ContinuousActions[0];
        float z = actions.ContinuousActions[1];
        //Se le suma esa posición al agente
        transform.localPosition += new Vector3(x, 0, z) * Time.deltaTime * speed;
    }
    //Para obtener observaciones
    public override void CollectObservations(VectorSensor sensor)
    {
        //Se añade como observa
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(checkpointr.localPosition);
    }
    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(0,6f),0,Random.Range(-3f, 3f));
        checkpointr.localPosition = new Vector3(Random.Range(-6f, -2f), 0, Random.Range(-3f, 3f));
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
       ActionSegment<float> actions =  actionsOut.ContinuousActions;
        actions[0] = Input.GetAxisRaw("Horizontal");
        actions[1] = Input.GetAxisRaw("Vertical");
    }
    private void OnTriggerEnter(Collider other)
    {
        //Si es una pared , se penaliza y se reinicia
        if (other.CompareTag("Wall"))
        {
            floor.GetComponent<Renderer>().material.color = new Color(1, 0, 0,0.5f);
            AddReward(-1f);
            EndEpisode();
        }
        //Si es el punto final , se recompensa y se reinicia
        else if (other.CompareTag("Finish"))
        {
            floor.GetComponent<Renderer>().material.color = new Color(0, 1, 0,0.5f);
            AddReward(1f);
            EndEpisode();
        }
    }
}
