using UnityEngine;

public interface ICellContent
{
    public GameObject gameObject{ get; }
    public CellController Cell { get; set; }

    public void CellDestroy();
}
