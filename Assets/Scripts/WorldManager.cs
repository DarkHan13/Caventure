using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class WorldManager : MonoBehaviour
{
    [SerializeField] private int width = 80;
    [SerializeField] private int height = 80;
    [SerializeField] private int enemyCount;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap platformTilemap;
    [SerializeField] private Tilemap specialTilemap;
    [SerializeField] private BoundsInt area;
    [SerializeField] private int countOfRooms;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject[] _enemyPrefabs;
    private CameraMovement _cameraMovement;
    private TileMapData _tileMapData;
    private List<Room> _rooms;
    // Tiles Dictionary
    public Dictionary<Vector3Int, int> AllTiles;
    public Dictionary<Vector3Int, int> PlatformTiles;
    // Tiles for spawn gameobjects
    private HashSet<Vector3Int> _floor;
    public HashSet<Vector3Int> _empty;

    public GameObject player;
    public static WorldManager instance;

    private Vector3 offset = new Vector3(0.5f, 0.5f);

    void Start()
    {
        _cameraMovement = GameObject.Find("Main Camera").GetComponent<CameraMovement>();
        _tileMapData = GetComponent<TileMapData>();
        AllTiles = new Dictionary<Vector3Int, int>(50);
        PlatformTiles = new Dictionary<Vector3Int, int>(50);

        Generate();
    }

    private void Awake()
    {
        instance = this;
        Debug.Log(Application.persistentDataPath);
    }

    void Generate()
    {
        Debug.Log("Start Generate World...");
        AllTiles.Clear();
        PlatformTiles.Clear();
        groundTilemap.ClearAllTiles();
        platformTilemap.ClearAllTiles();
        specialTilemap.ClearAllTiles();
        
        
        SetTiles();
        foreach (var room in _rooms)
        {
            foreach (var (key, value) in room.RoomData.Walls)
            {
                
                if (AllTiles.ContainsKey(key.GetVector3Int())) continue;
                AllTiles.Add(key.GetVector3Int(), value);
            }  
        }
        
        // Set barrels, chest and floor tiles   
        SetObjects();
        
        // set all tiles in tilemap
        CommitTiles();
        
        // spawn player
        SpawnPlayer();
        
        //spawn enemies
        SpawnEnemies();

        var pathfinding = new Pathfinding(_empty, _floor, PlatformTiles);
    }

    void SetObjects()
    {
        Vector2Int startPos = new Vector2Int(0, 0);
        bool found = false;
        for (int x = _rooms[0].bounds.xMin; x < _rooms[0].bounds.xMax; x++)
        {
            for (int y = _rooms[0].bounds.yMin; y < _rooms[0].bounds.yMax; y++)
            {
                if (!_rooms[0].RoomData.GroundTiles.ContainsKey(new MyVector3Int(x, y, 0)))
                {
                    startPos = new Vector2Int(x, y);
                    found = true;
                    break;
                }
            }
            
            if (found) break;
        }
        try
        {
            if (found){
                SetFloorAndCeilTiles(startPos.x, startPos.y);
                HashSet<Vector3Int> usedTiles = new HashSet<Vector3Int>();
                int index = Random.Range(0, _floor.Count);
                Vector3Int pos = _floor.ElementAt(index);
                specialTilemap.SetTile(pos, _tileMapData.ObjectTiles[0]);
                usedTiles.Add(pos);
                // Set Door position
                for (int i = 0; i < 10; i++)
                {
                    pos = _floor.ElementAt(Random.Range(0, _floor.Count));
                    if (!usedTiles.Contains(pos))
                    {
                        specialTilemap.SetTile(pos, _tileMapData.ObjectTiles[2]);
                        usedTiles.Add(pos);
                        break;
                    }
                }
                for (int i = 0; i < 50; i++)
                {
                    pos = _floor.ElementAt(Random.Range(0, _floor.Count));
                    if (!usedTiles.Contains(pos)) specialTilemap.SetTile(pos, _tileMapData.ObjectTiles[1]);
                    else
                    {
                        Debug.Log("There is object");
                        i--;
                    }
                }
            
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error in random maybe");
            Debug.Log(e.Message);
        }
        
       
    }

    void SpawnPlayer()
    {
        try
        {
            if (player != null)
            {
                int index = Random.Range(0, _floor.Count);
                if (index >= _floor.Count) index = 0;
                Vector3Int pos = _floor.ElementAt(index);
                player.transform.position = new Vector3(pos.x + 0.5f, pos.y + 0.5f, pos.z);
            }
            else
            {
                Vector3Int pos = _floor.ElementAt(Random.Range(0, _floor.Count));
                player = Instantiate(_playerPrefab, new Vector3(pos.x + 0.5f, pos.y + 0.5f, pos.z), Quaternion.identity);

                _cameraMovement._player = player.GetComponent<PlayerController>();
                _cameraMovement.enabled = true;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error in random spawn player");
            Debug.Log(e.Message);
        }
        
    }

    void SpawnEnemies()
    {
        if (_enemyPrefabs.Length == 0) return;
        for (int i = 0; i < enemyCount; i++)
        {
            int rand = Random.Range(0, _enemyPrefabs.Length);
            var enemy = Instantiate(_enemyPrefabs[rand]);
            int index = Random.Range(0, _floor.Count);
            if (index >= _floor.Count) index = 0;
            Vector3Int pos = _floor.ElementAt(index);
            enemy.transform.position = pos + offset;
        }
    }
    
    void SetRooms(int count)
    {
        _rooms = new List<Room>();

        for (int i = 0; i < count; i++)
        {
            _rooms.Add(new Room());
        }

        int maxAttempt = 20;
        int current = maxAttempt;
        
        // Range
        int nRangeX = 0;
        int pRangeX = 0;
        int nRangeY = 0;
        int pRangeY = 0;
        
        for (int i = 0; i < count; i++)
        {
            // Limit
            int negativeLimitX = -width + 5;
            int positiveLimitX = width - _rooms[i].bounds.size.x - 5;
            int negativeLimitY = -height + 5;
            int positiveLimitY = height - _rooms[i].bounds.size.y - 5;
            
            // Set Limit
            pRangeX = pRangeX > positiveLimitX ? positiveLimitX : pRangeX;
            pRangeY = pRangeY > positiveLimitY ? positiveLimitY : pRangeY;
            nRangeX = nRangeX < negativeLimitX ? negativeLimitX : nRangeX;
            nRangeY = nRangeY < negativeLimitY ? negativeLimitY : nRangeY;
            
            
            int x = Random.Range(nRangeX, pRangeX);
            int y = Random.Range(nRangeY, pRangeY);

            bool inRoom = false;
            foreach (var room in _rooms)
            {
                if (room.IsCross(x, y, x + _rooms[i].bounds.size.x, y + _rooms[i].bounds.size.y) && i != 0)
                {
                    inRoom = true;
                    break;
                }
            }

            if (inRoom)
            {
                current--;
                if (current <= 0)
                {
                    current = maxAttempt;
                    Debug.Log("ERROR " + (i + 1));
                    _rooms.RemoveAt(i);
                    count--;
                }
                i--;
                continue;
            }
            current = maxAttempt;
            _rooms[i].ChangePosition(x, y);
            // Recalculate Range
            nRangeX = x - _rooms[i].bounds.size.x < nRangeX ? x - _rooms[i].bounds.size.x - 5 : nRangeX;
            nRangeY = y - _rooms[i].bounds.size.y < nRangeY ? x - _rooms[i].bounds.size.y - 5: nRangeY;
            pRangeX = x + _rooms[i].bounds.size.x > pRangeX ? x + _rooms[i].bounds.size.x + 5: pRangeX;
            pRangeY = x + _rooms[i].bounds.size.y > pRangeY ? y + _rooms[i].bounds.size.y + 5: pRangeY;
        }
    }
    void SetTiles()
    {
        AllTiles.Clear();
        PlatformTiles.Clear();
        groundTilemap.ClearAllTiles();
        platformTilemap.ClearAllTiles();
        SetRooms(countOfRooms);
        
        _rooms.Sort();
        
        _rooms[0].IsMainRoom = true;
        _rooms[0].IsAccessibleFromMainRoom = true;
        
        for (int i = 0; i < _rooms.Count; i++)
        {
            foreach (var (key, value) in _rooms[i].RoomData.GroundTiles)
            {
                if (AllTiles.ContainsKey(key.GetVector3Int())) continue;
                AllTiles.Add(key.GetVector3Int(), value);
            }
        }

        for (int i = 0; i < _rooms.Count; i++)
        {
            foreach (var (key, value) in _rooms[i].RoomData.Platforms)
            {
                if (PlatformTiles.ContainsKey(key.GetVector3Int())) continue;
                PlatformTiles.Add(key.GetVector3Int(), value);
            }
        }

        FillBlanks();
        ConnectClosestRooms();

        
    }

    void FillBlanks()
    {
        for (int x = -width; x < width; x++)
        {
            for (int y = -height; y < height; y++)
            {
                bool inRoom = false;
                Vector3Int pos = new Vector3Int(x, y, 0);
                foreach (Room room in _rooms)
                {
                    if (pos.x >= room.bounds.xMin && pos.x <= room.bounds.xMax 
                             && pos.y >= room.bounds.yMin && pos.y <= room.bounds.yMax)
                    {
                        inRoom = true;
                        break;
                    }
                }
                if (inRoom) continue;
                if (AllTiles.ContainsKey(pos)) AllTiles[pos] = 0;
                else AllTiles.Add(pos, 0);
            }
        }
    }
    
    void SetUnderGroundTiles()
    {
        
        List<Vector3Int> list = new List<Vector3Int>();
        foreach (var (pos, tileId) in AllTiles)
        {
            if (_empty.Contains(pos + Vector3Int.up) || 
                _empty.Contains(pos + Vector3Int.down) ||
                _empty.Contains(pos + Vector3Int.left) ||
                _empty.Contains(pos + Vector3Int.right)) continue;
            list.Add(pos);
        }
        foreach (var pos in list)
        {
            AllTiles[pos] = 4;
        }
    }
    void ConnectClosestRooms(bool forceAccessibilityFromMainRoom = false)
    {
        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();

        if (forceAccessibilityFromMainRoom)
        {
            foreach (Room room in _rooms)
            {
                if (room.IsAccessibleFromMainRoom)
                {
                    roomListB.Add(room);
                }
                else
                {
                    roomListA.Add(room);
                }
            }
        }
        else
        {
            roomListA = _rooms;
            roomListB = _rooms;
        }

        int bestDistance = 0;
        MyVector3Int bestTileA = new MyVector3Int();
        MyVector3Int bestTileB = new MyVector3Int();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool possibleConnectionFound = false;
        
        foreach (Room roomA in roomListA)
        {
            if (!forceAccessibilityFromMainRoom)
            {
                possibleConnectionFound = false;
                if (roomA.connectedRooms.Count > 0)
                    continue;
                
            }
            
            foreach (Room roomB in roomListB)
            {
                if (roomA == roomB || roomA.IsConnected(roomB))
                {
                    continue;
                }
                
                foreach (var (posA, valA) in roomA.RoomData.Walls)
                {
                    foreach (var (posB, valB) in roomB.RoomData.Walls)
                    {
                        int distance = (int)(Mathf.Pow(posB.x - posA.x, 2) + Mathf.Pow(posB.y - posA.y, 2));

                        if (distance < bestDistance || !possibleConnectionFound)
                        {
                            bestDistance = distance;
                            possibleConnectionFound = true;
                            bestTileA = posA;
                            bestTileB = posB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }
            if (possibleConnectionFound && !forceAccessibilityFromMainRoom)
            {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }
        
        if (possibleConnectionFound && forceAccessibilityFromMainRoom)
        {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(true);
        }

        if (!forceAccessibilityFromMainRoom)
        {
            ConnectClosestRooms(true);
        }
    }

    List<MyVector3Int> GetLine(MyVector3Int from, MyVector3Int to)
    {
        
        // calculate angle between horizontal and line
        float angle = Vector2.Angle(Vector2.right, new Vector2(to.x, to.y) - new Vector2(from.x, from.y) );
        
        bool isVertical = angle > 70 && angle < 110;
        
        List<MyVector3Int> line = new List<MyVector3Int>();
        int x = from.x;
        int y = from.y;

        int dx = to.x - x;
        int dy = to.y - y;

        Dictionary<MyVector3Int, int> platforms = new Dictionary<MyVector3Int, int>();

        bool inverted = false;
        int step = Math.Sign(dx);
        int gradientStep = Math.Sign(dy);

        int longest = Math.Abs(dx);
        int shortest = Math.Abs(dy);

        if (longest < shortest)
        {
            inverted = true;
            Debug.Log(longest + " " + shortest);
            longest = Math.Abs(dy);
            shortest = Math.Abs(dx);

            step = Math.Sign(dy);
            gradientStep = Math.Sign(dx);
        }

        int gradientAccumulation = longest / 2;
        for (int i = 0; i < longest; i++)
        {
            line.Add(new MyVector3Int(x, y, 0));
            if (inverted)
            {
                y += step;
            }
            else x += step;
            
            // spawn platforms every 3 tile when tunnel is vertical
            if (isVertical && i % 3 == 0)
            {
                platforms.Add(new MyVector3Int(x, y, 0), 3);
                platforms.Add(new MyVector3Int(x - 1, y, 0), 3);
                platforms.Add(new MyVector3Int(x + 1, y, 0), 3);
            }
            
            gradientAccumulation += shortest;
            if (gradientAccumulation >= longest)
            {
                if (inverted) x += gradientStep;
                else y += gradientStep;
                gradientAccumulation -= longest;
            }
        }

        if (inverted)
        {
            foreach (var (key, value) in platforms)
            {
                if (PlatformTiles.ContainsKey(key.GetVector3Int())) PlatformTiles[key.GetVector3Int()] = value;
                PlatformTiles.Add(key.GetVector3Int(), value);
            }
        }
        if (inverted)
            Debug.Log("inverted angle: " + angle);
        else 
            Debug.Log("just angle: " + angle);
        
        return line;
    }
    void CreatePassage(Room roomA, Room roomB, MyVector3Int tileA, MyVector3Int tileB)
    {
        Room.ConnectRooms(roomA, roomB);


        List<MyVector3Int> line = GetLine(tileA, tileB);
        
        foreach (MyVector3Int point in line)
        {
            DrawCirlce(point, 2);
        }
        
        RoomData.RemoveAdjacentTiles(roomA.RoomData.Walls, tileA);
        RoomData.RemoveAdjacentTiles(roomB.RoomData.Walls, tileB);
    }

    void DrawCirlce(MyVector3Int pos, int r)
    {
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    int drawX = pos.x + x;
                    int drawY = pos.y + y;
                    bool inRoom = false;
                    foreach (Room room in _rooms)
                    {
                        if (drawX > room.bounds.xMin && drawX < room.bounds.xMax 
                                                     && drawY > room.bounds.yMin && drawY < room.bounds.yMax)
                        {
                            inRoom = true;
                            break;
                        }
                    }
                    if (inRoom) continue;
                    Vector3Int coord = new Vector3Int(drawX, drawY, 0);
                    if (AllTiles.ContainsKey(coord)) AllTiles.Remove(coord);
                }
            }
        }
    }


    void SetFloorAndCeilTiles(int startX, int startY)
    {
        _floor = new HashSet<Vector3Int>();
        var _ceil = new HashSet<Vector3Int>();
        _empty = new HashSet<Vector3Int>();
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        queue.Enqueue(new Vector3Int(startX, startY, 0));
        int count = 0;
        int number = 0;
        while (queue.Count > 0 && count < 1000)
        {
            Vector3Int pos = queue.Dequeue();
            
            for (int x = pos.x - 1; x <= pos.x + 1; x++)
            {
                for (int y = pos.y - 1; y <= pos.y + 1; y++)
                {
                    if (y == pos.y || x == pos.x)
                    {
                        Vector3Int tmp = new Vector3Int(x, y, 0);
                        if (!AllTiles.ContainsKey(tmp)
                            && !_empty.Contains(tmp))
                        {
                            number++;
                            _empty.Add(tmp);
                            queue.Enqueue(tmp);
                        }
                    }
                }
            }
        }
        
        foreach (Vector3Int pos in _empty)
        {
            if (AllTiles.ContainsKey(pos + Vector3Int.down))
            {
                count++;
                _floor.Add(pos);
            } if (AllTiles.ContainsKey(pos + Vector3Int.up))
            {
                _ceil.Add(pos);
                AllTiles[pos + Vector3Int.up] = 7;
            }
        }
        
        foreach (var pos in _empty)
        {
            var left = pos + Vector3Int.left;
            var right = pos + Vector3Int.right;
            if (AllTiles.ContainsKey(left)
                && !_floor.Contains(left + Vector3Int.down) && !_ceil.Contains(left + Vector3Int.up))
            {
                AllTiles[left] = 5;
            } if (AllTiles.ContainsKey(right) 
                  && !_floor.Contains(right) && !_ceil.Contains(right))
            {
                AllTiles[right] = 6;
            } 
        }
        
        foreach (var pos in _floor)
        {
            bool hasLeft = _floor.Contains(pos + Vector3Int.left) || AllTiles.ContainsKey(pos + Vector3Int.left + Vector3Int.down);
            bool hasRight = _floor.Contains(pos + Vector3Int.right)  || AllTiles.ContainsKey(pos + Vector3Int.right + Vector3Int.down);
            if (!hasLeft && hasRight)
            {
                AllTiles[pos + Vector3Int.down] = 8;
            } else if (!hasRight && hasLeft)
            {
                AllTiles[pos + Vector3Int.down] = 9;
            }
        }
        
        foreach (var pos in _ceil)
        {
            bool hasLeft = _floor.Contains(pos + Vector3Int.left) || AllTiles.ContainsKey(pos + Vector3Int.left + Vector3Int.up);
            bool hasRight = _floor.Contains(pos + Vector3Int.right)  || AllTiles.ContainsKey(pos + Vector3Int.right + Vector3Int.up);
            if (!hasLeft && hasRight)
            {
                AllTiles[pos + Vector3Int.up] = 10;
            } else if (!hasRight && hasLeft)
            {
                AllTiles[pos + Vector3Int.up] = 11;
            }
        }
        
        SetUnderGroundTiles();

    }
    
    void CommitTiles()
    {
        foreach (KeyValuePair<Vector3Int, int> pair in AllTiles)
        {    
            groundTilemap.SetTile(pair.Key, _tileMapData.DefaultTiles[pair.Value]);
        }
        foreach (KeyValuePair<Vector3Int, int> pair in PlatformTiles)
        {    
            platformTilemap.SetTile(pair.Key, _tileMapData.DefaultTiles[pair.Value]);
        }
        

    }
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            /*Vector3 mouse = MousePosition.GetMousePosition();
            Vector3Int gridPos = groundTilemap.WorldToCell(mouse);
            groundTilemap.SetTile(new Vector3Int(gridPos.x, gridPos.y, 0), null);*/
        } 
        else if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouse = MousePosition.GetMousePosition();
            Vector3Int gridPos = groundTilemap.WorldToCell(mouse);
            groundTilemap.SetTile(new Vector3Int(gridPos.x, gridPos.y, 0), _tileMapData.DefaultTiles[1]);
            area = groundTilemap.cellBounds;
        }
        else if (Input.GetKeyDown(KeyCode.R)) Generate();
    }

    private void OnDrawGizmos()
    {
        /*if (Pathfinding.instance == null) return;

        var grid = Pathfinding.instance.GetGrid();
        Gizmos.color = Color.blue;
        for (int x = 0; x < grid.gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < grid.gridArray.GetLength(1); y++)
            {
                var cell = grid.GetValue(x, y);
                if (Pathfinding.instance.IsCellEmpty(cell))
                {
                    if (cell.height == 0) Gizmos.color = Color.blue;
                    else if (cell.height == 1) Gizmos.color = Color.green;
                    else if (cell.height == 2) Gizmos.color = Color.red;
                    else Gizmos.color = Color.yellow;
                    Gizmos.DrawWireCube(cell.GetPosition() + new Vector3(0.5f, 0.5f), Vector3.one);
                }
            }
        }*/
    }
}
