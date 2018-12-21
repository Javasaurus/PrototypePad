using UnityEngine;

public class PrimVoxelMaze : VoxelMazeGenerator
{
    public float placementThreshold = 0.1f;


    protected override byte[,,] ConstructMaze(Vector3 dimensions)
    {
        //THIS METHOD USES PRIM'S RANDOMIZED ALGORITHM TO CREATE A MAZE, 
        //I LEFT THIS IN BUT IN FACT THE RECURSIVE DIVISION WAS SIMPLER
        //TO EXPLAIN AND ALSO SEEMS TO DELIVER CLEANER RESULTS
        byte[,,] maze = new byte[(int)dimensions.x, (int)dimensions.y, (int)dimensions.z];
        int rMax = maze.GetUpperBound(0) - 1;
        int cMax = maze.GetUpperBound(2) - 1;
        int hMax = maze.GetUpperBound(1);

        for (int i = 0; i <= rMax; i++)
        {
            for (int k = 0; k <= cMax; k++)
            {
                //CREATE THE WALLS AROUND THE MAZE
                if (i == 0 || k == 0 || i == rMax || k == cMax)
                {
                    maze[i, 0, k] = 1;
                }

                //CHECK IF WE CAN ACTUALLY MAKE A PATH FROM THIS CELL OUT
                else if (i % 2 == 0 && k % 2 == 0)
                {
                    if (Random.value > placementThreshold)
                    {
                        //RANDOMLY GO TO A NEIGHBOUR
                        maze[i, 0, k] = 1;

                        int a = Random.value < .5 ? 0 : (Random.value < .5 ? -1 : 1);
                        int b = a != 0 ? 0 : (Random.value < .5 ? -1 : 1);
                        maze[i + a, 0, k + b] = 1;
                    }
                }
            }
        }


        return maze;
    }
}
