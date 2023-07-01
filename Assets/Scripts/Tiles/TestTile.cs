using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class TestTile : Tile
{
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        tileData.gameObject = obj;
    }

    public GameObject obj;


#if UNITY_EDITOR
    [MenuItem("Assets/TestTile")]
    public static void Create()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save test tile", "def test", "Asset",
            "Save test tile", "Assets");
        if (path == "") return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TestTile>(), path);
    }
#endif    
}
