using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private Transform[] _spawnPositions;

    [SerializeField] private float _startSpawnRate = 4;

    private void Start()
    {
        StartSpawning();
        GameManager.Instance.OnNextRound.AddListener(StartSpawning);
        GameManager.Instance.OnRoundComplete.AddListener(StopSpawning);
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPos = _spawnPositions[Random.Range(0, _spawnPositions.Length)].position;
        Instantiate(_enemyPrefab, spawnPos, Quaternion.identity);
    }

    private void StartSpawning()
    {
        float spawnRate = _startSpawnRate - 0.2f * (GameManager.Instance.CurrentRound - 1);
        InvokeRepeating(nameof(SpawnEnemy), 0, spawnRate);
    }

    private void StopSpawning()
    {
        CancelInvoke();
    }
}
