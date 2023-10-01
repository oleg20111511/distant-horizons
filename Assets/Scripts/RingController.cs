using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class RingController : MonoBehaviour
{
    private static List<RingController> rings = new List<RingController>();

    // Inspector Fields
    [SerializeField] private List<CellController> cells;
    [SerializeField] private int ringLayer;

    // Properties
    public static ReadOnlyCollection<RingController> Rings => rings.AsReadOnly();

    public bool IsAccessible
    {
        get
        {
            // If there's 4 rings ([0], [1], [2], [3])...
            int towers = TowerManager.Instance.Towers.Count;  // and there is 1 tower...
            int furthersAvailableRing = Rings.Count - towers;  // Then ring [2] is the furthest available ring...
            return ringLayer < furthersAvailableRing;  // And this will return true for rings [2], [1] and [0]
        }
    }
    public ReadOnlyCollection<CellController> Cells => cells.AsReadOnly();
    public int RingLayer => ringLayer;



    void Awake()
    {
        foreach (CellController cell in cells) {
            cell.Ring = this;
        }

        rings.Add(this);
    }


    // Start is called before the first frame update
    private void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
