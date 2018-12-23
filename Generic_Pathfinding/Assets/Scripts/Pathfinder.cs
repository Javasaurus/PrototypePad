using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    /// <summary>
    /// There are multiple types of heuristics we can use. Heuristics can be regarded
    /// as the formula on how good a tile-choice is. Here we present 3 examples : 
    /// 1. Distance             ---> literally the linear distance between the points
    /// 2. Manhattan-Distance   ---> this is the distance to the target in a straight line
    /// 3. Combine              ---> the average between the above
    /// </summary>
    public enum Heuristics
    {
        DISTANCE, MANHATTAN_DISTANCE, COMBINED
    }

    /// <summary>
    /// The currently selected heuristics
    /// </summary>
    public Heuristics heuristics;

    /// <summary>
    /// The grid of tiles
    /// </summary>
    public TileGrid tileGrid;

    /// <summary>
    /// The target position, the position we want to go to on the grid
    /// </summary>
    public Vector2 target;

    /// <summary>
    /// The currently calculated path
    /// </summary>
    public List<Tile> path;

    /// <summary>
    /// A delegate that is called as soon as the path is calculated
    /// </summary>
    /// <param name="path"></param>
    public delegate void OnPathCalculated(List<Tile> path);
    public OnPathCalculated onPathCalculated;

    /// <summary>
    /// The pathfinding routine that can be cancelled if another (more) urgent path
    /// needs to be calculated
    /// </summary>
    private Coroutine pathFindingRoutine;

    public void Awake()
    {
        tileGrid.onSelectedTileChange += FindPathToTile;
    }

    /// <summary>
    /// Calculates the path to a target tile
    /// </summary>
    /// <param name="targetTile">The target tile (for example, a clicked tile)</param>
    public void FindPathToTile(Tile targetTile)
    {
        Clear();
        if (pathFindingRoutine != null)
        {
            StopCoroutine(pathFindingRoutine);
        }
        pathFindingRoutine = StartCoroutine(CalculatePath(targetTile));
    }

    /// <summary>
    /// The path calculation method
    /// </summary>
    /// <param name="targetTile"></param>
    /// <returns>the calculation routine</returns>
    private IEnumerator CalculatePath(Tile targetTile)
    {
        Tile startingTile = tileGrid.GetTile(transform.position);
        if (startingTile != null && targetTile != null && startingTile != targetTile)
        {
            List<Tile> tiles = GetPath(startingTile, targetTile);
            if (tiles != null)
            {
                onPathCalculated(tiles);
            }
            path = tiles;
        }
        pathFindingRoutine = null;
        yield return null;
    }

    /// <summary>
    /// This method is the core of an a star calculation.
    /// </summary>
    /// <param name="start">the starting tile</param>
    /// <param name="target">the target tile</param>
    /// <returns>The most optimal path A* could find. Returns null if there was no solution.</returns>
    private List<Tile> GetPath(Tile start, Tile target)
    {
        List<Tile> openSet = new List<Tile>();
        List<Tile> closedSet = new List<Tile>();
        List<Tile> arrivalZone = new List<Tile>();

        Tile[] targetNeighbors = GetNeighbors(target);
        foreach (Tile targetNeighbor in targetNeighbors)
        {
            if (targetNeighbor != null)
            {
                arrivalZone.Add(targetNeighbor);
            }
        }


        Dictionary<Tile, float[]> scores = new Dictionary<Tile, float[]>();
        Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();

        openSet.Add(start);
        SetScore(scores, start, CalculateHeuristic(start, target), 0);


        Tile current = start;
        while (openSet.Count > 0)
        {
            //find the best tile in the current open set (ergo the lowest f-score)
            current = GetBestEntry(openSet, scores);
            //add this one to the current path

            //if we reached the destination...
            if (current == target || (!target.walkable && arrivalZone.Contains(current)))
            {
                //Destination was reached !
                //break and return
                return ReconstructPath(cameFrom, current);
            }

            //move the currently best one from the open set into the closed set
            openSet.Remove(current);
            closedSet.Add(current);

            //get the neighbours for the current tile
            Tile[] neighbors = GetNeighbors(current);
            foreach (Tile neighbor in neighbors)
            {
                //if the neighbor exists, is not in the closed set (not visited before ) AND is walkable
                if (neighbor != null && !closedSet.Contains(neighbor) && neighbor.walkable)
                {
                    //calculate a temporary G score which is estimated as the G score of the current tile + the heuristic from the current to the neighbor
                    float tmpGScore = scores[current][1] + CalculateHeuristic(current, neighbor);

                    //if this is a completely new node it will not be in the open set yet, in that case we can add it to the open set and calculate its scores
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                        SetScore(scores, neighbor, tmpGScore + CalculateHeuristic(neighbor, target), tmpGScore);
                        SetChain(cameFrom, neighbor, current);
                    }
                    else
                    //if this node already was visited before, we should check if we have to update it if the new G-score is better than the old one
                    if (tmpGScore < scores[neighbor][1])
                    {
                        SetScore(scores, neighbor, tmpGScore + CalculateHeuristic(neighbor, target), tmpGScore);
                        SetChain(cameFrom, neighbor, current);
                    }
                }
            }
        }
        //here it means we found no solution...
        //means here that we did NOT find a solution...
        return null;
    }

    /// <summary>
    /// Reconstructs the path that was deliverd by an A* chain
    /// </summary>
    /// <param name="chain">The tile chain</param>
    /// <param name="current">the tile we currently are on</param>
    /// <returns></returns>
    private List<Tile> ReconstructPath(Dictionary<Tile, Tile> chain, Tile current)
    {
        List<Tile> path = new List<Tile>();
        //In the chain we recorded the next tile for every tile that is in the path. So
        //we can use this now to iterate all the keys and reconstruct the actual path
        while (chain.ContainsKey(current))
        {
            path.Add(current);
            current = chain[current];
        }
        //we now made the path that goes from the target to our position. We have to 
        //inverse this !
        path.Reverse();
        return path;
    }

    /// <summary>
    /// Convenience method to add a link between tiles into the chain
    /// </summary>
    /// <param name="chain">The current chain</param>
    /// <param name="child">The child tile</param>
    /// <param name="parent">The parent tile (tile the child tile came from)</param>
    private void SetChain(Dictionary<Tile, Tile> chain, Tile child, Tile parent)
    {
        if (chain.ContainsKey(child))
        {
            chain[child] = parent;
        }
        else
        {
            chain.Add(child, parent);
        }
    }

    /// <summary>
    /// Convenience method to update both the G and F scores. Keeping them in a single 
    /// dictionary is cheaper, as we only need to lookup a tile once. This also needs
    /// to be kept here instead of in a tile object as we want multiple pathfinders to
    /// operate at the same time, if the tiles themselves held this information we would
    /// constantly be overriding the values !
    /// </summary>
    /// <param name="scores">The dictionary of tiles and scores</param>
    /// <param name="tile">The tile that needs updating</param>
    /// <param name="fScore">the (new) fScore</param>
    /// <param name="gScore">the (new) gScore</param>
    private void SetScore(Dictionary<Tile, float[]> scores, Tile tile, float fScore, float gScore)
    {
        if (!scores.ContainsKey(tile))
        {
            scores.Add(tile, new float[] { fScore, gScore });
        }
        else
        {
            float[] scoreValues = scores[tile];
            scoreValues[0] = fScore;
            scoreValues[1] = gScore;
        }
    }

    /// <summary>
    /// Convenience method to set/update the gScore of a tile
    /// </summary>
    /// <param name="scores">The dictionary of tiles and scores</param>
    /// <param name="tile">The tile that needs updating</param>
    /// <param name="gScore">the (new) gScore</param>
    private void SetGScore(Dictionary<Tile, float[]> scores, Tile tile, float gScore)
    {
        float[] scoreValues = scores[tile];
        scoreValues[1] = gScore;
    }
    /// <summary>
    /// Convenience method to set/update the fScore of a tile
    /// </summary>
    /// <param name="scores">The dictionary of tiles and scores</param>
    /// <param name="tile">The tile that needs updating</param>
    /// <param name="gScore">the (new) gScore</param>
    private void SetFScore(Dictionary<Tile, float[]> scores, Tile tile, float fScore)
    {
        float[] scoreValues = scores[tile];
        scoreValues[0] = fScore;
    }

    /// <summary>
    /// Calculate the heuristics for the current tile and the tile we're moving to.
    /// This value can be modified using tile properties (for example, water tiles could be
    /// penalized).
    /// </summary>
    /// <param name="currentTile">The current tile</param>
    /// <param name="targetTile">The targetted tile</param>
    /// <returns> a heuristics score</returns>
    private float CalculateHeuristic(Tile currentTile, Tile targetTile)
    {
        switch (heuristics)
        {
            case Heuristics.DISTANCE:
                return GetDistance(currentTile, targetTile);
            case Heuristics.MANHATTAN_DISTANCE:
                return GetManhattanDistance(currentTile, targetTile);
            case Heuristics.COMBINED:
                float distance = GetDistance(currentTile, targetTile);
                float mDistance = GetManhattanDistance(currentTile, targetTile);
                return ((0.3f * distance) + (0.7f * mDistance)) / 2;
            default:
                return 0;
        }
    }

    /// <summary>
    /// Calculates the simple distance between two tiles
    /// </summary>
    /// <param name="currentTile"></param>
    /// <param name="targetTile"></param>
    /// <returns></returns>
    private float GetDistance(Tile currentTile, Tile targetTile)
    {
        return Mathf.Sqrt(Mathf.Pow(currentTile.x - targetTile.x, 2) + Mathf.Pow(currentTile.y - targetTile.y, 2));
    }

    /// <summary>
    /// Calculates the Manhattan distance between two tiles
    /// </summary>
    /// <param name="currentTile"></param>
    /// <param name="targetTile"></param>
    /// <returns></returns>
    private float GetManhattanDistance(Tile currentTile, Tile targetTile)
    {
        return Mathf.Abs(currentTile.x - targetTile.x) + Mathf.Abs(currentTile.y - targetTile.y);
    }

    /// <summary>
    /// Convenience method to find the entry with the lowest fScore in the current open set.
    /// We choose to manipulate the openset rather than the scores as it is a lot slower to
    /// sort the entire list of visited tiles versus the (small and shrinking) current set of open tiles.
    /// </summary>
    /// <param name="openSet"></param>
    /// <param name="scores"></param>
    /// <returns></returns>
    private Tile GetBestEntry(List<Tile> openSet, Dictionary<Tile, float[]> scores)
    {
        if (openSet.Count > 0)
        {
            //openset is going to be smaller than the total amount of scores, so we need to work form that perspective, 
            //avoid sorting all the scores all the time
            Tile best = openSet[0];
            float bestScore = float.MaxValue;
            foreach (Tile tile in openSet)
            {
                if (scores[tile][0] <= bestScore)
                {
                    best = tile;
                    bestScore = scores[tile][0];
                }
            }
            return best;
        }
        return null;
    }

    /// <summary>
    /// Convenience method to get the neighbouring tiles.
    /// This method only takes into account the orthogonal neighbours, but
    /// this can easily be amended when allowing for diagonal movement !
    /// </summary>
    /// <param name="tile">The current tile</param>
    /// <returns>An array of neighboring tiles</returns>
    private Tile[] GetNeighbors(Tile tile)
    {
        Tile[] neighbors = new Tile[4];
        int x = tile.x;
        int y = tile.y;

        neighbors[0] = GetTile(x, y + 1);
        neighbors[1] = GetTile(x + 1, y);
        neighbors[2] = GetTile(x, y - 1);
        neighbors[3] = GetTile(x - 1, y);

        return neighbors;
    }

    /// <summary>
    /// Gets a tile based on the int coordinates
    /// </summary>
    /// <param name="x"> the X-coordinate</param>
    /// <param name="y"> the Y-coordinate</param>
    /// <returns></returns>
    private Tile GetTile(int x, int y)
    {
        Tile[,] tileMap = tileGrid.tileMap;
        if (x < 0 || x >= tileMap.GetLength(0) || y < 0 || y >= tileMap.GetLength(1))
        {
            return null;
        }
        return tileMap[x, y];
    }

    //Debug methods...
    private void Clear()
    {
        Tile[,] tiles = tileGrid.tileMap;
        if (path != null)
        {
            path.Clear();
        }

    }

    /// <summary>
    /// It is always handy to have some form of visual representation of the pathfinding 
    /// result. It is however relatively expensive to do this in the game itself, so we
    /// try to draw it in the editor. This means we will be using Gizmos !
    /// </summary>
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (path != null)
        {
            foreach (Tile tile in path)
            {
                Vector3 GridDimensions = tileGrid.GridDimensions;
                float tileSize = tileGrid.tileSize;
                Gizmos.DrawCube(
                    new Vector3(
                    tile.x - ((GridDimensions.x - 1) / 2),
                    transform.position.y + 2,
                    tile.y - ((GridDimensions.y - 1) / 2)) * tileSize,
                    new Vector3(tileSize, 0.1f, tileSize));
            }

        }
    }

}


