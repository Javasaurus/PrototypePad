using UnityEngine;

public class VoxelWorld : MonoBehaviour
{
    /// <summary>
    /// The size of a maze chunk (y = the height of the maze)
    /// </summary>
    public Vector3 MazeChunkDimensions = new Vector3(25, 3, 25);
    /// <summary>
    /// The maze data (a byte is in this example set to 1 but ideally represents the type of block (for example 0=air, 1 = wall, 2 = breakable wall, etc etc)
    /// </summary>
    public byte[,,] mazeData;

    private void Awake()
    {
        LoadMaze();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadMaze();
        }
    }

    /// <summary>
    /// Loads a new maze into the voxel chunk
    /// </summary>
    private void LoadMaze()
    {
        mazeData = new RecursiveDivisionMaze().GenerateMaze(MazeChunkDimensions);
        GetComponentInChildren<VoxelChunk>().GenerateMesh(this, mazeData);
    }

    /// <summary>
    /// Returns the byte value of a certain block (representing its type)
    /// </summary>
    /// <param name="x">the x-coordinate</param>
    /// <param name="y">the y-coordinate</param>
    /// <param name="z">the z-coordinate</param>
    /// <returns> the block byte value </returns>
    public byte GetBlock(int x, int y, int z)
    {
        byte value = 0;
        if (x >= MazeChunkDimensions.x || x < 0 || y >= MazeChunkDimensions.y || y < 0 || z >= MazeChunkDimensions.z || z < 0)
        {
            //out of bounds
            value = (byte)0;
        }
        else
        {
            //in bounds
            value = mazeData[x, y, z];
        }
        return value;
    }
}
