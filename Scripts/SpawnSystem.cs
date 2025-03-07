using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SpawnSystem : MonoBehaviour
{
    public static SpawnSystem instance;

    [SerializeField] private Transform spawnListT;

    [Header("�e��v���n�u�w��")]
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("�X�|�[���ʒu�w��")]
    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private float spawnInterval = 3.0f;
    [SerializeField] private float difficultyUpInterval = 60.0f;

    private float difficultyMultiplier = 1.0f;
    private bool isSpawning = false;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("SpawnSystem���V�[���ɕ������݂���");
            Destroy(gameObject);
        }
    }

    public void StartSpawning()
    {
        if (isSpawning) return;

        isSpawning = true;
        StartSpawnRoutine().Forget();
        StartDifficultyUpRoutine().Forget();
    }

    public void StopSpawning()
    {
        isSpawning = false;

        //��Փx(�X�|�[���Ԋu)�����Z�b�g
        difficultyMultiplier = 1.0f;
    }

    private async UniTaskVoid StartSpawnRoutine()
    {
        while (isSpawning)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(spawnInterval / difficultyMultiplier));
            if (isSpawning)
            {
                SpawnRandomObject();
            }
        }
    }

    private async UniTaskVoid StartDifficultyUpRoutine()
    {
        while (isSpawning)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(difficultyUpInterval));
            if (isSpawning)
            {
                difficultyMultiplier += 0.1f;
            }
        }
    }

    private void SpawnRandomObject()
    {
        float value = UnityEngine.Random.Range(0f, 1f);
        if (value < 0.7f)
        {
            Spawn(enemyPrefabs, spawnPoints);
        }
        else
        {
            Spawn(itemPrefabs, spawnPoints);
        }
    }

    // �w�肳�ꂽ�v���n�u�ƃX�|�[���ʒu����ɃX�|�[���������s��
    private void Spawn(GameObject[] prefabs, Transform[] spawnPoints)
    {
        if (prefabs.Length > 0 && 
            spawnPoints.Length > 0)
        {
            //�����_���ɑI��ŃX�|�[��
            GameObject prefab = prefabs[UnityEngine.Random.Range(0, prefabs.Length)];
            Vector3 spawnPos = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position;

            var spawnObj = Instantiate(prefab, spawnPos, Quaternion.identity);
            spawnObj.transform.SetParent(spawnListT);
        }
    }
}