using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Static
    private static GameManager _instance;

    // Events
    public event System.Action<int> MoneyChanged;

    // Public fields
    public float finalTime;

    // Inspector fields
    [SerializeField] private LayerMask cellsLayer;
    [SerializeField] private int buildCost;
    [SerializeField] private int buildCostIncrease;
    [SerializeField] private int destroyCost;
    [SerializeField] private int towerDestroyReward = 50;
    [SerializeField] private int initialMoney;
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject housePrefab;
    
    // Private fields
    private int _money;

    // Properties
    public static GameManager Instance => _instance;
    public LayerMask CellsLayer => cellsLayer;
    public int DestroyCost => destroyCost;
    public int TowerDestroyReward => towerDestroyReward;
    public int Money
    {
        get => _money;
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            _money = value;
            MoneyChanged?.Invoke(_money);
        }
    }
    public int BuildCost
    {
        get
        {
            return buildCost + (buildCostIncrease * HouseController.Houses.Count);
        }
    }


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


    private void Start()
    {
        Money = initialMoney;
    }


    public void PerformCellAction(CellController cellController)
    {
        ShootCell(cellController);
    }


    public void Build()
    {
        if (Money < BuildCost)
        {
            return;
        }
        CellController targetCell = player.CurrentCell;
        if (targetCell.CellContent != null)
        {
            return;
        }
        Money -= BuildCost;
        GameObject house = Instantiate(housePrefab, targetCell.transform.position, quaternion.identity);
        HouseController houseController = house.GetComponent<HouseController>();
        targetCell.CellContent = houseController;
    }


    public void ShootRandomTower()
    {
        if (TowerManager.Instance.Towers.Count == 0)
        {
            return;
        }
        GameObject tower = TowerManager.Instance.Towers[UnityEngine.Random.Range(0, TowerManager.Instance.Towers.Count)];
        TowerController towerController = tower.GetComponent<TowerController>();
        ShootCell(towerController.Cell);
    }


    private void ShootCell(CellController cell)
    {
        if (Money < DestroyCost)
        {
            return;
        }
        if (cell.CellContent == null)
        {
            return;
        }
        if (player.State == PlayerController.PlayerState.Fighting)
        {
            return;
        }

        // Only shoot towers
        if (cell.CellContent.gameObject.GetComponent<TowerController>() != null)
        {
            Money -= DestroyCost;
            cell.CellContent = null;
        }
    }
}
