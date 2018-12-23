using UnityEngine;

public class TileGrid : MonoBehaviour
{
    /// <summary>
    /// A delegate that fires when the selected tile has changed. This selected units can register/unregister from this event 
    /// </summary>
    /// <param name="selectedTile"></param>
    public delegate void OnSelectedTileChange(Tile selectedTile);
    public OnSelectedTileChange onSelectedTileChange;
    /// <summary>
    /// The dimensions of the grid 
    /// </summary>
    public Vector2 GridDimensions = new Vector2(100, 100);
    /// <summary>
    /// The size of a tile (defaults to 1). This is the "resolution" of the A star
    /// </summary>
    public float tileSize;
    /// <summary>
    /// The actual map of tiles
    /// </summary>
    public Tile[,] tileMap;

    private void Start()
    {
        InitTileMap();
        onSelectedTileChange += LogSelectedTile;
    }

    /// <summary>
    /// Initializes the tile map. This is where the "procedural" data would have to come in later, but 
    /// for this tutorial we just randomly attribute a tile as "unwalkable". In an actual game you could
    /// include other tile types (water, height difference, etc.)
    /// </summary>
    private void InitTileMap()
    {
        tileMap = new Tile[(int)GridDimensions.x, (int)GridDimensions.y];
        for (int i = 0; i < tileMap.GetLength(0); i++)
        {
            for (int j = 0; j < tileMap.GetLength(1); j++)
            {
                tileMap[i, j] = new Tile(i, j);
                tileMap[i, j].walkable = Random.Range(0f, 1f) > 0.1f;
            }
        }
        tileSize = (1 / GridDimensions.x) * transform.localScale.x;
        //
        DebugSpawner spawner = GetComponent<DebugSpawner>();
        if (spawner)
        {
            spawner.SpawnMap(this);
        }
    }

    /// <summary>
    /// We use the on mouse over over the tile grid. In this case, this is a big cube that
    /// has a collider so it can be clicked.
    /// </summary>
    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            //we will try to "shoot" a ray from the mouse position to the world. That way we get the point
            //on the "chunk"/"floor"/"terrain"
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                //At this point we hit the floor. 
                //IMPORTANT : A tile is not a monobehaviour, it is just a data structure. This makes sure the A star algorithm will run a lot 
                //faster
                Tile selectedTile = GetTile(hit.point);
                if (selectedTile != null)
                {
                    //here we call the delegate function on the selected tile. You can consider this as "broadcasting" to whoever is currently
                    //registered that a tile was selected
                    onSelectedTileChange(selectedTile);
                }
            }
        }
    }

    /// <summary>
    /// This is just a method to show how to register to a delegate
    /// </summary>
    /// <param name="newSelectedTile"></param>
    public void LogSelectedTile(Tile newSelectedTile)
    {
        //      Debug.Log("New tile selected : " + newSelectedTile.x + "," + newSelectedTile.y);
    }

    /// <summary>
    /// A convenience method to get the tile position in the grid from a screen position
    /// </summary>
    /// <param name="point"></param>
    /// <returns>the tile position</returns>
    public Vector3 GetTilePosition(Vector3 point)
    {
        Vector3 tilePoint = new Vector3(
            point.x + GridDimensions.x * (tileSize / 2),
            point.y,
            point.z + GridDimensions.y * (tileSize / 2)
            );

        tilePoint /= tileSize;
        tilePoint = new Vector3(
            Mathf.FloorToInt(tilePoint.x),
            Mathf.FloorToInt(tilePoint.y),
            Mathf.FloorToInt(tilePoint.z)
            );
        //the tiles are spread around the center of this object, that means we need to "translate" the origin by half the size of the object
        return tilePoint;
    }

    /// <summary>
    ///  A convenience method to get a tile based on the screen position
    /// </summary>
    /// <param name="point"></param>
    /// <returns>A tile object</returns>
    public Tile GetTile(Vector3 point)
    {
        Vector3 tmp = GetTilePosition(point);
        return tileMap[(int)tmp.x, (int)tmp.z];
    }

    /// <summary>
    ///  A convenience method to get the world point starting from a tile
    /// </summary>
    /// <param name="tilePoint"></param>
    /// <returns></returns>
    public Vector3 GetWorldPoint(Vector3 tilePoint)
    {
        Vector3 worldPoint = new Vector3(
            tilePoint.x - GridDimensions.x * (tileSize / 2),
            tilePoint.y,
            tilePoint.z - GridDimensions.y * (tileSize / 2)
            );

        worldPoint *= tileSize;
        worldPoint = new Vector3(
            Mathf.FloorToInt(worldPoint.x),
            Mathf.FloorToInt(worldPoint.y),
            Mathf.FloorToInt(worldPoint.z)
            );
        //the tiles are spread around the center of this object, that means we need to "translate" the origin by half the size of the object
        return worldPoint;
    }



}
