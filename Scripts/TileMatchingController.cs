using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMatchingController : MonoBehaviour
{
    [Header("Tilemap & Tiles")]
    public Tilemap tilemapBlocks;
    public Tilemap tilemapHighlight;
    public TileBase highlightTile;

    public bool isPuzzleActive = false;
    public static event Action OnPuzzleCompleted;

    private Vector3Int? firstClickedPos = null;
    private Vector3Int? secondClickedPos = null;
    private bool isSwapping = false;

    void Start()
    {
        tilemapHighlight.ClearAllTiles();
        isPuzzleActive = false;
    }

    void Update()
    {
        if (!isPuzzleActive || isSwapping) return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
    }

    private void HandleMouseClick()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPos = tilemapBlocks.WorldToCell(mouseWorldPos);
        if (tilemapBlocks.HasTile(gridPos))
        {
            OnTileClicked(gridPos);
        }
    }

    public void ActivatePuzzle()
    {
        Debug.Log("Puzzle Diaktifkan!");
        isPuzzleActive = true;
    }

    public void DeactivatePuzzle()
    {
        Debug.Log("Puzzle Dinonaktifkan!");
        isPuzzleActive = false;
    }

    void OnTileClicked(Vector3Int clickedPos)
    {
        if (firstClickedPos == null)
        {
            firstClickedPos = clickedPos;
            ShowHighlight(clickedPos);
        }
        else if (secondClickedPos == null && clickedPos != firstClickedPos)
        {
            if (IsAdjacent(firstClickedPos.Value, clickedPos))
            {
                secondClickedPos = clickedPos;
                StartCoroutine(SwapAndCheckMatch(firstClickedPos.Value, secondClickedPos.Value));
            }
            else
            {
                ClearHighlight();
                firstClickedPos = clickedPos;
                ShowHighlight(clickedPos);
            }
        }
    }

    IEnumerator SwapAndCheckMatch(Vector3Int posA, Vector3Int posB)
    {
        isSwapping = true;
        ClearHighlight();

        TileBase tileA = tilemapBlocks.GetTile(posA);
        TileBase tileB = tilemapBlocks.GetTile(posB);

        tilemapBlocks.SetTile(posA, tileB);
        tilemapBlocks.SetTile(posB, tileA);

        yield return new WaitForSeconds(0.3f);

        HashSet<Vector3Int> matchedTiles = new HashSet<Vector3Int>();
        matchedTiles.UnionWith(FindMatchesAt(posA));
        matchedTiles.UnionWith(FindMatchesAt(posB));

        if (matchedTiles.Count > 0)
        {
            yield return StartCoroutine(ClearAndCascade(matchedTiles));
        }
        else
        {
            tilemapBlocks.SetTile(posA, tileA);
            tilemapBlocks.SetTile(posB, tileB);
        }

        isSwapping = false;
    }

    private IEnumerator ClearAndCascade(HashSet<Vector3Int> matchedTiles)
    {
        isSwapping = true;

        foreach (var pos in matchedTiles)
        {
            tilemapBlocks.SetTile(pos, null);
        }

        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(HandleGravity());

        HashSet<Vector3Int> newMatches = FindAllMatches();
        if (newMatches.Count > 0)
        {
            yield return StartCoroutine(ClearAndCascade(newMatches));
        }

        CheckForOverallCompletion();
        isSwapping = false;
    }

    private IEnumerator HandleGravity()
    {
        BoundsInt bounds = tilemapBlocks.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            List<TileBase> tilesInColumn = new List<TileBase>();
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (tilemapBlocks.HasTile(pos))
                {
                    tilesInColumn.Add(tilemapBlocks.GetTile(pos));
                }
            }

            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                tilemapBlocks.SetTile(new Vector3Int(x, y, 0), null);
            }

            for (int i = 0; i < tilesInColumn.Count; i++)
            {
                Vector3Int newPos = new Vector3Int(x, bounds.yMin + i, 0);
                tilemapBlocks.SetTile(newPos, tilesInColumn[i]);
            }
        }

        yield return new WaitForSeconds(0.3f);
    }

    private HashSet<Vector3Int> FindAllMatches()
    {
        HashSet<Vector3Int> allMatchedTiles = new HashSet<Vector3Int>();
        BoundsInt bounds = tilemapBlocks.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                allMatchedTiles.UnionWith(FindMatchesAt(new Vector3Int(x, y, 0)));
            }
        }
        return allMatchedTiles;
    }

    private HashSet<Vector3Int> FindMatchesAt(Vector3Int origin)
    {
        HashSet<Vector3Int> matched = new HashSet<Vector3Int>();
        TileBase targetTile = tilemapBlocks.GetTile(origin);
        if (targetTile == null) return matched;

        List<Vector3Int> horizontal = new List<Vector3Int> { origin };
        for (int dir = -1; dir <= 1; dir += 2)
        {
            Vector3Int pos = origin + new Vector3Int(dir, 0, 0);
            while (tilemapBlocks.GetTile(pos) == targetTile)
            {
                horizontal.Add(pos);
                pos += new Vector3Int(dir, 0, 0);
            }
        }
        if (horizontal.Count >= 3) matched.UnionWith(horizontal);

        List<Vector3Int> vertical = new List<Vector3Int> { origin };
        for (int dir = -1; dir <= 1; dir += 2)
        {
            Vector3Int pos = origin + new Vector3Int(0, dir, 0);
            while (tilemapBlocks.GetTile(pos) == targetTile)
            {
                vertical.Add(pos);
                pos += new Vector3Int(0, dir, 0);
            }
        }
        if (vertical.Count >= 3) matched.UnionWith(vertical);

        return matched;
    }

    private void CheckForOverallCompletion()
    {
        if (tilemapBlocks.GetUsedTilesCount() == 0)
        {
            Debug.Log("SELURUH PUZZLE SELESAI! Mengirim sinyal...");
            OnPuzzleCompleted?.Invoke();
            DeactivatePuzzle();
        }
    }

    bool IsAdjacent(Vector3Int a, Vector3Int b)
    {
        return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y)) == 1;
    }

    void ShowHighlight(Vector3Int pos)
    {
        tilemapHighlight.SetTile(pos, highlightTile);
    }

    void ClearHighlight()
    {
        tilemapHighlight.ClearAllTiles();
        firstClickedPos = null;
        secondClickedPos = null;
    }
}