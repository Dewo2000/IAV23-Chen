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
    [SerializeField] private Material phase1;
    public float speed;
    private Material Original;
    //Cuando recibe una acción
    private void Start()
    {
        Original = floor.GetComponent<Renderer>().material;
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        //La posición 0 del array lo interpretamos como la posición del agente en el punto x
        //La posición 1 del array lo interpretamos como la posición del agente en el punto z

        int x = actions.DiscreteActions[0]; //0:No se mueve 1:Derecha 2:Izquierda
        int z = actions.DiscreteActions[1]; //0:No se mueve 1:Delante 2:Detras

        Vector3 force = new Vector3(0,0,0);
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
        //transform.localPosition += new Vector3(x, 0, z) * Time.deltaTime * speed;
        GetComponent<Rigidbody>().velocity = force * speed + new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);

        //AddReward(-1f / MaxStep);
    }
    //Para obtener observaciones
    public override void CollectObservations(VectorSensor sensor)
    {
        //Se añade como observa
        Vector3 dirToButton = (button.transform.localPosition - transform.localPosition).normalized;
        sensor.AddObservation(dirToButton.x);
        sensor.AddObservation(dirToButton.z);
        bool doorOpen = button.GetComponent<DoorButton>().isOpen();
        sensor.AddObservation(doorOpen);
        if (doorOpen)
        {
            Vector3 dirToCheckpoing = (checkPoint.transform.localPosition - transform.localPosition).normalized;
            sensor.AddObservation(dirToCheckpoing.x);
            sensor.AddObservation(dirToCheckpoing.z);
        }
        else
        {
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
        }
    }
    public override void OnEpisodeBegin()
    {
        transform.localPosition = Vector3.zero;
        button.localPosition = new Vector3(7, 0, Random.Range(-3f, 3f));
        button.GetComponent<DoorButton>().ResetButton();
      
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> actions = actionsOut.DiscreteActions;
        switch (Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")))
        {
            case -1: actions[0] = 2;break;
            case  0: actions[0] = 0; break;
            case  1: actions[0] = 1; break;
        }
        switch (Mathf.RoundToInt(Input.GetAxisRaw("Vertical")))
        {
            case -1: actions[1] = 2; break;
            case 0: actions[1] = 0; break;
            case 1: actions[1] = 1; break;
        }
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
            if (button.GetComponent<DoorButton>().pushButton())
            {
                floor.GetComponent<Renderer>().material = phase1;
                AddReward(1f);
            }
        }
        else if (other.CompareTag("Finish"))
        {
            floor.GetComponent<Renderer>().material = success;
            AddReward(1f);
            EndEpisode();
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Door"))
        {
            floor.GetComponent<Renderer>().material = Original;
        }
    }
}
