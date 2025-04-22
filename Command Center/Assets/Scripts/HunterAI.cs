using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class HunterAI : MonoBehaviour
{
    public bool enabled = true;
    public bool displayDebug = true;

    [SerializeField] private Transform _enemy;
    [SerializeField] private Transform _enemySprite;
    [SerializeField] private RawImage _enemyImg;
    [SerializeField] private Material _enemyMat;
    [SerializeField] private Transform _player;

    [SerializeField] private Vector3 _spriteOffset;

    [Space(10)]

    [SerializeField] private NavMeshAgent _agent;

    [SerializeField] private float _maxHealth;
    public float health;

    [SerializeField] private float _visionRange;

    private float engage;
    [SerializeField] private float _giveUpTime;
    
    [Space(10)]

    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _chaseSpeed;
    [SerializeField] private float _runawaySpeed;
    private float speed;

    [Space(10)]

    [SerializeField] private float _respawnTime;

    [Space(10)]

    [SerializeField] private Animator _animator;
    [SerializeField] private Animator _fadeAnimator;
    [SerializeField] private ParticleSystem _hitParticles;

    [Space(10)]

    [SerializeField] private AudioSource _footstepSound;
    [SerializeField] private AudioClip _walkClip;
    [SerializeField] private AudioClip _runClip;
    [SerializeField] private AudioSource _chaseSound;
    [SerializeField] private AudioSource _patrolSound;
    
    public enum State
    { 
        patrol,
        chase,
        hunt,
    }

    public State activeState;

    [SerializeField] private List<Transform> _patrolPoints;
    private int patrolIndex;

    private Transform target;

    private void Start()
    {
        health = _maxHealth;
        _animator.SetBool("enabled", enabled);
        StartCoroutine(SoundLoop());
    }

    private void Update()
    {
        _enemySprite.eulerAngles = Vector3.zero + _spriteOffset;
        _animator.SetFloat("yRot", _enemy.eulerAngles.y);

        _enemyMat.mainTexture = _enemyImg.texture;

        if (enabled)
        {
            UpdateState();
            PlayerCheck();
            if (health <= 0)
            {
                RunAway(false);
            }
            else if (health <= _maxHealth / 2)
            {
                _hitParticles.Play();
            }
        }

        if (displayDebug)
        {
            DisplayEditorDebugInfo();
        }

        if (!_footstepSound.isPlaying && enabled)
        {
            _footstepSound.Play();
        }
    }

    private void UpdateState()
    {
        switch ((int)activeState)
        {
            case 0:
                Patrol();
                break;
            case 1:
                Chase();
                break;
            case 2:
                Hunt();
                break;
        }

        if (target != null)
        {
            _agent.destination = target.position;
        }
        _agent.speed = speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && enabled) // Damage player if touching.
        {
            _fadeAnimator.SetBool("fadeOut", true);
            GlobalVariables.m_health--;
            
            if (GlobalVariables.m_health > 0)
            {
                RunAway(true);
            }
            else
            {
                _fadeAnimator.SetBool("fadeOut", true);
            }
        }
    }

    /// Patrol following patrol points placed in level.
    private void Patrol()
    {
        //Debug.Log("Patrolling");
        speed = _walkSpeed;
        if (Vector3.Distance(_enemy.position, _patrolPoints[patrolIndex].position) < 3) // If enemy is within a certain range of the patrol point.
        {
            NextPatrolPoint();
        }
        else
        {
            if (target != _patrolPoints[patrolIndex])
            {
                target = _patrolPoints[patrolIndex];
            }
        }
        _footstepSound.clip = _walkClip;
    }

    /// Go through the patrol point list, and wrap back around when the end is reached.
    private void NextPatrolPoint()
    {
        patrolIndex++;
        if (patrolIndex >= _patrolPoints.Count) // Wrap back around to the first patrol point.
        {
            patrolIndex = 0;
        }
    }

    /// Chase after current position of player.
    private void Chase()
    {
        //Debug.Log("Chasing");
        engage -= 1f * Time.deltaTime; // Constantly subtract from engage. Refilled once seen again.
        if (engage <= 0) // Give up on chasing player if not seen anymore.
        {
            activeState = State.patrol;
        }

        speed = _chaseSpeed;
        if (target != _player)
        {
            target = _player;
        }
        _footstepSound.clip = _runClip;
    }

    /// Go investigate a certain position, possibly an old location of the player.
    private void Hunt()
    {
        //Debug.Log("Hunting");
    }

    /// The enemy gets sent back to the spawn location after a certain amount of time. Disable everything until then.
    private void RunAway(bool disablePlayer)
    {
        health = _maxHealth;
        enabled = false;
        _agent.enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        _animator.SetBool("enabled", enabled);
        //_animator.Play("Leviathan_Run");
        StartCoroutine(WaitToRespawn(disablePlayer));
    }

    private IEnumerator WaitToRespawn(bool disablePlayer)
    {
        if (disablePlayer) _player.GetComponent<PlayerMovement>().enabled = false;
        yield return new WaitForSeconds(2);
        _player.GetComponent<PlayerMovement>().enabled = true;

        _fadeAnimator.SetBool("fadeOut", false);
        int r = Random.Range(0, _patrolPoints.Count); // Pick a random patrol point to respawn at.
        target = _patrolPoints[r];
        transform.position = _patrolPoints[r].position + Vector3.up * 10; // Spawn above it and wait.
        activeState = State.patrol;
        
        yield return new WaitForSeconds(_respawnTime); // Wait to respawn.
        
        // Re activate everything.
        enabled = true;
        _animator.SetBool("enabled", enabled);
        _agent.enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;

    }

    /// Check for player.
    private void PlayerCheck()
    {
        RaycastHit hit;

        // Check for players in peripheral, forward, and diagonal directions. Vision cone.
        Vector3[] directions = new Vector3[] {transform.right / 2, -transform.right / 2, transform.forward, (transform.forward + transform.right).normalized, (transform.forward - transform.right).normalized };
        for (int i = 0; i < directions.Length; i++) // Check all directions.
        {
            if (Physics.Raycast(transform.position, directions[i], out hit, _visionRange))
            {
                if (hit.transform.gameObject.CompareTag("Player")) // Player seen.
                {
                    if (engage <= 0.1f && !_chaseSound.isPlaying)
                    {
                        _chaseSound.Play();
                    }
                    //Debug.Log("Player has been spotted!");
                    engage = _giveUpTime; // Reset the engage timer.
                    activeState = State.chase;
                }
            }
        }
    }

    /// Loop of sounds played randomly while patrolling.
    private IEnumerator SoundLoop()
    {
        yield return new WaitForSeconds(1);
        int r = Random.Range(6, 12);
        yield return new WaitForSeconds(r);
        if (!_patrolSound.isPlaying && !_chaseSound.isPlaying)
        {
            float pitch = Random.Range(0.7f, 0.9f); // Play at random pitch for variation.
            _patrolSound.pitch = pitch;
            _patrolSound.Play();
        }
        StartCoroutine(SoundLoop());
    }

    /// Helper for showing the enemy vision in editor.
    private void DisplayEditorDebugInfo()
    {
        Debug.DrawRay(transform.position, transform.right / 2 * _visionRange, Color.green); // 360 deg
        Debug.DrawRay(transform.position, -transform.right / 2 * _visionRange, Color.green); // 180 deg

        Debug.DrawRay(transform.position, (transform.forward + transform.right).normalized * _visionRange, Color.yellow); // 45 deg
        Debug.DrawRay(transform.position, (transform.forward - transform.right).normalized * _visionRange, Color.yellow); // 135 deg

        Debug.DrawRay(transform.position, transform.forward * _visionRange, Color.red); // 90 deg
    }
}
