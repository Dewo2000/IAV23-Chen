using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class OpenDoorAgent : Agent
{
    [SerializeField] private Transform button;
    [SerializeField] private Transform checkPoint;
    [SerializeField] private GameObject floor;
    [SerializeField] private Material fail;
    [SerializeField] private Material success;
    public float speed;
    //Cuando recibe una acción
    public override void OnActionReceived(ActionBuffers actions)
    {
        //La posición 0 del array lo interpretamos como la posición del agente en el punto x
        //La posición 1 del array lo interpretamos como la posición del agente en el punto z
        int x = actions.DiscreteActions[0]; //0:No se mueve 1:Derecha 2:Izquierda
        int z = actions.DiscreteActions[1]; //0:No se mueve 1:Delante 2:Detras

        Vector3 force = Vector3.zero;
        switch (x)
        {
            case 0: force.x = 0f;
                break;
            case 1:
                force.x = 1f;
                break;
            case 2:
                force.x = -1f;
                break;
        }
        switch (z)
        {
            case 0:
                force.z = 0f;
                break;
            case 1:
                force.z = 1f;
                break;
            case 2:
                force.z = -1f;
                break;
        }
        transform.localPosition += new Vector3(x, 0, z) * Time.deltaTime * speed;
        GetComponent<Rigidbody>().velocity = force * speed * Time.deltaTime;

        AddReward(-1f / MaxStep);
    }
    //Para obtener observaciones
    public override void CollectObservations(VectorSensor sensor)
    {
        //Se añade como observa
       
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);
        sensor.AddObservation(button.localPosition.x);
        sensor.AddObservation(button.localPosition.z);
        bool doorOpen = button.GetComponent<DoorButton>().isOpen();
        sensor.AddObservation(doorOpen);
        if (doorOpen)
        {
            sensor.AddObservation(checkPoint.transform.localPosition.x);
            sensor.AddObservation(checkPoint.transform.localPosition.z);
        }
        else
        {
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
        }
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
        else if (other.CompareTag("Button"))
        {
            floor.GetComponent<Renderer>().material = success;
            AddReward(1f);
            EndEpisode();
        }
    }
}
