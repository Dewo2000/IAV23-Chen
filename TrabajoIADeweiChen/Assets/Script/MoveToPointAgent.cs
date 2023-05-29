using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToPointAgent : Agent
{
    //Para obtener la posición de los objetos
    [SerializeField] private Transform checkpointr;
    //Para cambiar el color del suelo en función del estado de aprendizaje
    [SerializeField] private GameObject floor;
    //Los materiales de distintos estados
    [SerializeField] private Material fail;
    [SerializeField] private Material success;
    //La velocidad que se mueve el agente
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
        //V2
        //Se calcula la diferencia de posición entre el agente y el checkpoint y se añade como observación.
        Vector3 dirToPint = (checkpointr.transform.localPosition - transform.localPosition).normalized;
        sensor.AddObservation(dirToPint.x);
        sensor.AddObservation(dirToPint.z);
        //V1
        //Se añade directamente las posiciones del agente y el checkpoint
        //sensor.AddObservation(transform.localPosition.x);
        //sensor.AddObservation(transform.localPosition.z);
        //sensor.AddObservation(checkpointr.localPosition.x);
        //sensor.AddObservation(checkpointr.localPosition.z);
    }
    //Resetea la escena cuando empieza una nueva ronda de aprendizaje
    public override void OnEpisodeBegin()
    {
        //Se pone al agente y el checkpoint en posiciones aleatorias
        transform.localPosition = new Vector3(Random.Range(0, 6f), 0, Random.Range(-3f, 3f));
        checkpointr.localPosition = new Vector3(Random.Range(-6f, -2f), 0, Random.Range(-3f, 3f));
    }
    // La heuristica sirve para controlar nosotros manualmente al agente , asignando manualmente valores a actions.
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
