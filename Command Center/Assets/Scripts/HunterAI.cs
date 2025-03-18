using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HunterAI : MonoBehaviour
{
    public bool enabled = true;

    [SerializeField] private Transform _enemy;
    [SerializeField] private Transform _player;

    [SerializeField] private NavMeshAgent _agent;

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
    }

    private void Patrol()
    {
        Debug.Log("Patrolling");
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

    private void Chase()
    {
        Debug.Log("Chasing");
        if (target != _player)
        {
            target = _player;
        }
    }

    private void Hunt()
    {
        Debug.Log("Hunting");
    }

    private void NextPatrolPoint()
    {
        patrolIndex++;
        if (patrolIndex >= _patrolPoints.Count) // Wrap back around to the first patrol point.
        {
            patrolIndex = 0;
        }
    }
}
