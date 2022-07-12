using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using StarterAssets;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private DeathArena deathArena;
    [SerializeField] private ThirdPersonController _controller;
    [SerializeField] private List<Enemy> bossMinions;
    [SerializeField] private List<Enemy> arenaMinions;
    [SerializeField] private List<GameObject> enemiesOnBossScene;
    [SerializeField] public List<GameObject> EnemiesOnArena;
    [SerializeField] private Transform ArenaPos;
    [SerializeField] private Transform BossArenaPos;
    [SerializeField] private float _spawnMaxDistance;
    [SerializeField] private float _spawnMinDistance;
    [SerializeField] private float _spawnDelay;

    private int currentEnemyFromList; 

    private bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    private void Awake()
    {
        for (int i = 0; i < bossMinions.Count; i++)
        {
            for (int j = 0; j < bossMinions[i].count; j++)
            {
                GameObject enemyObject = Instantiate(bossMinions[i].prefab);
                enemiesOnBossScene.Add(enemyObject);
                enemyObject.SetActive(false);
            }
        }

        for (int i = 0; i < arenaMinions.Count; i++)
        {
            for (int j = 0; j < arenaMinions[i].count; j++)
            {
                GameObject enemyObject = Instantiate(arenaMinions[i].prefab);
                EnemiesOnArena.Add(enemyObject);
                enemyObject.SetActive(false);
            }
        }
        currentEnemyFromList = 0;
    }

    private void Start()
    {
        Shuffle(enemiesOnBossScene);
    }

    public void ArenaSummon() => StartCoroutine(ArenaSummonCorutine());
    public void EnemySummon(int enemiesCount) => StartCoroutine(BossSummonCorutine(enemiesCount));

    private void Shuffle(List<GameObject> list)
    {
        System.Random r = new System.Random();

        for (int i = list.Count - 1; i >= 1; i--)
        {
            int j = r.Next(i + 1);

            GameObject tmp = list[j];
            list[j] = list[i];
            enemiesOnBossScene[i] = tmp;
        }
    }

    private IEnumerator ArenaSummonCorutine()
    {
        for (int i = 0; i < EnemiesOnArena.Count; i++)
        {
            yield return new WaitForSeconds(_spawnDelay);
            Vector3 point;
            while (!(RandomPoint(ArenaPos.position, _spawnMaxDistance, out point) &&
                                Vector3.Distance(transform.position, point) > _spawnMinDistance))
            {
#if(UNITY_EDITOR)
                Debug.Log("Looking for a new point to spawn");
#endif
            }

            if (i > EnemiesOnArena.Count)
            {
                StopCoroutine(BossSummonCorutine(EnemiesOnArena.Count));
                break;
            }
            else
            {
                var enemyController = EnemiesOnArena[i].GetComponent<EnemyController>();
                enemyController.Target = _controller.transform;
                EnemiesOnArena[i].transform.position = point;
                EnemiesOnArena[i].SetActive(true);
                enemyController.Revive();
                enemyController.Agressive();
            }
        }
    }

    private IEnumerator BossSummonCorutine(int enemiesCount)
    {

        for (int i = currentEnemyFromList; i < enemiesOnBossScene.Count; i++)
        {
            yield return new WaitForSeconds(_spawnDelay);
            Vector3 point;

            while (!(RandomPoint(transform.position, _spawnMaxDistance, out point) &&
                                Vector3.Distance(transform.position, point) > _spawnMinDistance))
            {
#if(UNITY_EDITOR)
                Debug.Log("Looking for a new point to spawn");
#endif
            }

            if (i > (currentEnemyFromList + enemiesCount))
            {
                StopCoroutine(BossSummonCorutine(enemiesCount));
                break;
            }
            else
            {
                var enemyController = enemiesOnBossScene[i].GetComponent<EnemyController>();
                enemyController.Target = _controller.transform;
                enemiesOnBossScene[i].transform.position = point;
                enemiesOnBossScene[i].SetActive(true);
                enemyController.Revive();
                enemyController.Agressive();
            }
        }

        if (currentEnemyFromList <= enemiesOnBossScene.Count)
        {
            currentEnemyFromList += enemiesCount +1;
        }
        else
        {
            currentEnemyFromList = 0;
        }
    }

    public void OffAllEnemies()
    {
        foreach (GameObject enemy in enemiesOnBossScene)
        {
            enemy.SetActive(false);
        }
    }
}

[System.Serializable]
public class Enemy
{
    public int count;
    public GameObject prefab;
}
