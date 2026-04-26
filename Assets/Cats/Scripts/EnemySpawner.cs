using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public float spawnInterval = 2f;
    public bool autoStart = true;
    public int enemyNumber = 10;

    [Header("Edge Definition")]
    public Transform startPoint;
    public Transform endPoint;

    private void Start()
    {
        if (autoStart)
        {
            StartCoroutine(SpawnRoutine());
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnEnemy();

            if(--enemyNumber < 1)
            {
                Destroy(gameObject);
                yield break;
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        if (startPoint == null || endPoint == null)
            return;

        float t = Random.value; // 0 ? 1
        Vector2 spawnPos = Vector2.Lerp(startPoint.position, endPoint.position, t);

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    // ?? Draw the edge in editor
    private void OnDrawGizmos()
    {
        if (startPoint == null || endPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(startPoint.position, endPoint.position);

        // Optional: draw endpoints
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(startPoint.position, 0.15f);
        Gizmos.DrawSphere(endPoint.position, 0.15f);
    }
}