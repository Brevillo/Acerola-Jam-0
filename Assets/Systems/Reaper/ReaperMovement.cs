using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using OliverUtils;

public class ReaperMovement : MonoBehaviour {

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float spawnRadius;
    [SerializeField] private AudioSource audioSource;

    private Transform target;

    private void Start() {

        target = FindObjectOfType<Player>().transform;

        Vector3 dir = Random.onUnitSphere * spawnRadius;
        NavMesh.SamplePosition(transform.position + dir, out var hit, spawnRadius, 1);
        transform.position = hit.position;
    }

    private void Update() {

        agent.SetDestination(Physics.Raycast(target.position, Vector3.down, out var hit, Mathf.Infinity, GameInfo.GroundMask) ? hit.point : target.position);
    }

    private void OnTriggerEnter(Collider other) {

        if (other.TryGetComponentInParent(out PlayerHealth player)) {
            player.ReaperTouch();
            audioSource.Stop();
        }
    }
}
