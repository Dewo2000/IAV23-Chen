using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class OpenDoorAgent : Agent
{
    [SerializeField] private Transform button;
    [SerializeField] private GameObject floor;
    [SerializeField] private Material fail;
    [SerializeField] private Material success;
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

        //AddReward(-1f / MaxStep);
    }
    //Para obtener observaciones
    public override void CollectObservations(VectorSensor sensor)
    {
        //Se añade como observa
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(button.localPosition);
        //sensor.AddObservation(transform.localPosition - checkpointr.localPosition);
    }
    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(0, 6f), 0, Random.Range(-3f, 3f));
        button.localPosition = new Vector3(Random.Range(-6f, -2f), 0, Random.Range(-3f, 3f));
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> actions = actionsOut.ContinuousActions;
        actions[0] = Input.GetAxisRaw("Horizontal");
        actions[1] = Input.GetAxisRaw("Vertical");
    }
    private void OnTriggerEnter(Collider other)
    {
        //Si es una pared , se penaliza y se reinicia
        if (other.CompareTag("Wall"))
        {
            floor.GetComponent<Renderer>().material = fail;
            AddReward(-1f);
            EndEpisode();
        }
        //Si es el punto final , se recompensa y se reinicia
        else if (other.CompareTag("Finish"))
        {
            floor.GetComponent<Renderer>().material = success;
            AddReward(1f);
            EndEpisode();
        }
    }
}
