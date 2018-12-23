using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pathfinder))]
public class PathMover : MonoBehaviour
{
    /// <summary>
    /// The movement speed this entity will move about the board with
    /// </summary>
    public float speed = 3f;
    /// <summary>
    /// The pathfinder (this is a required field, can't move without a path !)
    /// </summary>
    private Pathfinder pathfinder;
    /// <summary>
    /// The currently active path
    /// </summary>
    private List<Tile> path;
    /// <summary>
    /// The movement routine. We want to be able to intterrupt this !
    /// </summary>
    private Coroutine movementCoroutine;

    // Use this for initialization
    private void Start()
    {
        pathfinder = GetComponent<Pathfinder>();
        pathfinder.onPathCalculated += MoveObject;
    }

    /// <summary>
    /// Start a coroutine, move along a path,
    /// but make sure any current movement is halted first
    /// </summary>
    /// <param name="path">The curently calculated path</param>
    private void MoveObject(List<Tile> path)
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }
        this.path = path;
        movementCoroutine = StartCoroutine(MovementRoutine());
    }

    /// <summary>
    /// The actual routine that can be interrupted
    /// </summary>
    /// <returns>a movement routine</returns>
    private IEnumerator MovementRoutine()
    {
        foreach (Tile tile in path)
        {
            Vector3 itemPos = pathfinder.tileGrid.GetWorldPoint(new Vector3(tile.x, transform.position.y, tile.y));
            while (Vector3.Distance(transform.position, itemPos) > .0001f)
            {
                transform.position = Vector3.MoveTowards(transform.position, itemPos, speed * Time.deltaTime);
                yield return null;
            }
        }

    }

}
