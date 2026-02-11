using UnityEngine;
using System;
using System.Collections.Generic;
using Priority_Queue;

public class PathFinder : MonoBehaviour
{
    [SerializeField] Vector2 gridStartPoint;
    [SerializeField] Vector2 gridEndPoint;
    [SerializeField] float cellSize = 0.5f;
    [SerializeField] Vector2 collisionCheckSensorSize = new Vector2(1, 1);
    [SerializeField] int priorityQueueMaxSize = 200;
    public LayerMask layerTocheckCollide;
    [SerializeField] bool optimizingPath = true;
    [SerializeField] Vector2 defaultUnitSize = new Vector2(1f, 1f);

    public static PathFinder instance;

    int numCols;
    int numRows;
    Nodes[,] nodes;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // 그리드 크기
        numCols = (int)((gridEndPoint.x - gridStartPoint.x) / cellSize + 0.5);
        numRows = (int)((gridEndPoint.y - gridStartPoint.y) / cellSize + 0.5);

        nodes = generateNodes();

        if (nodes == null)
        {
            Debug.LogError("PathFinder: nodes 생성 실패!");
        }
    }

    

    Nodes[,] generateNodes()
    {
        Nodes[,] nodes = new Nodes[numCols, numRows];

        for (int y = 0; y < numRows; y++)
        {
            for (int x = 0; x < numCols; x++)
            {
                Vector2 nodeCenter = new Vector2(
                    gridStartPoint.x + cellSize / 2 + x * cellSize,
                    gridStartPoint.y + cellSize / 2 + y * cellSize);

                bool isWall = null != Physics2D.OverlapBox(nodeCenter, collisionCheckSensorSize, 0, layerTocheckCollide);
                nodes[x, y] = new Nodes(x, y, nodeCenter, isWall);
            }
        }

        return nodes;
    }

    public LinkedList<Vector2> getShortestPath(Vector2 start, Vector2 goal, Vector2 unitSize)
    {
        return CalculatePath(start, goal, unitSize);
    }

    public LinkedList<Vector2> getShortestPath(Vector2 start, Vector2 goal, BoxCollider2D unitCollider)
    {
        Vector2 unitSize = unitCollider != null ? unitCollider.size : defaultUnitSize;
        return CalculatePath(start, goal, unitSize);
    }

    public LinkedList<Vector2> getShortestPath(Vector2 start, Vector2 goal)
    {
        return CalculatePath(start, goal, defaultUnitSize);
    }

    private LinkedList<Vector2> CalculatePath(Vector2 start, Vector2 goal, Vector2 unitSize)
    {

        if (nodes == null)
        {
            Debug.LogError("[PathFinder] nodes가 null입니다!");
            return new LinkedList<Vector2>();
        }

        FastPriorityQueue<Nodes> openList = new FastPriorityQueue<Nodes>(priorityQueueMaxSize);
        LinkedList<Nodes> closeList = new LinkedList<Nodes>();
        LinkedList<Nodes> nodeOnPathList = new LinkedList<Nodes>();
        LinkedList<Vector2> finalPath = new LinkedList<Vector2>();
        bool isPathFound = false;

        Nodes startNode = findNodeOnPosition(start);
        Nodes goalNode = findNodeOnPosition(goal);

        if (startNode == null)
        {
            Debug.LogError($"[PathFinder] 시작 노드를 찾을 수 없음: {start} (그리드 범위: {gridStartPoint} ~ {gridEndPoint})");
            return new LinkedList<Vector2>();
        }

        if (goalNode == null)
        {
            Debug.LogError($"[PathFinder] 목표 노드를 찾을 수 없음: {goal} (그리드 범위: {gridStartPoint} ~ {gridEndPoint})");
            return new LinkedList<Vector2>();
        }

        if (goalNode.isWall)
        {
            Debug.LogError($"[PathFinder] 목표 노드가 벽입니다! {goal}");
            return new LinkedList<Vector2>();
        }

        // 직선 경로 체크
        bool directPath = !Physics2D.BoxCast(
                start, unitSize, 0,
                goalNode.nodeCenter - start,
                Vector2.Distance(start, goal),
                layerTocheckCollide);

        if (directPath)
        {
            nodeOnPathList.AddFirst(startNode);
            nodeOnPathList.AddLast(goalNode);
            isPathFound = true;
        }
        else
        {
            isPathFound = findPath(startNode, goalNode, openList, closeList, nodeOnPathList);

            if (isPathFound)
            {
                excludeUselessWaypoints(nodeOnPathList);

                if (optimizingPath)
                {
                    optimizeWaypoints(nodeOnPathList, unitSize);
                }
            }
        }

        // 경로 생성
        if (isPathFound)
        {
            if (nodeOnPathList.Count > 0)
                nodeOnPathList.RemoveFirst();

            while (nodeOnPathList.Count > 1)
            {
                finalPath.AddLast(nodeOnPathList.First.Value.nodeCenter);
                nodeOnPathList.RemoveFirst();
            }

            finalPath.AddLast(goal);

            if (nodeOnPathList.Count > 0)
                nodeOnPathList.RemoveFirst();

            Debug.Log($"[PathFinder] 최종 경로 생성 완료: {finalPath.Count}개 웨이포인트");
        }
        else
        {
            Debug.LogError($"[PathFinder] 경로 찾기 실패!");
        }

        // 노드 상태 정리
        CleanupNodes(openList, closeList);

        return finalPath;
    }

    private void CleanupNodes(FastPriorityQueue<Nodes> openList, LinkedList<Nodes> closeList)
    {
        while (openList.Count > 0)
        {
            Nodes n = openList.Dequeue();
            n.parent = null;
            n.onOpenList = false;
            n.onCloseList = false;
        }

        foreach (Nodes n in closeList)
        {
            n.parent = null;
            n.onOpenList = false;
            n.onCloseList = false;
        }
    }

    bool findPath(Nodes start, Nodes goal, FastPriorityQueue<Nodes> openList, LinkedList<Nodes> closeList, LinkedList<Nodes> nodeOnPathList)
    {
        if (start == null || goal == null || start == goal)
        {
            Debug.LogWarning($"[PathFinder] findPath 조건 실패: start={start}, goal={goal}");
            return false;
        }

        openList.Enqueue(start, start.fCost);
        start.onOpenList = true;

        int iterations = 0;
        int maxIterations = numCols * numRows; // 무한루프 방지

        while (openList.Count > 0)
        {
            iterations++;
            if (iterations > maxIterations)
            {
                Debug.LogError($"[PathFinder] 최대 반복 횟수 초과! ({maxIterations})");
                return false;
            }

            Nodes nodeToFind = openList.Dequeue();
            closeList.AddFirst(nodeToFind);
            nodeToFind.onCloseList = true;
            nodeToFind.onOpenList = false;

            if (nodeToFind == goal)
            {
                Debug.Log($"[PathFinder] 목표 도달! 반복: {iterations}");
                makeWaypoints(goal, nodeOnPathList);
                return true;
            }

            jump(nodeToFind, goal, openList, closeList);
        }

        Debug.LogWarning($"[PathFinder] openList가 비었지만 목표에 도달하지 못함. 반복: {iterations}");
        return false;
    }

    void makeWaypoints(Nodes goalNode, LinkedList<Nodes> nodeOnPathList)
    {
        Nodes iterNode = goalNode;
        int count = 0;

        do
        {
            nodeOnPathList.AddFirst(iterNode);
            iterNode = iterNode.parent;
            count++;

            if (count > numCols * numRows)
            {
                Debug.LogError("[PathFinder] makeWaypoints 무한루프 감지!");
                break;
            }
        } while (iterNode != null);
    }

    void excludeUselessWaypoints(LinkedList<Nodes> nodeOnPathList)
    {
        if (nodeOnPathList.Count <= 2) return;
        LinkedListNode<Nodes> iter = nodeOnPathList.First;

        while (iter.Next != null && iter.Next.Next != null)
        {
            Nodes current = iter.Value;
            Nodes next = iter.Next.Value;
            Nodes nextnext = iter.Next.Next.Value;

            if ((current.XIndex < next.XIndex && next.XIndex < nextnext.XIndex
                || current.XIndex == next.XIndex && next.XIndex == nextnext.XIndex
                || current.XIndex > next.XIndex && next.XIndex > nextnext.XIndex) &&
                (current.YIndex < next.YIndex && next.YIndex < nextnext.YIndex
                || current.YIndex == next.YIndex && next.YIndex == nextnext.YIndex
                || current.YIndex > next.YIndex && next.YIndex > nextnext.YIndex))
            {
                nodeOnPathList.Remove(next);
            }
            else iter = iter.Next;
        }
    }

    void optimizeWaypoints(LinkedList<Nodes> nodeOnPathList, Vector2 unitSize)
    {
        if (nodeOnPathList.Count <= 2) return;

        LinkedListNode<Nodes> iter = nodeOnPathList.First;
        while (iter.Next != null && iter.Next.Next != null)
        {
            Nodes current = iter.Value;
            Nodes next = iter.Next.Value;
            Nodes nextnext = iter.Next.Next.Value;

            if (!Physics2D.BoxCast(
                current.nodeCenter, unitSize, 0,
                nextnext.nodeCenter - current.nodeCenter,
                Vector2.Distance(current.nodeCenter, nextnext.nodeCenter),
                layerTocheckCollide))
            {
                nodeOnPathList.Remove(next);
            }
            else iter = iter.Next;
        }
    }

    void jump(Nodes node, Nodes goalNode, FastPriorityQueue<Nodes> openList, LinkedList<Nodes> closeList)
    {
        Nodes parent = node.parent == null ? node : node.parent;

        if (parent.XIndex <= node.XIndex || parent.YIndex != node.YIndex)
            updateJumpPoints(jumpHorizontal(node, 1, goalNode), node, goalNode, openList, closeList);

        if (parent.XIndex >= node.XIndex || parent.YIndex != node.YIndex)
            updateJumpPoints(jumpHorizontal(node, -1, goalNode), node, goalNode, openList, closeList);

        if (parent.XIndex != node.XIndex || parent.YIndex <= node.YIndex)
            updateJumpPoints(jumpVertical(node, 1, goalNode), node, goalNode, openList, closeList);

        if (parent.XIndex != node.XIndex || parent.YIndex >= node.YIndex)
            updateJumpPoints(jumpVertical(node, -1, goalNode), node, goalNode, openList, closeList);

        if (parent.XIndex <= node.XIndex || parent.YIndex <= node.YIndex)
            updateJumpPoints(jumpDiagonal(node, 1, 1, goalNode), node, goalNode, openList, closeList);

        if (parent.XIndex >= node.XIndex || parent.YIndex <= node.YIndex)
            updateJumpPoints(jumpDiagonal(node, -1, 1, goalNode), node, goalNode, openList, closeList);

        if (parent.XIndex >= node.XIndex || parent.YIndex >= node.YIndex)
            updateJumpPoints(jumpDiagonal(node, -1, -1, goalNode), node, goalNode, openList, closeList);

        if (parent.XIndex <= node.XIndex || parent.YIndex >= node.YIndex)
            updateJumpPoints(jumpDiagonal(node, 1, -1, goalNode), node, goalNode, openList, closeList);
    }

    void updateJumpPoints(Nodes jumpEnd, Nodes jumpStart, Nodes goalNode, FastPriorityQueue<Nodes> openList, LinkedList<Nodes> closeList)
    {
        if (jumpEnd == null) return;
        if (jumpEnd.onCloseList) return;

        if (jumpEnd.onOpenList)
        {
            if (jumpEnd.gCost > jumpStart.gCost + Vector2.Distance(jumpStart.nodeCenter, jumpEnd.nodeCenter))
            {
                jumpEnd.parent = jumpStart;
                jumpEnd.gCost = jumpStart.gCost + Vector2.Distance(jumpEnd.nodeCenter, jumpStart.nodeCenter);
            }
            return;
        }
        else
        {
            jumpEnd.parent = jumpStart;
            jumpEnd.gCost = jumpStart.gCost + Vector2.Distance(jumpEnd.nodeCenter, jumpStart.nodeCenter);
            jumpEnd.hCost = Vector2.Distance(goalNode.nodeCenter, jumpEnd.nodeCenter);
            jumpEnd.onOpenList = true;
            openList.Enqueue(jumpEnd, jumpEnd.fCost);
        }
    }

    Nodes jumpHorizontal(Nodes start, int xDir, Nodes goalNode)
    {
        int currentXDir = start.XIndex;
        int currentYDir = start.YIndex;

        while (true)
        {
            currentXDir += xDir;

            if (!isWalkalbeAt(currentXDir, currentYDir)) return null;
            Nodes currentNode = nodes[currentXDir, currentYDir];

            if (currentNode == goalNode) return goalNode;

            if (isWalkalbeAt(currentXDir + xDir, currentYDir + 1) && !isWalkalbeAt(currentXDir, currentYDir + 1)
                || isWalkalbeAt(currentXDir + xDir, currentYDir - 1) && !isWalkalbeAt(currentXDir, currentYDir - 1))
            {
                return currentNode;
            }
        }
    }

    Nodes jumpVertical(Nodes start, int yDir, Nodes goalNode)
    {
        int currentXDir = start.XIndex;
        int currentYDir = start.YIndex;

        while (true)
        {
            currentYDir += yDir;

            if (!isWalkalbeAt(currentXDir, currentYDir)) return null;
            Nodes currentNode = nodes[currentXDir, currentYDir];

            if (currentNode == goalNode) return goalNode;

            if (isWalkalbeAt(currentXDir + 1, currentYDir + yDir) && !isWalkalbeAt(currentXDir + 1, currentYDir)
                || isWalkalbeAt(currentXDir - 1, currentYDir + yDir) && !isWalkalbeAt(currentXDir - 1, currentYDir))
            {
                return currentNode;
            }
        }
    }

    Nodes jumpDiagonal(Nodes start, int xDir, int yDir, Nodes goalNode)
    {
        int currentXDir = start.XIndex;
        int currentYDir = start.YIndex;

        while (true)
        {
            currentXDir += xDir;
            currentYDir += yDir;

            if (!isWalkalbeAt(currentXDir, currentYDir)) return null;
            Nodes currentNode = nodes[currentXDir, currentYDir];

            if (currentNode == goalNode) return goalNode;

            if (isWalkalbeAt(currentXDir - xDir, currentYDir + yDir) && !isWalkalbeAt(currentXDir - xDir, currentYDir)
                || isWalkalbeAt(currentXDir + xDir, currentYDir - yDir) && !isWalkalbeAt(currentXDir, currentYDir - yDir))
            {
                return currentNode;
            }

            Nodes temp;
            temp = jumpVertical(currentNode, yDir, goalNode);
            if (temp != null && !temp.onCloseList && !temp.onOpenList)
            {
                return currentNode;
            }
            temp = jumpHorizontal(currentNode, xDir, goalNode);
            if (temp != null && !temp.onCloseList && !temp.onOpenList)
            {
                return currentNode;
            }
        }
    }

    bool isWalkalbeAt(int x, int y)
    {
        return 0 <= x && x < numCols && 0 <= y && y < numRows && !nodes[x, y].isWall;
    }

    Nodes findNodeOnPosition(Vector2 position)
    {
        if (nodes == null)
        {
            Debug.LogError("[PathFinder] findNodeOnPosition: nodes가 null!");
            return null;
        }

        if (position.x < gridStartPoint.x || position.y < gridStartPoint.y
            || position.x > gridEndPoint.x || position.y > gridEndPoint.y)
        {
            Debug.LogWarning($"[PathFinder] 위치 {position}가 그리드 범위 밖 (범위: {gridStartPoint} ~ {gridEndPoint})");
            return null;
        }

        Vector2 relativePosition = position - gridStartPoint;
        int x = (int)(relativePosition.x / cellSize);
        int y = (int)(relativePosition.y / cellSize);

        if (x < 0 || x >= numCols || y < 0 || y >= numRows)
        {
            Debug.LogWarning($"[PathFinder] 계산된 인덱스 [{x}, {y}]가 범위 밖 (그리드: {numCols} x {numRows})");
            return null;
        }

        return nodes[x, y];
    }

