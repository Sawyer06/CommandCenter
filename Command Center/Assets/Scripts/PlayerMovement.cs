using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform _playerParent;
    [SerializeField] private Transform _playerCharacter;
    [SerializeField] private RawImage _playerImg;
    [SerializeField] private Material _playerMat;
    [SerializeField] private Rigidbody _playerRb;

    [Space(10)]
    
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    public float speed;

    [Space(10)]

    [SerializeField] private float _staminaDrainMult;
    [SerializeField] private float _staminaRefillMult;
    private float stamina;
    private float maxStamina = 1;
    [SerializeField] private Flashlight _flashlight;

    [Space(10)]

    [SerializeField] private TextMeshProUGUI _pickupAmntTxt;
    [SerializeField] private Image _staminaUI;
    [SerializeField] private Slider _batteryChargeUI;
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private GameObject _winScreen;

    [Space(10)]

    [SerializeField] private Animator _animator;
    [SerializeField] private Animator _damageAnimator;

    [Space(10)]

    [SerializeField] private AudioSource _footstepSound;
    [SerializeField] private AudioClip _walkClip;
    [SerializeField] private AudioClip _runClip;
    [SerializeField] private AudioSource _looseSound;

    private bool moving;

    private bool gameOver = false;

    private void Start()
    {
        stamina = maxStamina;
        _batteryChargeUI.maxValue = _flashlight.maxBatteryCharge;
    }

    private void Update()
    {
        UpdateUI();
        _playerMat.mainTexture = _playerImg.texture;

        if (GlobalVariables.m_communicationParts >= 5)
        {
            _winScreen.SetActive(true);
            GlobalVariables.m_health = 2;
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Movement
        Vector3 movement = !gameOver ? new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) : Vector3.zero;
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) moving = true;
        else moving = false;

        _playerRb.MovePosition(_playerParent.position + movement * speed * Time.deltaTime);

        if (GlobalVariables.m_health <= 0 && !gameOver)
        {
            GameOver();
        }
        Debug.Log("Health = " + GlobalVariables.m_health + ", GameOver: " + gameOver);

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
                
                // Update animator based on where mouse is.
                if (dir.x > 0 && dir.z < dir.x)
                {
                    //Debug.Log("Right");
                    _animator.SetInteger("x", 1);
                    _animator.SetInteger("z", 0);
                }
                else if (dir.x < 0 && dir.z > dir.x)
                {
                    //Debug.Log("Left");
                    _animator.SetInteger("x", -1);
                    _animator.SetInteger("z", 0);
                }
                else if (dir.z > 0 && dir.x < dir.z)
                {
                    //Debug.Log("Back");
                    _animator.SetInteger("z", 1);
                    _animator.SetInteger("x", 0);
                }
                else if (dir.z < 0 && dir.x > dir.z)
                {
                    //Debug.Log("Forward");
                    _animator.SetInteger("z", -1);
                    _animator.SetInteger("x", 0);
                }
            }
        }

        // Sprinting
        if (Input.GetButton("Sprint") && stamina > 0) // Button pressed.
        {
            if (GlobalVariables.m_health >= 2)
            {
                speed = _runSpeed;
                _footstepSound.clip = _runClip;
            }
            else
            {
                speed = _runSpeed / 2;
            }
            stamina -= 0.1f * _staminaDrainMult * Time.deltaTime; // Drain stamina.
        }
        else if (!Input.GetButton("Sprint") || stamina <= 0) // Button not pressed.
        {
            if (GlobalVariables.m_health >= 2)
            {
                speed = _walkSpeed;
                _footstepSound.clip = _walkClip;
            }
            else
            {
                speed = _walkSpeed / 1.5f;
                _footstepSound.clip = _walkClip;
            }
            
            if (stamina < maxStamina) // Do not go over max stamina.
            {
                stamina += 0.1f * _staminaRefillMult * Time.deltaTime; // Refill stamina.
            }
            else if (stamina >= maxStamina)
            {
                stamina = maxStamina;
            }
        }

        if (moving)
        {
            _animator.SetBool("moving", true);
            if (!_footstepSound.isPlaying)
            {
                _footstepSound.Play();
            }
        }
        else
        {
            _animator.SetBool("moving", false);
        }
    }

    /// Update player UI.
    private void UpdateUI()
    {
        //Debug.Log("Updating UI");
        _staminaUI.fillAmount = Mathf.Clamp01(stamina); // Needs to be between 0 and 1.
        _batteryChargeUI.value = _flashlight.charge;

        if (GlobalVariables.m_health < 2)
        {
            _damageAnimator.enabled = true;
        }
        else
        {
            _damageAnimator.enabled = false;
        }

        _pickupAmntTxt.text = GlobalVariables.m_communicationParts.ToString() + "/" + GlobalVariables.m_maxCommunicationParts.ToString();
    }

    private void GameOver()
    {
        Debug.Log("Game Over");
        gameOver = true;
        _gameOverScreen.SetActive(true);
        if (!_looseSound.isPlaying)
        {
            _looseSound.Play();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if player object collides with communication part.
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);

            GlobalVariables.m_communicationParts++;
        }
    }
}
