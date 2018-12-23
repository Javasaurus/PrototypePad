using UnityEngine;

/// <summary>
/// This class moves an object to the clicked tile so we can see where the
/// Pathfinders will be moving to
/// </summary>
public class DebugIndicator : MonoBehaviour
{

    public TileGrid tileGrid;

    // Use this for initialization
    private void Start()
    {
        transform.position = Vector3.one * 9999;
        tileGrid.onSelectedTileChange += MoveToTile;
    }

    private void MoveToTile(Tile selectedTile)
    {
        transform.position = tileGrid.GetWorldPoint(new Vector3(selectedTile.x, transform.position.y, selectedTile.y));
    }


}
