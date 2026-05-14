using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Script.Systems
{
    public class TilemapSampleSetup : MonoBehaviour
    {
        [Header("Tile Visual")]
        [SerializeField] private Sprite tileSprite;

        [Header("Ground Size")]
        [SerializeField] private int width = 40;
        [SerializeField] private int y = -2;

        [ContextMenu("Build Sample Ground")]
        [Obsolete("Obsolete")]
        public void BuildSampleGround()
        {
            Grid grid = FindObjectOfType<Grid>();
            if (grid == null)
            {
                GameObject gridObject = new GameObject("Grid");
                grid = gridObject.AddComponent<Grid>();
            }

            Transform groundTransform = grid.transform.Find("Ground");
            Tilemap groundTilemap;
            TilemapRenderer tilemapRenderer;
            TilemapCollider2D tilemapCollider;
            Rigidbody2D rb2D;

            if (groundTransform == null)
            {
                GameObject groundObject = new GameObject("Ground");
                groundObject.transform.SetParent(grid.transform);
                groundTilemap = groundObject.AddComponent<Tilemap>();
                tilemapRenderer = groundObject.AddComponent<TilemapRenderer>();
                tilemapCollider = groundObject.AddComponent<TilemapCollider2D>();
                rb2D = groundObject.AddComponent<Rigidbody2D>();
                groundObject.AddComponent<CompositeCollider2D>();
            }
            else
            {
                groundTilemap = groundTransform.GetComponent<Tilemap>() ?? groundTransform.gameObject.AddComponent<Tilemap>();
                tilemapRenderer = groundTransform.GetComponent<TilemapRenderer>() ?? groundTransform.gameObject.AddComponent<TilemapRenderer>();
                tilemapCollider = groundTransform.GetComponent<TilemapCollider2D>() ?? groundTransform.gameObject.AddComponent<TilemapCollider2D>();
                rb2D = groundTransform.GetComponent<Rigidbody2D>() ?? groundTransform.gameObject.AddComponent<Rigidbody2D>();
                _ = groundTransform.GetComponent<CompositeCollider2D>() ?? groundTransform.gameObject.AddComponent<CompositeCollider2D>();
            }

            rb2D.bodyType = RigidbodyType2D.Static;
            tilemapCollider.usedByComposite = true;
            tilemapRenderer.sortingOrder = -10;

            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = tileSprite;
            tile.colliderType = Tile.ColliderType.Grid;

            groundTilemap.ClearAllTiles();
            for (int x = -width / 2; x <= width / 2; x++)
            {
                groundTilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }
}
