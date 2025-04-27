using UnityEngine;

public class ChargeFlashlight : MonoBehaviour
{
    Flashlight gotFlashLight;
    public GameObject prompt;

    void Start()
    {
        gotFlashLight = GameObject.Find("Flashlight").GetComponent<Flashlight>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.gameObject.CompareTag("Player"))
        {
            if (prompt != null) prompt.SetActive(false);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.transform.tag.Equals("Player"))
        {
            if (prompt != null) prompt.SetActive(true);
            //gotFlashLight = other.transform.GetChild(3).GetComponent<Flashlight>();
            if (Input.GetKey(KeyCode.E))
            {
                gotFlashLight.Charging();
            }
        }
    }
}
