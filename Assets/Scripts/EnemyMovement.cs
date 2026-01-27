using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour {
    public Transform player;
    private NavMeshAgent _navMeshAgent;

    private void Start() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        if(player) {
            _navMeshAgent.SetDestination(player.position);
        }
    }
}