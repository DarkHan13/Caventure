using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CreateRoom : MonoBehaviour
{

    [SerializeField] private Tilemap _tilemapGround;
    [SerializeField] private Tilemap _tilemapWalls;
    [SerializeField] private Tilemap _tilemapPlatforms;
    public Dictionary<MyVector3Int, int> Tiles;
    private BoundsInt bounds;


    /*[ContextMenu("Load Room")]
    void CreateAndLoad()
    {
        _tilemapGround.CompressBounds();
        BoundsInt bounds = _tilemapGround.cellBounds;
        GameObject world = GameObject.Find("World");
        TileMapData tileMapData = world.GetComponent<TileMapData>(); 
        Tiles = new Dictionary<MyVector3Int, int>(50);
        TileBase[] allTiles = _tilemapGround.GetTilesBlock(bounds);
        Debug.Log(bounds.ToString());
        for (int x = 0; x < bounds.size.x; x++) {
            for (int y = 0; y < bounds.size.y; y++) {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    int id = 0;
                    for (int i = 0; i < tileMapData.DefaultTiles.Length; i++)
                    {
                        if (tileMapData.DefaultTiles[i].Equals(tile))
                        {
                            id = i;
                            break;
                        }
                    }

                    MyVector3Int v = new MyVector3Int(x, y, 0);
                    Debug.Log(v.ToString());
                    Tiles.Add(v, id);
                }
            }
        }
        FileIOService.SaveData(Tiles,  Application.persistentDataPath + "/test.txt");
    }*/
    
    [ContextMenu("Load New Room")]
    void NewCreateAndLoad()
    {
        RoomData roomData = new RoomData();
        
        GameObject world = GameObject.Find("World");
        TileMapData tileMapData = world.GetComponent<TileMapData>();

        _tilemapGround.CompressBounds();
        bounds = _tilemapGround.cellBounds;
        
        roomData.GroundTiles = CreateDictionary(tileMapData, _tilemapGround);
        roomData.Walls = CreateDictionary(tileMapData, _tilemapWalls);
        roomData.Platforms = CreateDictionary(tileMapData, _tilemapPlatforms);
        
        
        FileIOService.NewSaveData(roomData,  Application.persistentDataPath + "/test.txt");
    }

    Dictionary<MyVector3Int, int> CreateDictionary(TileMapData tileMapData, Tilemap tilemap)
    {
        Dictionary<MyVector3Int, int> AllTiles = new Dictionary<MyVector3Int, int>(50);
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++) {
            for (int y = 0; y < bounds.size.y; y++) {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    int id = 0;
                    for (int i = 0; i < tileMapData.DefaultTiles.Length; i++)
                    {
                        if (tileMapData.DefaultTiles[i].Equals(tile))
                        {
                            id = i;
                            break;
                        }
                    }

                    MyVector3Int v = new MyVector3Int(x, y, 0);
                    AllTiles.Add(v, id);
                }
            }
        }

        return AllTiles;
    }
}
