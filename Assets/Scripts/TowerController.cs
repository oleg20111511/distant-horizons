using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TowerController : MonoBehaviour, ICellContent
{
    // Events
    public event System.Action<TowerController> Destroyed;

    // Inspector fields
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float animDuration;

    // Private fields
    public CellController _cell;

    public CellController Cell
    {
        get { return _cell; }
        set { _cell = value; }
    }

    private void Start()
    {
        BottomUpDraw();
    }


    private void OnDestroy()
    {
        Destroyed?.Invoke(this);
    }


    private void BottomUpDraw()
    {
        Material material = spriteRenderer.material;
        material.SetFloat("_Cutoff", 1f);
        material.DOFloat(0, "_Cutoff", animDuration);
    }

    public void CellDestroy()
    {
        Destroy(gameObject);
    }
}
