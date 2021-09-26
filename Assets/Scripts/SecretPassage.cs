using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SecretPassage : MonoBehaviour
{
    private TilemapCollider2D tilemapCollider;
    private Tilemap tilemap;

    private float lerpDuration = 1.5f;
    private float endAlpha = 0.2f;

    private void Start()
    {
        tilemapCollider = GetComponent<TilemapCollider2D>();
        tilemap = GetComponent<Tilemap>();

        foreach (Vector3Int tilePosition in tilemap.cellBounds.allPositionsWithin)
        {
            tilemap.RemoveTileFlags(tilePosition, TileFlags.LockColor);
        }
    }
    private void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.name == "Player")
        {
            Vector3 centre = coll.bounds.center;
            Vector3 min = coll.bounds.min;
            Vector3 max = coll.bounds.max;

            Vector3[] corners =
            {
                new Vector3(min.x, min.y, 0f), new Vector3(min.x, max.y, 0f), new Vector3(min.x, centre.y, 0f),
                new Vector3(max.x, min.y, 0f), new Vector3(max.x, max.y, 0f), new Vector3(max.x, centre.y, 0f),
            };

            foreach (Vector3 corner in corners)
            {
                Vector3Int tilePosition = tilemap.WorldToCell(corner);
                if (tilemap.HasTile(tilePosition))
                {
                    StartCoroutine(LerpColour(tilePosition));
                }
            }
        }
    }
    private IEnumerator LerpColour(Vector3Int tilePosition)
    {
        Color lerpedColour = tilemap.GetColor(tilePosition);
        float timeElapsed = 0;

        while (timeElapsed < lerpDuration)
        {
            lerpedColour = new Color(lerpedColour.r, lerpedColour.g, lerpedColour.b, Mathf.Lerp(1, endAlpha, timeElapsed / lerpDuration));
            tilemap.SetColor(tilePosition, lerpedColour);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        lerpedColour = new Color(lerpedColour.r, lerpedColour.g, lerpedColour.b, endAlpha);
        tilemap.SetColor(tilePosition, lerpedColour);
        tilemap.SetTileFlags(tilePosition, TileFlags.LockColor);
    }
}
