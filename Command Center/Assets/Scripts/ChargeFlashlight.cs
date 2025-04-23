using UnityEngine;

public class ChargeFlashlight : MonoBehaviour
{
    Flashlight gotFlashLight;
    void Start()
    {
        gotFlashLight = GameObject.Find("Flashlight").GetComponent<Flashlight>();
    }

    void OnTriggerStay(Collider other)
    {
        if(other.transform.tag.Equals("Player"))
        {
            //gotFlashLight = other.transform.GetChild(3).GetComponent<Flashlight>();
            if (Input.GetKey(KeyCode.G))
            {
                gotFlashLight.Charging();
            }
        }
    }
}
