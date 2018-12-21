using UnityEngine;

public abstract class VoxelMazeGenerator
{
    /// <summary>
    /// Generates a maze of given dimensions
    /// </summary>
    /// <param name="dimensions">The dimensions of a maze</param>
    /// <returns>a 3D byte array containing block type bytes</returns>
    public byte[,,] GenerateMaze(Vector3 dimensions)
    {
        byte[,,] maze = ConstructMaze(dimensions);
        SetMazeHeight(ref maze, dimensions);
        return maze;
    }
    /// <summary>
    /// Constructs the maze using a given implementation
    /// </summary>
    /// <param name="dimensions">The dimensions of a maze</param>
    /// <returns>a 3D byte array containing block type bytes</returns>
    protected abstract byte[,,] ConstructMaze(Vector3 dimensions);

    /// <summary>
    /// Inflates the maze to a given height (for now it just sets it to the value in the dimensions.y)
    /// </summary>
    /// <param name="maze">The maze that needs to receive height</param>
    /// <param name="dimensions">The given dimensions</param>
    private void SetMazeHeight(ref byte[,,] maze, Vector3 dimensions)
    {
        int rMax = maze.GetUpperBound(0) - 1;
        int cMax = maze.GetUpperBound(2) - 1;
        int hMax = maze.GetUpperBound(1);
        //add wall height
        for (int i = 0; i <= rMax; i++)
        {
            for (int k = 0; k <= cMax; k++)
            {
                if (maze[i, 0, k] != 0)
                {
                    for (int j = 0; j < hMax; j++)
                    {
                        maze[i, j, k] = 1;
                        //RANDOMLY BREAK here?
                    }
                }
            }
        }
    }




}
