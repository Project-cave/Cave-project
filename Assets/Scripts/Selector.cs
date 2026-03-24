using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    public static Selector instance;
    public RectTransform selectionBox;
    public Canvas canvas;
    [SerializeField] float formationSpacing = 0.5f;

    private Vector2 startPos;
    private Vector2 endPos;
    private bool isDragging = false;

    public List<GameObject> selectedUnits = new List<GameObject>();
    public List<GameObject> SelectedUnits
    {
        get => selectedUnits;
    }

    public LayerMask selectableLayer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (selectionBox != null)
            selectionBox.gameObject.SetActive(false);

        if (canvas == null)
        {
            canvas = selectionBox.GetComponentInParent<Canvas>();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDrag();
        }

        if (isDragging)
        {
            UpdateDrag();
        }

        if (Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }

        if (Input.GetMouseButtonDown(1) && selectedUnits.Count > 0)
        {
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0;
            MoveSelectedUnitsWithFormation(targetPosition);
        }
    }

    public void MoveSelectedUnitsWithFormation(Vector3 centerPosition)
    {

        if (selectedUnits.Count == 0)
        {
            Debug.LogWarning("[Selector] 선택된 유닛이 없습니다!");
            return;
        }

        if (selectedUnits.Count == 1)
        {
            PlayerMovement movement = selectedUnits[0].GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.MoveToPosition(centerPosition);
            }
            else
            {
                Debug.LogError($"[Selector] {selectedUnits[0].name}에 PlayerMovement가 없습니다!");
            }
            return;
        }

        int unitsPerRow = Mathf.CeilToInt(Mathf.Sqrt(selectedUnits.Count));
        int currentRow = 0;
        int currentCol = 0;
        int movedCount = 0;

        foreach (GameObject unit in selectedUnits)
        {
            if (unit == null)
            {
                Debug.LogWarning("[Selector] null 유닛 발견!");
                continue;
            }

            PlayerMovement movement = unit.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                float offsetX = (currentCol - unitsPerRow / 2f) * formationSpacing;
                float offsetY = (currentRow - unitsPerRow / 2f) * formationSpacing;

                Vector3 targetPos = new Vector3(
                    centerPosition.x + offsetX,
                    centerPosition.y + offsetY,
                    0
                );
                movement.MoveToPosition(targetPos);
                movedCount++;

                currentCol++;
                if (currentCol >= unitsPerRow)
                {
                    currentCol = 0;
                    currentRow++;
                }
            }
            else
            {
                Debug.LogError($"[Selector] {unit.name}에 PlayerMovement가 없습니다!");
            }
        }
    }

    void StartDrag()
    {
        isDragging = true;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out startPos
        );

        if (selectionBox != null)
        {
            selectionBox.gameObject.SetActive(true);
            selectionBox.anchoredPosition = startPos;
            selectionBox.sizeDelta = Vector2.zero;
        }

        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
        {
            DeselectAll();
        }
    }

    void UpdateDrag()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out endPos
        );

        if (selectionBox != null)
        {
            Vector2 boxStart = startPos;
            Vector2 boxEnd = endPos;

            Vector2 boxCenter = (boxStart + boxEnd) / 2;
            selectionBox.anchoredPosition = boxCenter;

            Vector2 boxSize = new Vector2(
                Mathf.Abs(boxStart.x - boxEnd.x),
                Mathf.Abs(boxStart.y - boxEnd.y)
            );
            selectionBox.sizeDelta = boxSize;
        }
    }

    void EndDrag()
    {
        isDragging = false;

        if (selectionBox != null)
            selectionBox.gameObject.SetActive(false);

        Rect selectionRect = GetScreenRect(startPos, endPos);

        GameObject[] selectables = GameObject.FindGameObjectsWithTag("selectable");

        foreach (GameObject obj in selectables)
        {
            Collider2D collider = obj.GetComponent<Collider2D>();
            if (collider == null) continue;

            Rect colliderScreenRect = GetColliderScreenRect(collider);

            if (selectionRect.Overlaps(colliderScreenRect))
            {
                SelectUnit(obj);
            }
        }
    }

    Rect GetColliderScreenRect(Collider2D collider)
    {
        Bounds bounds = collider.bounds;

        Vector3 bottomLeft = Camera.main.WorldToScreenPoint(bounds.min);
        Vector3 topRight = Camera.main.WorldToScreenPoint(bounds.max);

        Vector2 canvasBottomLeft, canvasTopRight;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            bottomLeft,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out canvasBottomLeft
        );

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            topRight,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out canvasTopRight
        );

        return Rect.MinMaxRect(canvasBottomLeft.x, canvasBottomLeft.y, canvasTopRight.x, canvasTopRight.y);
    }

    void SelectUnit(GameObject unit)
    {
        if (!selectedUnits.Contains(unit))
        {
            selectedUnits.Add(unit);

            SpriteRenderer renderer = unit.GetComponent<SpriteRenderer>();
            PlayerMovement movement = unit.GetComponent<PlayerMovement>();

            if (movement != null)
            {
                movement.moveable = true;
                Debug.Log($"[Selector] {unit.name} 선택됨 (moveable = true)");
            }
            if (renderer != null)
            {
                renderer.color = Color.blue;
            }
        }
    }

    public void DeselectAll()
    {
        foreach (GameObject unit in selectedUnits)
        {
            if (unit != null)
            {
                SpriteRenderer renderer = unit.GetComponent<SpriteRenderer>();
                PlayerMovement movement = unit.GetComponent<PlayerMovement>();

                if (movement != null)
                    movement.moveable = false;
                if (renderer != null)
                    renderer.color = Color.white;
            }
        }

        selectedUnits.Clear();
    }

    Rect GetScreenRect(Vector2 start, Vector2 end)
    {
        Vector2 topLeft = Vector2.Min(start, end);
        Vector2 bottomRight = Vector2.Max(start, end);

        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    public List<GameObject> GetSelectedUnits()
    {
        return new List<GameObject>(selectedUnits);
    }
}