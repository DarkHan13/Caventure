using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TileMapShadowCast : MonoBehaviour
{
    [ContextMenu("Generate")]
    public void Generate() {
        var tilemapCollider = GetComponent<CompositeCollider2D>();
        GameObject shadowCasterContainer = GameObject.Find("shadow_casters");
        for (int i = 0; i < tilemapCollider.pathCount; i++) {
            Vector2[] pathVertices = new Vector2[tilemapCollider.GetPathPointCount(i)];
            tilemapCollider.GetPath(i, pathVertices);
            GameObject shadowCaster = new GameObject("shadow_caster_" + i);
            PolygonCollider2D shadowPolygon = (PolygonCollider2D)shadowCaster.AddComponent(typeof(PolygonCollider2D));
            shadowCaster.transform.parent = shadowCasterContainer.transform;
            shadowPolygon.points = pathVertices;
            shadowPolygon.enabled = false;
            ShadowCaster2D shadowCasterComponent = shadowCaster.AddComponent<ShadowCaster2D>();
            shadowCasterComponent.selfShadows = false;
        }
    }
}
