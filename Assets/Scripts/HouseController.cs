using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public class HouseController : MonoBehaviour, ICellContent
{
    private static List<HouseController> houses = new List<HouseController>();
    public static event Action<HouseController> HouseBuilt;
    public static event Action<HouseController> HouseDestroyed;

    [SerializeField] private int baseMoneyPerTick;
    [SerializeField] private float ringMultiplier;
    [SerializeField] private float tickDuration;
    [SerializeField] private int maxHP;
    [SerializeField] private Slider slider;
    [SerializeField] private int hpPerTick;

    // Private fields
    private CellController cell;
    private int _hp;
    private float lastDecay;

    // Properties
    public CellController Cell
    {
        get { return cell; }
        set { cell = value; }
    }

    public int HP
    {
        get { return _hp; }
        set {
            _hp = value;
            slider.value = (float)_hp / maxHP;
            if (_hp < maxHP && !slider.gameObject.activeSelf)
            {
                slider.gameObject.SetActive(true);
            }
            else if (_hp == maxHP && slider.gameObject.activeSelf)
            {
                slider.gameObject.SetActive(false);
            }
        }
    }

    public static ReadOnlyCollection<HouseController> Houses => houses.AsReadOnly();



    private void Awake()
    {
        houses.Add(this);
        HouseBuilt?.Invoke(this);
    }


    private void Start()
    {
        StartCoroutine(Work());
        HP = maxHP;
    }


    private void FixedUpdate()
    {
        if (!cell.Ring.IsAccessible)
        {
            Decay();
        }
        else if (HP < maxHP)
        {
            HP = maxHP;
        }
    }


    private void OnDestroy()
    {
        houses.Remove(this);
        HouseDestroyed?.Invoke(this);
    }


    public void CellDestroy()
    {
        Destroy(gameObject);
    }


    private IEnumerator Work()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickDuration);
            float moneyIncrease = baseMoneyPerTick + (baseMoneyPerTick * cell.Ring.RingLayer * ringMultiplier);
            GameManager.Instance.Money += (int) moneyIncrease;
        }
    }


    private void Decay()
    {

        if ((Time.time - lastDecay) < tickDuration)
        {
            return;
        }

        lastDecay = Time.time;
        HP -= hpPerTick;

        if (HP <= 0)
        {
            cell.CellContent = null;
        }
    }
}
