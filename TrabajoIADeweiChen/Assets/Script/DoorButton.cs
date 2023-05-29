using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase Auxiliar para el funcionamiento del boton para abrir puerta.
public class DoorButton : MonoBehaviour
{
    //Estado del boton
    private bool buttonDown = false;
    //La puerta que se va a desactivar
    [SerializeField] private GameObject Door;

    //Método que devuelve si ya está la puerta abierta para añadir observación de la meta
    public bool isOpen()
    {
        return buttonDown;
    }
    //Método que se llama cuando empieza un nuevo episodio , para resetear la escena
    public void ResetButton()
    {
        //Vuelve a color blanco , se activa la puerta y se resetea el booleano
        GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1f);
        Door.SetActive(true);
        buttonDown = false;
    }
    //Método que imita la función de pulsar boton
    public bool pushButton()
    {
        //Si no ha sido pulsado , lo pulsa
        if (!buttonDown)
        {
            // Cambia color, desactiva la puerta y setea a true el booleano
            GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.6f);
            buttonDown = true;
            Door.SetActive(false);
            //Se devuelve true indicando que el botón ha sido pulsado justo ahora
            return true;
        }
        else
        {
            //Se devuelve false indicando que el botón ya ha sido pulsado
            return false;
        }
    }
}
