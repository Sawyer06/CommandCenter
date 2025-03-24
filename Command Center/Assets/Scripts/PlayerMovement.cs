using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform _playerParent;
    [SerializeField] private Transform _playerCharacter;
    [SerializeField] private Rigidbody _playerRb;

    [Space(10)]
    
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    private float speed;

    [Space(10)]

    [SerializeField] private float _staminaDrainMult;
    [SerializeField] private float _staminaRefillMult;
    private float stamina;
    private float maxStamina = 1;
    [SerializeField] private Flashlight _flashlight;

    [Space(10)]

    [SerializeField] private Image _staminaUI;
    [SerializeField] private Slider _batteryChargeUI;

    private void Start()
    {
        stamina = maxStamina;
        _batteryChargeUI.maxValue = _flashlight.maxBatteryCharge;
    }

    private void Update()
    {
        UpdateUI();

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Movement
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        _playerRb.MovePosition(_playerParent.position + movement * speed * Time.deltaTime);

        // Looking
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 dir = hit.point - _playerCharacter.position;
            dir.y = 0;

            if (dir.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dir);
                _playerCharacter.rotation = Quaternion.RotateTowards(_playerCharacter.rotation, targetRotation, 500 * Time.deltaTime);
            }
        }

        // Sprinting
        speed = _walkSpeed;

        if (Input.GetButton("Sprint") && stamina > 0) // Button pressed.
        {
            speed = _runSpeed;
            stamina -= 0.1f * _staminaDrainMult * Time.deltaTime; // Drain stamina.
        }
        else if (!Input.GetButton("Sprint")) // Button not pressed.
        {
            speed = _walkSpeed;
            if (stamina < maxStamina) // Do not go over max stamina.
            {
                stamina += 0.1f * _staminaRefillMult * Time.deltaTime; // Refill stamina.
            }
            else if (stamina >= maxStamina)
            {
                stamina = maxStamina;
            }
        }
    }

    /// Update player UI.
    private void UpdateUI()
    {
        //Debug.Log("Updating UI");
        _staminaUI.fillAmount = Mathf.Clamp01(stamina); // Needs to be between 0 and 1.
        _batteryChargeUI.value = _flashlight.charge;
    }
}
