using UnityEngine;

public class CaveMeshGenerator : MonoBehaviour
{

    //stopped at https://www.youtube.com/watch?v=2gIxh8CX3Hk&list=PLFt_AvWsXl0eZgMK_DT5_biRkWXftAOf9&index=3
    public SquareGrid squareGrid;

    public void GenerateMesh(int[,] map, float squareSize)
    {
        squareGrid = new SquareGrid(map, squareSize);
    }

    public class SquareGrid {
        public Square[,] squares;

        public SquareGrid(int[,] map, float squareSize)
        { 
            int nodeCountX = map.GetLength(0);
            int nodeCountZ = map.GetLength(1);
            float mapWidth = nodeCountX * squareSize;
            float mapLength = nodeCountZ * squareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountZ];

            for (int x = 0; x < nodeCountX; x++)
            {
                for (int z = 0; z < nodeCountZ; z++)
                {
                    Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, 0, -mapLength / 2 + z * squareSize + squareSize / 2);
                    controlNodes[x, z] = new ControlNode(pos, map[x, z] == 1, squareSize);
                }
            }

            squares = new Square[nodeCountX - 1, nodeCountZ - 1];

            for (int x = 0; x < nodeCountX-1; x++)
            {
                for (int z = 0; z < nodeCountZ-1; z++)
                {
                    squares[x, z] = new Square(controlNodes[x, z + 1], controlNodes[x + 1, z + 1], controlNodes[x + 1, z], controlNodes[x, z]);
                }
            }
        }
    }

    public class Square {

        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node centreTop, centreRight, centreBottom, centreLeft;

        public Square(ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft)
        { topLeft = _topLeft; topRight = _topRight; bottomLeft = _bottomLeft; bottomRight = _bottomRight;
            centreTop = topLeft.right; centreLeft = bottomLeft.above; centreRight = bottomRight.above; centreBottom = bottomLeft.right;
        }
    }

    public class Node {
        public Vector3 position;
        public int vertexIndex = -1;

        public Node(Vector3 pos)
        {
            position = pos;
        }
    }

    public class ControlNode : Node {

        public bool active;
        public Node above, right;

        public ControlNode(Vector3 pos, bool _active, float squareSize) : base(pos) {
            active = _active;
            above = new Node(position + Vector3.forward * squareSize/2f);
            right = new Node(position + Vector3.right * squareSize / 2f);
        }
    }

    void OnDrawGizmos()
    {
        if (squareGrid != null)
            for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
            {
                for (int z = 0; z < squareGrid.squares.GetLength(1); z++)
                {
                    Gizmos.color = (squareGrid.squares[x, z].topLeft.active) ? Color.green : Color.red;
                    Gizmos.DrawCube(squareGrid.squares[x, z].topLeft.position, Vector3.one * .4f);

                    Gizmos.color = (squareGrid.squares[x, z].topRight.active) ? Color.green : Color.red;
                    Gizmos.DrawCube(squareGrid.squares[x, z].topRight.position, Vector3.one * .4f);

                    Gizmos.color = (squareGrid.squares[x, z].bottomLeft.active) ? Color.green : Color.red;
                    Gizmos.DrawCube(squareGrid.squares[x, z].bottomLeft.position, Vector3.one * .4f);

                    Gizmos.color = (squareGrid.squares[x, z].bottomRight.active) ? Color.green : Color.red;
                    Gizmos.DrawCube(squareGrid.squares[x, z].bottomRight.position, Vector3.one * .4f);

                    Gizmos.color = Color.grey;
                    Gizmos.DrawCube(squareGrid.squares[x, z].centreTop.position, Vector3.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[x, z].centreBottom.position, Vector3.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[x, z].centreRight.position, Vector3.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[x, z].centreLeft.position, Vector3.one * .15f);
                }
            }
    }
}
