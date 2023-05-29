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

    //M�todo que devuelve si ya est� la puerta abierta para a�adir observaci�n de la meta
    public bool isOpen()
    {
        return buttonDown;
    }
    //M�todo que se llama cuando empieza un nuevo episodio , para resetear la escena
    public void ResetButton()
    {
        //Vuelve a color blanco , se activa la puerta y se resetea el booleano
        GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1f);
        Door.SetActive(true);
        buttonDown = false;
    }
    //M�todo que imita la funci�n de pulsar boton
    public bool pushButton()
    {
        //Si no ha sido pulsado , lo pulsa
        if (!buttonDown)
        {
            // Cambia color, desactiva la puerta y setea a true el booleano
            GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.6f);
            buttonDown = true;
            Door.SetActive(false);
            //Se devuelve true indicando que el bot�n ha sido pulsado justo ahora
            return true;
        }
        else
        {
            //Se devuelve false indicando que el bot�n ya ha sido pulsado
            return false;
        }
    }
}
