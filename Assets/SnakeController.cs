using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    GridSpawner grid;
    SnakeAgent2 agent;
    LineRenderer line;
    public Color onColor;
    public List<Vector2> BodyLocs = new List<Vector2>();
    public int SpawnPadding;

    public Vector2 Direction = Vector2.right;
    public float StepTime = 0.5f;

    public string DangerTiles;
    public Dictionary<Vector2, int> DangerTileLookup = new Dictionary<Vector2, int>();

    public Vector2 input = Vector3.right;
    Vector2 prevInput = Vector3.right;

    [Header("Display Tiles: L to R, U to D")]
    public List<SnakeTile> DangerDisplays = new List<SnakeTile>();
    public List<SnakeTile> DirectionDisplays = new List<SnakeTile>();

    public Transform goal;
    public bool moveGoal;
    public int generations;
    public bool PlayerControl;
    public bool RandomControl;
    public bool RobotControl;
    public bool AutoSpeed;
    public bool ASAP;
    public int startLength;

    public int manhattanDist, prevManhattanDist;
    void Start()
    {
        grid = FindObjectOfType<GridSpawner>();
        line = GetComponent<LineRenderer>();
        agent = GetComponent<SnakeAgent2>();
        BodyLocs.Clear();
        BodyLocs.Add(new Vector2(Mathf.Floor(Random.Range(10, grid.GridSize.x - 10)), Mathf.Floor(Random.Range(10, grid.GridSize.y - 10))));
        for (int l = 0; l < startLength; l++) 
        {
            BodyLocs.Add(BodyLocs[BodyLocs.Count - 1]);
        }

        print(grid.tiles.Count);
        grid.tiles[(int)BodyLocs[0].x][(int)BodyLocs[0].y].on = true;

        Invoke("Step", ASAP ? Time.fixedDeltaTime : StepTime);

        goal.transform.position = new Vector2(Mathf.Floor(Random.Range(0, grid.GridSize.x)), Mathf.Floor(Random.Range(0, grid.GridSize.y)));

        UpdateDangerTiles();

        // yikes
        DangerTileLookup.Add(new Vector2(1, 0), 4);
        DangerTileLookup.Add(new Vector2(-1, 0), 3);
        DangerTileLookup.Add(new Vector2(0, 1), 1);
        DangerTileLookup.Add(new Vector2(0, -1), 6);

        DangerTileLookup.Add(new Vector2(-1, 1), 0);
        DangerTileLookup.Add(new Vector2(1, 1), 2);
        DangerTileLookup.Add(new Vector2(-1, -1), 5);
        DangerTileLookup.Add(new Vector2(1, -1), 7);

        if (RandomControl)
            Invoke("RandomizeInput", StepTime);
    }

    void RandomizeInput() 
    {
        Vector2 tempInput = InputDecode(Random.Range(0, 4));

        if (tempInput != -input)
            input = tempInput;

        Invoke("RandomizeInput", StepTime * Random.Range(1, 4));
    }

    bool invalidMove = false;
    private void Update()
    {
        if (PlayerControl)
        {
            Vector2 tempInput = input;
            if (Input.GetKeyDown(KeyCode.A))
                tempInput = new Vector2(-1, 0);
            else if (Input.GetKeyDown(KeyCode.D))
                tempInput = new Vector2(1, 0);
            else if (Input.GetKeyDown(KeyCode.S))
                tempInput = new Vector2(0, -1);
            else if (Input.GetKeyDown(KeyCode.W))
                tempInput = new Vector2(0, 1);

            if (tempInput != -input)
                input = tempInput;
        }

        Debug.DrawLine(BodyLocs[0], BodyLocs[0] + input * 2, Color.green);
    }

    void Step()
    {
        int action = agent.RequestInput(DangerTiles, GetGoalDirection());
        if (RobotControl)
        {
            Vector2 tempInput = InputDecode(action);
            if (tempInput != -input)
                input = tempInput;
            //else
                //agent.Penalize(.01f);
        }

        if (BodyLocs[0] == (Vector2)goal.transform.position)
        {
            BodyLocs.Add(new Vector2(0, 0));
            if (moveGoal)
            {
                Vector2 newPos = new Vector2(Mathf.Floor(Random.Range(0, grid.GridSize.x)), Mathf.Floor(Random.Range(0, grid.GridSize.y)));
                while (BodyLocs.Contains(newPos))
                    newPos = new Vector2(Mathf.Floor(Random.Range(0, grid.GridSize.x)), Mathf.Floor(Random.Range(0, grid.GridSize.y)));

                goal.transform.position = newPos;
            }
            if (RobotControl)
            {
                agent.Reward(30);
                manhattanDist = 1000;
                prevManhattanDist = 1000;
            }
        }

        if (!DangerDisplays[DangerTileLookup[input]].on && !grid.tiles[(int)(BodyLocs[0].x + input.x)][(int)(BodyLocs[0].y + input.y)].on)
        {
            MoveSnake(input);

            Vector2 goalDiff = (Vector2)goal.position - BodyLocs[0];
            manhattanDist = (int)(Mathf.Abs(goalDiff.x) + Mathf.Abs(goalDiff.y));

            if (RobotControl && prevManhattanDist != manhattanDist)
            {
                if (manhattanDist < prevManhattanDist)
                {
                    agent.Reward(1);
                }
                else 
                {
                    agent.Penalize(-10);
                }

                prevManhattanDist = manhattanDist;
            }

            UpdateDangerTiles();
        }
        else    
        {
            for (int i = 0; i < BodyLocs.Count; i++)
                grid.tiles[(int)BodyLocs[i].x][(int)BodyLocs[i].y].on = false;

            input = RandomControl ? InputDecode(Random.Range(0, 5)) : Vector2.right;
            BodyLocs.Clear();
            BodyLocs.Add(new Vector2(Mathf.Floor(Random.Range(10, grid.GridSize.x - 10)), Mathf.Floor(Random.Range(10, grid.GridSize.y - 10))));
            for (int l = 0; l < startLength; l++)
            {
                BodyLocs.Add(BodyLocs[BodyLocs.Count - 1]);
            }

            //grid.tiles[(int)BodyLocs[0].x][(int)BodyLocs[0].y].on = true;
            if (moveGoal)
                goal.transform.position = new Vector2(Mathf.Floor(Random.Range(0, grid.GridSize.x)), Mathf.Floor(Random.Range(0, grid.GridSize.y)));

            if (RobotControl)
            {
                agent.Penalize(-1000);
            }

            generations += 1;

            UpdateDangerTiles();
        }

        if (RobotControl)
        {
            agent.UpdateQTable(DangerTiles, GetGoalDirection(), action);

            if (generations % 10 == 0 && agent.changeQTable)
                agent.SaveTable();
        }
        Invoke("Step", ASAP ? Time.fixedDeltaTime : AutoSpeed ? (0.5f * StepTime/(manhattanDist + 1)) + (0.5f * StepTime) : StepTime);
    }

    List<Vector2> tempLocs;
    void MoveSnake(Vector2 moveVector) 
    {
        grid.tiles[(int)BodyLocs[BodyLocs.Count - 1].x][(int)BodyLocs[BodyLocs.Count - 1].y].on = false;

        for (int i = BodyLocs.Count - 1; i > 0; i--)
        {
            BodyLocs[i] = BodyLocs[i - 1];
        }

        BodyLocs[0] += moveVector;
        line.positionCount = BodyLocs.Count;
        int k = 0;
        foreach (Vector2 pos in BodyLocs)
        {
            grid.tiles[(int)pos.x][(int)pos.y].onCol = new Color(onColor.r, onColor.g, onColor.b, 1);

            grid.tiles[(int)pos.x][(int)pos.y].on = true;
            line.SetPosition(k, pos);
            k += 1;
        }
    }
    void UpdateDangerTiles() 
    {
        // Cardinal Directions
        DangerDisplays[4].on = BodyLocs[0].x + 1 > grid.GridSize.x - 1 || grid.tiles[(int)BodyLocs[0].x + 1][(int)BodyLocs[0].y].on;

        DangerDisplays[3].on = BodyLocs[0].x - 1 < 0 || grid.tiles[(int)BodyLocs[0].x - 1][(int)BodyLocs[0].y].on;

        DangerDisplays[1].on = BodyLocs[0].y + 1 > grid.GridSize.y - 1 || grid.tiles[(int)BodyLocs[0].x][(int)BodyLocs[0].y + 1].on;

        DangerDisplays[6].on = BodyLocs[0].y - 1 < 0 || grid.tiles[(int)BodyLocs[0].x][(int)BodyLocs[0].y - 1].on;

        
        // Diagonals
        DangerDisplays[0].on = BodyLocs[0].x - 1 < 0 ||
                               BodyLocs[0].y + 1 > grid.GridSize.y - 1 || grid.tiles[(int)BodyLocs[0].x - 1][(int)BodyLocs[0].y + 1].on;

        DangerDisplays[2].on = BodyLocs[0].x + 1 > grid.GridSize.x - 1 ||
                               BodyLocs[0].y + 1 > grid.GridSize.y - 1 || grid.tiles[(int)BodyLocs[0].x + 1][(int)BodyLocs[0].y + 1].on;

        DangerDisplays[5].on = BodyLocs[0].x - 1 < 0 ||
                               BodyLocs[0].y - 1 < 0 || grid.tiles[(int)BodyLocs[0].x - 1][(int)BodyLocs[0].y - 1].on;

        DangerDisplays[7].on = BodyLocs[0].x + 1 > grid.GridSize.x - 1 ||
                               BodyLocs[0].y - 1 < 0 || grid.tiles[(int)BodyLocs[0].x + 1][(int)BodyLocs[0].y - 1].on;

        DangerTiles = "";
        for(int i = 0; i < DangerDisplays.Count; i++) 
        {
            DangerTiles += DangerDisplays[i].on ? "1" : "0";
        }
    }

    int InputEncode(Vector2 inp) 
    {
        int retval = -1;

        switch (inp)
        {
            case Vector2 v when v.Equals(Vector2.left):
                retval = 0;
                break;
            case Vector2 v when v.Equals(Vector2.up):
                retval = 1;
                break;
            case Vector2 v when v.Equals(Vector2.right):
                retval = 2;
                break;
            case Vector2 v when v.Equals(Vector2.down):
                retval = 3;
                break;
        }

        return retval;
    }

    Vector2 InputDecode(int id)
    {
        Vector2 retval = Vector2.right;

        switch (id)
        {
            case 0:
                retval = Vector2.left;
                break;
            case 1:
                retval = Vector2.up;
                break;
            case 2:
                retval = Vector2.right;
                break;
            case 3:
                retval = Vector3.down;
                break;
        }

        return retval;
    }

    Vector2 GetGoalDirection(bool display = true, bool cardinal = false) 
    {
        Vector2 rawDiff = (Vector2)goal.position - BodyLocs[0];
        Vector2 normalizedDiff = new Vector2(rawDiff.x == 0 ? 0 : Mathf.Sign(rawDiff.x),
                                             rawDiff.y == 0 ? 0 : Mathf.Sign(rawDiff.y));
        if (cardinal)
        {
            if (rawDiff.x > rawDiff.y)
                normalizedDiff.y = 0;
            else
                normalizedDiff.x = 0;
        }

        if (display)
        {
            foreach (SnakeTile t in DirectionDisplays)
            {
                t.on = false;
            }

            switch (normalizedDiff)
            {
                case Vector2 v when v.Equals(new Vector2(-1, 1)):
                    DirectionDisplays[0].on = true;
                    break;
                case Vector2 v when v.Equals(new Vector2(0, 1)):
                    DirectionDisplays[1].on = true;
                    break;
                case Vector2 v when v.Equals(new Vector2(1, 1)):
                    DirectionDisplays[2].on = true;
                    break;

                case Vector2 v when v.Equals(new Vector2(-1, 0)):
                    DirectionDisplays[3].on = true;
                    break;
                case Vector2 v when v.Equals(new Vector2(1, 0)):
                    DirectionDisplays[4].on = true;
                    break;

                case Vector2 v when v.Equals(new Vector2(-1, -1)):
                    DirectionDisplays[5].on = true;
                    break;
                case Vector2 v when v.Equals(new Vector2(0, -1)):
                    DirectionDisplays[6].on = true;
                    break;
                case Vector2 v when v.Equals(new Vector2(1, -1)):
                    DirectionDisplays[7].on = true;
                    break;
            }
        }
        return normalizedDiff;
    }

    List<Vector3> directions = new List<Vector3> { Vector3.up + Vector3.left, Vector3.up, Vector3.up + Vector3.right,
                                                   Vector3.left,                          Vector3.right,
                                                   Vector3.down + Vector3.left, Vector3.down, Vector3.down + Vector3.right };
    string GetGoalDirection() 
    {
        Vector2 rawDiff = (Vector2)goal.position - BodyLocs[0];
        Vector2 normalizedDiff = new Vector2(rawDiff.x == 0 ? 0 : Mathf.Sign(rawDiff.x),
                                             rawDiff.y == 0 ? 0 : Mathf.Sign(rawDiff.y));
        return directions.IndexOf(normalizedDiff).ToString();
    }
    public void TogglePlayerControl() 
    {
        PlayerControl = !PlayerControl;
        RobotControl = !RobotControl;
    }
}
