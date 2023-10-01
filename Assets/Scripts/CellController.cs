using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    // Private fields
    private RingController _ring;
    private ICellContent _cellContent;

    // Properties
    public RingController Ring
    {
        get { return _ring; }
        set
        {
            if (_ring == null) {
                _ring = value;
            }
        }
    }
    public ICellContent CellContent
    {
        get { return _cellContent; }
        set {
            if (_cellContent != null) {
                _cellContent.CellDestroy();
            }
            if (value != null) {
                value.Cell = this;
            }
            _cellContent = value;
        }
    }


    public CellController GetNeighbour(Vector2 direction)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, GameManager.Instance.CellsLayer);
        foreach (RaycastHit2D hit2d in hits) {
            if (hit2d.collider.gameObject != gameObject)
            {
                return hit2d.collider.gameObject.GetComponent<CellController>();
            }
        }
        return null;
    }


    public void OnMouseDown()
    {
        GameManager.Instance.PerformCellAction(this);
    }
}
