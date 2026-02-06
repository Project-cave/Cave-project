using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Selector : MonoBehaviour
{
    public RectTransform selectionBox;

    private Vector2 startPos;
    private Vector2 endPos;
    private bool isDragging = false;

    private List<GameObject> selectedUnits = new List<GameObject>();

    public LayerMask selectableLayer;

    public Color selectedColor = Color.green;
    private Dictionary<GameObject,Color> originalColors = new Dictionary<GameObject,Color>();

    // Start is called before the first frame update
    void Start()
    {
        if(selectionBox != null)
            selectionBox.gameObject.SetActive(false);
    }

    // Update is called once per frame
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
    }

    void StartDrag()
    {
        isDragging = true;
        startPos = Input.mousePosition;

        if(selectionBox != null)
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
        endPos = Input.mousePosition;

        if(selectionBox != null)
        {
            Vector2 boxStart = startPos;
            Vector2 boxEnd = endPos;

            Vector2 boxCentor = (boxStart + boxEnd) / 2;
            selectionBox.anchoredPosition = boxCentor;

            Vector2 boxSize = new Vector2(
                Mathf.Abs(boxStart.x - boxEnd.x),
                Mathf.Abs(boxStart.y - boxEnd.y)
            );
            selectionBox.sizeDelta = boxSize;
        }
    }

    void EndDrag()
    {
        isDragging=false;

        if(selectionBox != null )
            selectionBox.gameObject.SetActive(false);

        Rect selectionRect = GetScreenRect(startPos, endPos);

        GameObject[] selectables = GameObject.FindGameObjectsWithTag("selectable");
        
        foreach(GameObject obj in selectables)
        {
            
            Collider2D collider = obj.GetComponent<Collider2D>();
            
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

        return Rect.MinMaxRect(bottomLeft.x, bottomLeft.y, topRight.x, topRight.y);
    }

    void SelectUnit(GameObject unit)
    {
        if (!selectedUnits.Contains(unit))
        {
            selectedUnits.Add(unit);

            // 시각적 피드백 (색상 변경)
            SpriteRenderer renderer = unit.GetComponent<SpriteRenderer>();
            PlayerMovement movement = unit.GetComponent<PlayerMovement>();
            movement.moveable = true;
            if (renderer != null)
            {
                renderer.color = Color.blue;
                Debug.Log("blue");
                //if (!originalColors.ContainsKey(unit))
                //{
                //    originalColors[unit] = renderer.material.color;
                //}
                //renderer.material.color = selectedColor;
            }

        }
    }

    void DeselectAll()
    {
        foreach (GameObject unit in selectedUnits)
        {
            if (unit != null)
            {
                // 원래 색상으로 복원
                SpriteRenderer renderer = unit.GetComponent<SpriteRenderer>();
                PlayerMovement movement = unit.GetComponent<PlayerMovement>();
                movement.moveable = false;
                renderer.color = Color.white;
                //if (renderer != null && originalColors.ContainsKey(unit))
                //{
                //    renderer.material.color = originalColors[unit];
                //}

               
            }
        }

        selectedUnits.Clear();
        originalColors.Clear();
    }

    Rect GetScreenRect(Vector2 start, Vector2 end)
    {
        // 시작점과 끝점으로부터 Rect 생성
        //start.y = Screen.height - start.y;
        //end.y = Screen.height - end.y;

        Vector2 topLeft = Vector2.Min(start, end);
        Vector2 bottomRight = Vector2.Max(start, end);
        Debug.Log(topLeft);
        Debug.Log(bottomRight);

        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    //선택된 유닛들에게 명령을 내리는 예제 함수
    public void MoveSelectedUnits(Vector3 destination)
    {
        foreach (GameObject unit in selectedUnits)
        {
            // 각 유닛의 이동 스크립트 호출
            PlayerMovement movement = unit.GetComponent<PlayerMovement>();
            movement.moveable = true;
            
        }
    }

    // 현재 선택된 유닛 목록 가져오기
    public List<GameObject> GetSelectedUnits()
    {
        return new List<GameObject>(selectedUnits);
    }
}
