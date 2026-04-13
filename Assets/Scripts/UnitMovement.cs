using UnityEngine;
using System.Collections.Generic;

public class UnitMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float arrivalThreshold = 0.2f;
    public bool tileCenterMode = false;

    private LinkedList<Vector2> currentPath = new LinkedList<Vector2>();
    private PathFinder pathFinder;
    private BoxCollider2D boxCollider;

    private Grid grid;
    private Vector3Int lastCellPos;

    public float MovementSpeed => movementSpeed;
    public float ArrivalThreshold => arrivalThreshold;
    public bool HasPath => currentPath != null && currentPath.Count > 0;

    public Vector2 NextWaypoint
    {
        get
        {
            if (HasPath)
                return currentPath.First.Value;
            else 
                return (Vector2)transform.position;
        }
    }

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        pathFinder = PathFinder.instance;
        if (pathFinder == null)
        {
            Debug.LogError($"[{gameObject.name}] PathFinder instance를 찾을 수 없습니다!");
        }

        grid = FindFirstObjectByType<Grid>();
        if (grid != null)
        {
            lastCellPos = grid.WorldToCell(transform.position);
        }
    }

    public void SetDestination(Vector3 targetPosition)
    {
        if (pathFinder == null) return;

        Vector2 start = transform.position;
        Vector2 goal;

        if (tileCenterMode)
            goal = new Vector2((int)(targetPosition.x) + 0.5f, (int)(targetPosition.y) + 0.5f);
        else
            goal = targetPosition;

        currentPath.Clear();

        if (boxCollider != null)
        {
            currentPath = pathFinder.getShortestPath(start, goal, boxCollider);
        }
        else
        {
            currentPath = pathFinder.getShortestPath(start, goal, new Vector2(1f, 1f));
        }
    }

    public void Move()
    {
        if (!HasPath) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            currentPath.First.Value,
            movementSpeed * Time.deltaTime
        );

        if (grid != null && UnitManager.instance != null)
        {
            Vector3Int currentCellPos = grid.WorldToCell(transform.position);
            if (currentCellPos != lastCellPos)
            {
                UnitManager.instance.OnUnitMoved(gameObject, lastCellPos, currentCellPos);
                lastCellPos = currentCellPos;
            }
        }

        if (Vector2.Distance(transform.position, currentPath.First.Value) < arrivalThreshold)
        {
            currentPath.RemoveFirst();
        }
    }

    public void ClearPath()
    {
        currentPath.Clear();
    }

    public void SetSpeed(float speed)
    {
        movementSpeed = speed;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying && currentPath != null && currentPath.Count > 0)
        {
            Color originalColor = Gizmos.color;
            Gizmos.color = Color.green;

            foreach (var loc in currentPath)
                Gizmos.DrawCube(new Vector3(loc.x, loc.y, 0), new Vector3(0.5f, 0.5f, 0.5f));

            Gizmos.DrawLine(transform.position, currentPath.First.Value);

            for (LinkedListNode<Vector2> iter = currentPath.First; iter.Next != null; iter = iter.Next)
            {
                Vector3 from = iter.Value;
                Vector3 to = iter.Next.Value;
                Gizmos.DrawLine(from, to);
            }

            Gizmos.color = originalColor;
        }
    }
#endif
}