#if DEBUG
    [SerializeField] bool DrawGizmo;

    void OnDrawGizmos()
    {
        if (DrawGizmo && nodes != null)
        {
            drawGridLine();
            drawObstacles();
        }
    }

    void drawGridLine()
    {
        Gizmos.color = Color.gray;

        for (int i = 0; i <= numCols; i++)
        {
            Vector2 start = new Vector2(gridStartPoint.x + i * cellSize, gridStartPoint.y);
            Vector2 end = new Vector2(gridStartPoint.x + i * cellSize, gridEndPoint.y);
            Gizmos.DrawLine(start, end);
        }

        for (int i = 0; i <= numRows; i++)
        {
            Vector2 start = new Vector2(gridStartPoint.x, gridStartPoint.y + i * cellSize);
            Vector2 end = new Vector2(gridEndPoint.x, gridStartPoint.y + i * cellSize);
            Gizmos.DrawLine(start, end);
        }
    }

    void drawObstacles()
    {
        Color red = new Color(1, 0, 0, 0.5f);
        for (int y = 0; y < numRows; y++)
        {
            for (int x = 0; x < numCols; x++)
            {
                if (nodes[x, y].isWall)
                {
                    Gizmos.color = red;
                    Gizmos.DrawCube(nodes[x, y].nodeCenter, new Vector2(cellSize, cellSize));
                }
            }
        }
    }
#endif
}