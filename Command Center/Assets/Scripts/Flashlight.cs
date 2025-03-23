using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public bool on;

    public float charge;

    [SerializeField] private float _distance;
    [SerializeField] private float _strength;

    [SerializeField] private Light _spotLight;

    private void Update()
    {
        if (Input.GetButtonDown("Flashlight") && charge > 0)
        {
            if (!on)
            {
                LightOn();
            }
            else
            {
                LightOff();
            }
        }

        if (on && charge > 0)
        {
            EnemyCheck();
        }
        else if (on && charge <= 0)
        {
            charge = 0;
            LightOff();
        }
    }

    /// Check for enemy using raycast.
    private void EnemyCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, _distance))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                //Debug.Log("Hit");
                hit.transform.GetComponent<HunterAI>().health -= 1f * _strength * Time.deltaTime; // Drain enemy health on flashlight hitting enemy.
            }
        }
    }

    private void LightOn()
    {
        on = true;
        _spotLight.enabled = true;
    }
    private void LightOff()
    {
        on = false;
        _spotLight.enabled = false;
    }
}
