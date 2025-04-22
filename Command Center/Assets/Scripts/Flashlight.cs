using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public bool on;

    public float charge;
    public float maxBatteryCharge;
    [SerializeField] private float _batteryDrainMultiplier;
    [SerializeField] private float _batteryRefillMultiplier;

    [Space(10)]

    [SerializeField] private float _distance;
    [SerializeField] private float _strength;

    [Space(10)]

    [SerializeField] private Light _spotLight;

    [Space(10)]

    [SerializeField] private AudioSource _onSound;
    [SerializeField] private AudioSource _offSound;

    private void Start()
    {
        charge = maxBatteryCharge;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Flashlight")) // If designated flashlight button pressed.
        {
            if (!on && charge > 0) // Turn on if off.
            {
                LightOn();
            }
            else // Turn off if on.
            {
                LightOff();
            }
        }

        if (on && charge > 0) // Flashlight is on and has battery.
        {
            EnemyCheck();
            charge -= 0.1f * _batteryDrainMultiplier * Time.deltaTime;
        }
        else if (on && charge <= 0) // Flashlight is on but has run out of battery.
        {
            charge = 0;
            LightOff();
        }
        else if (!on && charge <= maxBatteryCharge) // Flashlight is off and has not exceeded the max battery charge.
        {
            //charge += 0.1f * _batteryRefillMultiplier * Time.deltaTime;
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
        if (!_onSound.isPlaying)
        {
            _onSound.Play();
        }
    }
    private void LightOff()
    {
        on = false;
        _spotLight.enabled = false;
        if (!_offSound.isPlaying)
        {
            _offSound.Play();
        }
    }
    public void Charging()
    {
        if (charge! > maxBatteryCharge)
        {
            charge += 0.0025f;
        }
    }
}
