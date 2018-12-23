[System.Serializable]
public class Tile
{
    /// <summary>
    /// A tile is a data structure that holds some information on the 
    /// terrain.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public Tile(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public int x;
    public int y;
    public bool walkable;


}
