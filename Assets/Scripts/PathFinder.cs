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

    BoxCollider2D boxcollider;

    public static PathFinder instance;

    int numCols;
    int numRows;

    Nodes[,] nodes;

    Nodes goalNode;
    Nodes startNode;


    FastPriorityQueue<Nodes> openList;
    LinkedList<Nodes> closeList;

    LinkedList<Nodes> nodeOnPathList;

    LinkedList<Vector2> finalPath;

    bool isPathFound;

    private void Awake()
    {
        boxcollider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        numCols = (int)((gridEndPoint.x - gridStartPoint.x) / cellSize + 0.5);
        numRows = (int)((gridEndPoint.y - gridStartPoint.y) / cellSize + 0.5);
        nodes = generateNodes();

        openList = new FastPriorityQueue<Nodes>(priorityQueueMaxSize);
        closeList = new LinkedList<Nodes>();

        nodeOnPathList = new LinkedList<Nodes>();
        finalPath = new LinkedList<Vector2>();

        goalNode = findNodeOnPosition(transform.position);
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

                bool isWall =
                    null != Physics2D.OverlapBox(nodeCenter, collisionCheckSensorSize, 0, layerTocheckCollide);


                nodes[x, y] = new Nodes(x, y, nodeCenter, isWall);

            }
        }
        return nodes;
    }

    public LinkedList<Vector2> getShortestPath(Vector2 start, Vector2 goal)
    {
        isPathFound = false;
        finalPath.Clear();

        startNode = findNodeOnPosition(start);
        goalNode = findNodeOnPosition(goal);


        if (!Physics2D.BoxCast(
                gameObject.transform.position, boxcollider.size, 0,
                goalNode.nodeCenter - start, Vector2.Distance(start, goal), layerTocheckCollide))
        {
            nodeOnPathList.AddFirst(startNode);
            nodeOnPathList.AddFirst(goalNode);
            isPathFound = true;
        }
        else
        {
            findPath(startNode, goalNode); // start A* + JPS algorithm

            excludeUselessWaypoints();
            if (optimizingPath) optimizeWaypoints();
        }

        if (isPathFound)
        {
            nodeOnPathList.RemoveFirst();

            while (nodeOnPathList.Count > 1)
            {
                finalPath.AddLast(nodeOnPathList.First.Value.nodeCenter);
                nodeOnPathList.RemoveFirst();
            }
            finalPath.AddLast(goal);
            nodeOnPathList.RemoveFirst();
        }

        return finalPath;
    }

    void initOpenList()
    {
        while (openList.Count > 0)
        {
            Nodes n = openList.Dequeue();
            n.parent = null;
            n.onOpenList = false;
        }

    }

    void initCloseList()
    {
        while (closeList.Count > 0)
        {
            LinkedListNode<Nodes> n = closeList.First;
            n.Value.parent = null;
            n.Value.onCloseList = false;

            closeList.RemoveFirst();
        }
    }



    void findPath(Nodes start, Nodes goal) // find path with a* algorithm
    {

        initCloseList();
        initOpenList();
        nodeOnPathList.Clear();

        if (start == null || goal == null || start == goal) return;

        openList.Enqueue(start, start.fCost);
        start.onOpenList = true;

        while (openList.Count > 0)
        {

            Nodes nodeToFind = openList.Dequeue();
            closeList.AddFirst(nodeToFind);
            nodeToFind.onCloseList = true;
            nodeToFind.onOpenList = false;// move node from open list to close list


            if (nodeToFind == goal)
            {
                isPathFound = true;
                break;
            }

            jump(nodeToFind);
        }

        makeWaypoints();
    }

    void makeWaypoints()
    {
        if (!isPathFound) return;

        Nodes iterNode = goalNode;

        do
        {
            nodeOnPathList.AddFirst(iterNode);
            iterNode = iterNode.parent;
        } while (iterNode != null);

    }

    void excludeUselessWaypoints()
    {
        if (nodeOnPathList.Count <= 2) return;
        LinkedListNode<Nodes> iter = nodeOnPathList.First;

        while (iter.Next.Next != null)
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


    Nodes current;
    Nodes next;
    Nodes nextnext;
    void optimizeWaypoints()
    {

        if (nodeOnPathList.Count <= 2) return;

        LinkedListNode<Nodes> iter = nodeOnPathList.First;
        while (iter.Next.Next != null)
        {
            current = iter.Value;
            next = iter.Next.Value;
            nextnext = iter.Next.Next.Value;
            if (
                !Physics2D.BoxCast(current.nodeCenter, boxcollider.size, 0,
                nextnext.nodeCenter - current.nodeCenter,
                Vector2.Distance(current.nodeCenter, nextnext.nodeCenter),
                layerTocheckCollide))
            {
                nodeOnPathList.Remove(next);
            }
            else iter = iter.Next;
        }

    }





    void jump(Nodes node) // find jump point from node variable and add jump point to open list.
    {
        Nodes parent;

        if (node.parent == null) parent = node;
        else parent = node.parent;

        if (parent.XIndex <= node.XIndex || parent.YIndex != node.YIndex)
            updateJumpPoints(jumpHorizontal(node, 1), node);

        if (parent.XIndex >= node.XIndex || parent.YIndex != node.YIndex)
            updateJumpPoints(jumpHorizontal(node, -1), node);

        if (parent.XIndex != node.XIndex || parent.YIndex <= node.YIndex)
            updateJumpPoints(jumpVertical(node, 1), node);

        if (parent.XIndex != node.XIndex || parent.YIndex >= node.YIndex)
            updateJumpPoints(jumpVertical(node, -1), node);


        if (parent.XIndex <= node.XIndex || parent.YIndex <= node.YIndex)
            updateJumpPoints(jumpDiagonal(node, 1, 1), node);

        if (parent.XIndex >= node.XIndex || parent.YIndex <= node.YIndex)
            updateJumpPoints(jumpDiagonal(node, -1, 1), node);

        if (parent.XIndex >= node.XIndex || parent.YIndex >= node.YIndex)
            updateJumpPoints(jumpDiagonal(node, -1, -1), node);

        if (parent.XIndex <= node.XIndex || parent.YIndex >= node.YIndex)
            updateJumpPoints(jumpDiagonal(node, 1, -1), node);
    }

    void updateJumpPoints(Nodes jumpEnd, Nodes jumpStart)
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
            jumpEnd.hCost = Vector2.Distance(goalNode.nodeCenter, jumpEnd.nodeCenter); // update distance
            jumpEnd.onOpenList = true;
            openList.Enqueue(jumpEnd, jumpEnd.fCost);
        }

    }

    Nodes jumpHorizontal(Nodes start, int xDir)
    {
        int currentXDir = start.XIndex;
        int currentYDir = start.YIndex;
        Nodes currentNode;

        while (true)
        {
            currentXDir += xDir;

            if (!isWalkalbeAt(currentXDir, currentYDir)) return null;
            currentNode = nodes[currentXDir, currentYDir];

            if (currentNode == goalNode)
            {

                return goalNode;
            }

            if (isWalkalbeAt(currentXDir + xDir, currentYDir + 1) && !isWalkalbeAt(currentXDir, currentYDir + 1)
                || isWalkalbeAt(currentXDir + xDir, currentYDir - 1) && !isWalkalbeAt(currentXDir, currentYDir - 1))
            {

                return currentNode;

            }
        }
    }

    Nodes jumpVertical(Nodes start, int yDir)
    {

        int currentXDir = start.XIndex;
        int currentYDir = start.YIndex;
        Nodes currentNode;

        while (true)
        {

            currentYDir += yDir;

            if (!isWalkalbeAt(currentXDir, currentYDir)) return null;
            currentNode = nodes[currentXDir, currentYDir];

            if (currentNode == goalNode)
            {
                return goalNode;
            }

            if (isWalkalbeAt(currentXDir + 1, currentYDir + yDir) && !isWalkalbeAt(currentXDir + 1, currentYDir)
                || isWalkalbeAt(currentXDir - 1, currentYDir + yDir) && !isWalkalbeAt(currentXDir - 1, currentYDir))
            {
                return currentNode;
            }
        }

    }

    Nodes jumpDiagonal(Nodes start, int xDir, int yDir) // if parent node and 
    {
        int currentXDir = start.XIndex;
        int currentYDir = start.YIndex;
        Nodes currentNode;

        while (true)
        {
            currentXDir += xDir;
            currentYDir += yDir;


            if (!isWalkalbeAt(currentXDir, currentYDir)) return null;
            currentNode = nodes[currentXDir, currentYDir];

            if (currentNode == goalNode)
            {

                return goalNode;
            }
            if (isWalkalbeAt(currentXDir - xDir, currentYDir + yDir) && !isWalkalbeAt(currentXDir - xDir, currentYDir)
            || isWalkalbeAt(currentXDir + xDir, currentYDir - yDir) && !isWalkalbeAt(currentXDir, currentYDir - yDir)
                )
            {

                return currentNode;
            }

            Nodes temp;
            temp = jumpVertical(currentNode, yDir);
            if (temp != null && !temp.onCloseList && !temp.onOpenList)
            {
                return currentNode;
            }
            temp = jumpHorizontal(currentNode, xDir);
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




    bool areTwoNodesStraight(Nodes node1, Nodes node2)
    {
        return node1.XIndex == node2.XIndex ||
            node1.YIndex == node2.YIndex;
    }

    Nodes findNodeOnPosition(Vector2 position)
    {
        if (nodes == null) return null;

        if (position.x < gridStartPoint.x || position.y < gridStartPoint.y
            || position.x > gridEndPoint.x || position.y > gridEndPoint.y) return null;

        Vector2 relativePosition = position - gridStartPoint;
        int x = (int)(relativePosition.x / cellSize);
        int y = (int)(relativePosition.y / cellSize);

        return nodes[x, y];
    }



#if DEBUG
    [SerializeField] bool DrawGizmo;
    Color goalGizmoColor = new Color(1, 1, 0, 0.5f);
    void OnDrawGizmos()
    {
        if (DrawGizmo)
        {
            drawPlayerPositionNode();
            drawNodeOnGizmo(goalNode, goalGizmoColor);
            drawGridLine();
            drawObstacles();
            drawShortestPath();
            //drawFinalWaypoints();
            //drawFinalNodes();
            //drawClosedNodes();
        }


    }

    void drawShortestPath()
    {
        if (finalPath != null && finalPath.Count > 0)
        {
            drawLineBetweenNodes(transform.position, finalPath.First.Value);


            for (LinkedListNode<Vector2> iter = finalPath.First; iter.Next != null; iter = iter.Next)
            {
                drawLineBetweenNodes(iter.Value, iter.Next.Value);
            }
        }
    }

    void drawFinalNodes()
    {
        if (openList != null)
            foreach (Nodes n in nodeOnPathList)
            {
                drawNodeOnGizmo(n, Color.red);
            }
    }

    void drawFinalWaypoints()
    {
        if (nodeOnPathList != null && nodeOnPathList.Count > 1)
        {

            for (LinkedListNode<Nodes> iter = nodeOnPathList.First; iter.Next != null; iter = iter.Next)
            {
                drawLineBetweenNodes(iter.Value.nodeCenter, iter.Next.Value.nodeCenter);
            }
        }

    }

    void drawLineBetweenNodes(Vector2 start, Vector2 end)
    {
        Gizmos.color = Color.white;

        Gizmos.DrawLine(start, end);
    }

    Color playerGizmoColor = new Color(0, 1, 1, 0.5f);
    void drawPlayerPositionNode()
    {
        if (transform != null)
            drawNodeOnGizmo(findNodeOnPosition(transform.position), playerGizmoColor);
    }

    void drawGridLine()
    {
        Gizmos.color = Color.gray;

        Vector2 lineStartPoint = new Vector2();
        Vector2 lineEndPoint = new Vector2();

        //drawVerticalLines
        lineStartPoint.x = gridStartPoint.x;
        lineStartPoint.y = gridStartPoint.y;
        lineEndPoint.x = gridStartPoint.x;
        lineEndPoint.y = gridEndPoint.y; // line end point
        for (int i = 0; i <= numCols; i++)
        {
            Gizmos.DrawLine(lineStartPoint, lineEndPoint);
            lineStartPoint.x += cellSize;
            lineEndPoint.x += cellSize;
        }


        //drawHorizontalLines
        lineStartPoint.x = gridStartPoint.x;
        lineStartPoint.y = gridStartPoint.y;
        lineEndPoint.y = gridStartPoint.y;
        lineEndPoint.x = gridEndPoint.x; //line end point
        for (int i = 0; i <= numRows; i++)
        {
            Gizmos.DrawLine(lineStartPoint, lineEndPoint);
            lineStartPoint.y += cellSize;
            lineEndPoint.y += cellSize;
        }

    }

    void drawClosedNodes()
    {
        if (closeList != null)
            foreach (Nodes n in closeList)
            {
                if (n.parent != null)
                {
                    drawNodeOnGizmo(n, new Color(0, 0, 0, 0.5f));
                    drawLineBetweenNodes(n.nodeCenter, n.parent.nodeCenter);
                }
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
                    drawNodeOnGizmo(nodes[x, y], red);

            }
        }
    }

    void drawNodeOnGizmo(Nodes node, Color gizmoColor)
    {
        if (node == null) return;

        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(node.nodeCenter, new Vector2(cellSize, cellSize));
    }

}
#endif