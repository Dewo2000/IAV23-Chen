using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class OpenDoorAgent : Agent
{
    //Para obtener la posición de los objetos
    [SerializeField] private Transform button;
    [SerializeField] private Transform checkPoint;
    //Para cambiar el color del suelo en función del estado de aprendizaje
    [SerializeField] private GameObject floor;
    //Los materiales de distintos estados
    [SerializeField] private Material fail;
    [SerializeField] private Material success;
    [SerializeField] private Material phase1;
    //La velocidad que se mueve el agente
    public float speed;
    //El material original del suelo
    private Material Original;
    private void Start()
    {
        //Obtener el material original del suelo
        Original = floor.GetComponent<Renderer>().material;
    }

    //Cuando recibe una acción
    public override void OnActionReceived(ActionBuffers actions)
    {
        //La posición 0 del array lo interpretamos como la posición del agente en el punto x
        //La posición 1 del array lo interpretamos como la posición del agente en el punto z

        int x = actions.DiscreteActions[0]; //0:No se mueve 1:Derecha 2:Izquierda
        int z = actions.DiscreteActions[1]; //0:No se mueve 1:Delante 2:Detras

        //Se le añade una fuerza dependiendo del valor aletorio guardado en el array de DiscreteActions
        Vector3 force = new Vector3(0,0,0);
        switch (x)
        {
            //0:No se mueve 
            case 0: force.x = 0f;
                break;
            //1:Derecha
            case 1:
                force.x = 1f;
                break;
            // 2:Izquierda
            case 2:
                force.x = -1f;
                break;
        }
        switch (z)
        {
            //0:No se mueve
            case 0:
                force.z = 0f;
                break;
            // 1:Delante 
            case 1:
                force.z = 1f;
                break;
            // 2:Detras
            case 2:
                force.z = -1f;
                break;
        }
        //Se le aplica la fuerza calculada
        GetComponent<Rigidbody>().velocity = force * speed + new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);
    }
    //Para obtener observaciones
    public override void CollectObservations(VectorSensor sensor)
    {
        //Se calcula la diferencia de posición entre el agente y el boton.
        Vector3 dirToButton = (button.transform.localPosition - transform.localPosition).normalized;
        //Se añade las coordenas x z de la posición calculada
        sensor.AddObservation(dirToButton.x);
        sensor.AddObservation(dirToButton.z);
        // Se obtiene el booleano de si la puerta está abierta
        bool doorOpen = button.GetComponent<DoorButton>().isOpen();
        // Se añade como observación
        sensor.AddObservation(doorOpen);
        // Dependiendo de si la puerta está abierta
        if (doorOpen)
        {
            //Abierto , se añade como observación la distancia entre el agente y el checkpoint
            Vector3 dirToCheckpoing = (checkPoint.transform.localPosition - transform.localPosition).normalized;
            sensor.AddObservation(dirToCheckpoing.x);
            sensor.AddObservation(dirToCheckpoing.z);
        }
        else
        {
            //Cerrado , se añade un valor nulo
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
        }
    }

    //Resetea la escena cuando empieza una nueva ronda de aprendizaje
    public override void OnEpisodeBegin()
    {
        //Devuelve al agente a su pos inicial
        transform.localPosition = Vector3.zero;
        //Se setea el botón a una z aleatoria y se resetea el pulsado del botón
        button.localPosition = new Vector3(7, 0, Random.Range(-3f, 3f));
        button.GetComponent<DoorButton>().ResetButton();
      
    }

    // La heuristica sirve para controlar nosotros manualmente al agente , asignando manualmente valores a actions.
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //Obtenemos el array de DiscreteActions , los valores que se asigna coincide con la parte de OnActionReceived
        ActionSegment<int> actions = actionsOut.DiscreteActions;
        //Para el caso horizontal
        switch (Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")))
        {
            case -1: actions[0] = 2;break;
            case  0: actions[0] = 0; break;
            case  1: actions[0] = 1; break;
        }
        //Para el caso vertical
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
        // Si es botón , se recompensa y sigue el proceso
        else if (other.CompareTag("Button"))
        {
            //Aqui se comprueba que es la primera vez que se pulsa para no añadir reward demas.
            if (button.GetComponent<DoorButton>().pushButton())
            {
                floor.GetComponent<Renderer>().material = phase1;
                AddReward(1f);
            }
        }
        //Si es el punto final , se recompensa y se reinicia
        else if (other.CompareTag("Finish"))
        {
            floor.GetComponent<Renderer>().material = success;
            AddReward(1f);
            EndEpisode();
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        //Si colisiona con la puerta , cambia el material a original
        //Esta hecho para algunos casos que se atasca en la puerta y visualmente no poder diferenciar
        if (collision.gameObject.CompareTag("Door"))
        {
            floor.GetComponent<Renderer>().material = Original;
        }
    }
}
