using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // Static
    private static UIController _instance;

    // Inspector fields
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI towersText;
    [SerializeField] private TextMeshProUGUI shootButtonText;
    [SerializeField] private TextMeshProUGUI buildButtonText;
    [SerializeField] private Image shootButton;
    [SerializeField] private Color buttonActiveColor;
    [SerializeField] private Color buttonInactiveColor;

    // Private fields

    // Properties
    public static UIController Instance => _instance;


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
        GameManager.Instance.MoneyChanged += OnMoneyChanged;
        TowerManager.Instance.TowerSpawned += OnTowerSpawned;
        TowerManager.Instance.TowerDestroyed += OnTowerSpawned;
        HouseController.HouseBuilt += OnHouseEvent;
        HouseController.HouseDestroyed += OnHouseEvent;

        shootButtonText.text = "Shoot\n($" + GameManager.Instance.DestroyCost + ")";
        UpdateBuildButtonText();
    }


    private void FixedUpdate()
    {
        UpdateTimeText();
    }


    // public void Set
    private void OnMoneyChanged(int money)
    {
        moneyText.text = "$" + money;
        UpdateShootButton();
    }


    private void OnTowerSpawned(TowerController towerController)
    {
        towersText.text = TowerManager.Instance.Towers.Count + "/5";
        if (TowerManager.Instance.Towers.Count == 4)
        {
            towersText.color = Color.red;
        }
        else
        {
            towersText.color = Color.white;
        }
        UpdateShootButton();
    }


    private void OnHouseEvent(HouseController house)
    {
        UpdateBuildButtonText();
    }


    private void UpdateShootButton()
    {
        if (TowerManager.Instance.Towers.Count > 0 && GameManager.Instance.Money >= GameManager.Instance.DestroyCost)
        {
            shootButton.color = buttonActiveColor;
        }
        else
        {
            shootButton.color = buttonInactiveColor;
        }
    }


    private void UpdateTimeText()
    {
        int timeInSeconds = (int) Time.time;
        int minutes = (int)(timeInSeconds / 60);
        int seconds = (int)(timeInSeconds % 60);
        
        timeText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
    }


    private void UpdateBuildButtonText()
    {
        buildButtonText.text = "Build\n($" + GameManager.Instance.BuildCost + ")";
    }
}
