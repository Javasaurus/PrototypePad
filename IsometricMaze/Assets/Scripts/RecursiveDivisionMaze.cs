using UnityEngine;

public class RecursiveDivisionMaze : VoxelMazeGenerator
{

    protected override byte[,,] ConstructMaze(Vector3 dimensions)
    {
        byte[,,] maze = new byte[(int)dimensions.x, (int)dimensions.y, (int)dimensions.z];
        int rMax = maze.GetUpperBound(0) - 1;
        int cMax = maze.GetUpperBound(2) - 1;
        int hMax = maze.GetUpperBound(1);

        MakeMaze(ref maze, rMax, cMax);

        return maze;
    }

    private void MakeMaze(ref byte[,,] maze, int rMax, int cMax)
    {
        //Create outside walls
        SetOutsideWalls(ref maze, rMax, cMax);
        //Make a maze inside the generated walls
        MakeMaze(ref maze, 0, cMax - 1, 0, rMax - 1);
        //Carve an opening to the outside, ensuring a solveable maze !
        MakeOpenings(ref maze, rMax, cMax);
    }
    /// <summary>
    /// Creates a maze within the given constructions within the rectangle described by left,right, top and bottom
    /// </summary>
    /// <param name="maze">The maze byte array</param>
    /// <param name="left">The left coordinate of a divisor</param>
    /// <param name="right">The right coordinate of a divisor</param>
    /// <param name="top">The top coordinate of a divisor</param>
    /// <param name="bottom">The bottom coordinate of a divisor</param>
    private void MakeMaze(ref byte[,,] maze, int left, int right, int top, int bottom)
    {
        int width = right - left;
        int height = bottom - top;

        //makes sure there is still room to divide with the minimal path size being 1, then picks the best
        //direction to divide into (if taller than wide, horizontal, else vertical, if width and height are 
        //the same, pick a random one)
        if (width > 2 && height > 2)
        {
            if (width > height)
            {
                DivideVertical(ref maze, left, right, top, bottom);
            }
            else if (height > width)
            {
                DivideHorizontal(ref maze, left, right, top, bottom);
            }
            else if (height == width)
            {
                if (Random.Range(0f, 1f) > 0.5f)
                {
                    DivideVertical(ref maze, left, right, top, bottom);
                }
                else
                {
                    DivideHorizontal(ref maze, left, right, top, bottom);
                }
            }
        }
        else if (width > 2 && height <= 2)
        {
            DivideVertical(ref maze, left, right, top, bottom);
        }
        else if (width <= 2 && height > 2)
        {
            DivideHorizontal(ref maze, left, right, top, bottom);
        }
    }

    /// <summary>
    /// Generates the outside walls of the maze
    /// </summary>
    /// <param name="maze">The maze byte array</param>
    /// <param name="rMax">The maximal amount of rows</param>
    /// <param name="cMax">The maximal amount of columns</param>
    private void SetOutsideWalls(ref byte[,,] maze, int rMax, int cMax)
    {
        //make the outter walls
        for (int i = 0; i < rMax; i++)
        {
            maze[i, 0, 0] = 1;
            maze[i, 0, cMax - 1] = 1;
        }
        for (int i = 0; i < cMax; i++)
        {
            maze[0, 0, i] = 1;
            maze[cMax - 1, 0, i] = 1;
        }
    }

    /// <summary>
    /// Divides a space in the maze vertically
    /// </summary>
    /// <param name="maze">The maze byte array</param>
    /// <param name="left">The left coordinate of a divisor</param>
    /// <param name="right">The right coordinate of a divisor</param>
    /// <param name="top">The top coordinate of a divisor</param>
    /// <param name="bottom">The bottom coordinate of a divisor</param>
    private void DivideVertical(ref byte[,,] maze, int left, int right, int top, int bottom)
    {
        Random rand = new Random();

        //find a random point to divide at
        //must be even to draw a wall there
        int divide = left + 2 + Random.Range(0, (right - left - 1) / 2) * 2;

        //draw a line at the halfway point
        for (int i = top; i < bottom; i++)
        {
            maze[i, 0, divide] = 1;
        }

        //get a random odd integer between top and bottom and clear it
        int clearSpace = top + Random.Range(0, (bottom - top) / 2) * 2 + 1;

        maze[clearSpace, 0, divide] = 0;

        MakeMaze(ref maze, left, divide, top, bottom);
        MakeMaze(ref maze, divide, right, top, bottom);
    }

    /// <summary>
    /// Divides a space in the maze horizontally
    /// </summary>
    /// <param name="maze">The maze byte array</param>
    /// <param name="left">The left coordinate of a divisor</param>
    /// <param name="right">The right coordinate of a divisor</param>
    /// <param name="top">The top coordinate of a divisor</param>
    /// <param name="bottom">The bottom coordinate of a divisor</param>
    private void DivideHorizontal(ref byte[,,] maze, int left, int right, int top, int bottom)
    {
        Random rand = new Random();

        //find a random point to divide at
        //must be even to draw a wall there
        int divide = top + 2 + Random.Range(0, (bottom - top - 1) / 2) * 2;
        if (divide % 2 == 1)
        {
            divide++;
        }

        //draw a line at the halfway point
        for (int i = left; i < right; i++)
        {
            maze[divide, 0, i] = 1;
        }

        //get a random odd integer between left and right and clear it
        int clearSpace = left + Random.Range(0, (right - left) / 2) * 2 + 1;

        maze[divide, 0, clearSpace] = 0;

        //recur for both parts of the newly split section
        MakeMaze(ref maze, left, right, top, divide);
        MakeMaze(ref maze, left, right, divide, bottom);
    }

    /// <summary>
    /// Generates an outcome for the maze (starting and finishing position)
    /// </summary>
    /// <param name="maze">The maze byte array</param>
    /// <param name="rMax">The maximal amount of rows</param>
    /// <param name="cMax">The maximal amount of columns</param>
    private void MakeOpenings(ref byte[,,] maze, int rMax, int cMax)
    {

        //a random location for the entrance and exit
        int entrance_row = Mathf.Min(Random.Range(1, rMax - 1) * 2 + 1, rMax - 2);
        int exit_row = Mathf.Min(Random.Range(1, rMax - 1) * 2 + 1, rMax - 2);

        //clear the locations

        //STARTING LOCATION
        maze[entrance_row, 0, 0] = 0;

        //EXIT LOCATION
        maze[exit_row, 0, cMax - 1] = 0;

    }

}
