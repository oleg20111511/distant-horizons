using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class TowerManager : MonoBehaviour
{
    // Static
    private static TowerManager _instance;

    // Events
    public event Action<TowerController> TowerSpawned;
    public event Action<TowerController> TowerDestroyed;

    // Inspector fields
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private float delayLowerBound;
    [SerializeField] private float delayUpperBound;
    [SerializeField] private float delayDecrease;

    // Fields
    private List<GameObject> towers = new List<GameObject>();
    private int maxTowers;
    
    // Properties
    public static TowerManager Instance => _instance;
    public ReadOnlyCollection<GameObject> Towers => towers.AsReadOnly();


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        maxTowers = RingController.Rings.Count - 1;
        StartCoroutine(TowerSpawner());
    }


    private IEnumerator TowerSpawner()
    {
        while (true)
        {
            float waitPeriod = Random.Range(delayLowerBound, delayUpperBound);
            yield return new WaitForSeconds(waitPeriod);
            if (Towers.Count < maxTowers)
            {
                SpawnTower();
                IncreaseDifficulty();
            }
            else
            {
                GameOver();
                break;
            }
        }
        
    }


    private void IncreaseDifficulty()
    {
        delayLowerBound -= delayDecrease;
        delayUpperBound -= delayDecrease;
        if (delayDecrease > 0.05f)
        {
            delayDecrease *= 0.9f;
        }
        if (delayLowerBound < 0.5f)
        {
            delayLowerBound = 0.5f;
        }
        if (delayUpperBound < 0.75f)
        {
            delayUpperBound = 0.75f;
        }
    }


    private CellController GetRandomAccessibleCell()
    {
        List<CellController> accessibleCells = new List<CellController>();
        foreach (RingController ringController in RingController.Rings)
        {
            if (!ringController.IsAccessible && ringController.RingLayer != 0)
            {
                continue;
            }
            accessibleCells.AddRange(ringController.Cells);
        }

        if (accessibleCells.Count > 0)
        {
            return accessibleCells[Random.Range(0, accessibleCells.Count)];
        }
        else
        {
            return null;
        }
    }


    private void SpawnTower()
    {
        CellController cell = GetRandomAccessibleCell();
        if (cell != null)
        {
            SpawnTower(cell);
        }
    }


    private void SpawnTower(CellController cell)
    {
        GameObject tower = Instantiate(towerPrefab, cell.transform.position, quaternion.identity);
        TowerController towerController = tower.GetComponent<TowerController>();
        towers.Add(tower);
        cell.CellContent = towerController;
        towerController.Destroyed += TowerDestroyedHandler;
        TowerSpawned?.Invoke(towerController);
    }


    private void TowerDestroyedHandler(TowerController tower)
    {
        towers.Remove(tower.gameObject);
        TowerDestroyed?.Invoke(tower);
    }


    private void GameOver()
    {
        GameManager.Instance.finalTime = Time.time;
        SceneManager.LoadScene("GameOver");
    }
}
