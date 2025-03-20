using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HunterAI : MonoBehaviour
{
    public bool enabled = true;
    public bool displayDebug = true;

    [SerializeField] private Transform _enemy;
    [SerializeField] private Transform _player;

    [SerializeField] private NavMeshAgent _agent;

    [SerializeField] private float _visionRange;

    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _chaseSpeed;
    private float speed;

    public enum State
    { 
        patrol,
        chase,
        hunt
    }

    public State activeState;

    [SerializeField] private List<Transform> _patrolPoints;
    private int patrolIndex;

    private Transform target;

    private void Update()
    {
        if (enabled)
        {
            UpdateState();
            PlayerCheck();
        }

        if (displayDebug)
        {
            DisplayEditorDebugInfo();
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
        speed = _chaseSpeed;
        if (target != _player)
        {
            target = _player;
        }
    }

    /// Go investigate a certain position, possibly an old location of the player.
    private void Hunt()
    {
        //Debug.Log("Hunting");
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
                    Debug.Log("Player has been spotted!");
                    activeState = State.chase;
                }
            }
        }
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
