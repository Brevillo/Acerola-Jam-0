using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using OliverUtils;

public class ReaperMovement : MonoBehaviour {

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float spawnRadius;
    [SerializeField] private float groundDetectRadius;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private int maxSpawnAttempts;

    private Transform target;

    private void Start() {

        target = FindObjectOfType<Player>().transform;

        Vector3 spawnPoint = Vector3.zero;
        for (int i = 0; i < maxSpawnAttempts; i++) {

            Vector3 check = transform.position + Random.onUnitSphere.Flat().normalized * spawnRadius;

            if (NavMesh.SamplePosition(check, out var hit, groundDetectRadius, 1)) {
                spawnPoint = hit.position;
                break;
            }
        }

        transform.position = spawnPoint;
    }

    private void Update() {

        agent.SetDestination(Physics.Raycast(target.position, Vector3.down, out var hit, Mathf.Infinity, GameInfo.GroundMask) ? hit.point : target.position);
    }

    private void OnTriggerEnter(Collider other) {

        if (other.TryGetComponent(out PlayerHealth player)) {
            player.ReaperTouch();
            audioSource.Stop();
        }
    }
}
