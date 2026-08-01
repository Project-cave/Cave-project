using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Tilemap Reference")]
    [SerializeField] private Tilemap floorTilemap;
    
    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 1f;
    [SerializeField] private float minZoom = 2f;
    private float maxZoom;  // 맵 전체가 보이는 크기로 자동 계산
    
    [Header("Pan Settings (우클릭으로 이동)")]
    [SerializeField] private float panSensitivity = 1f;
    
    private Camera mainCamera;
    
    // 맵 경계 (월드 좌표)
    private float mapLeft, mapRight, mapBottom, mapTop;
    
    void Awake()
    {
        mainCamera = Camera.main;
    }
    
    void Start()
    {
        CalculateMapBounds();

        float mapWidth  = mapRight - mapLeft;
        float mapHeight = mapTop - mapBottom;
        float aspect    = (float)Screen.width / Screen.height;
        maxZoom = Mathf.Max(mapWidth / (2f * aspect), mapHeight / 2f);
    }
    
    // FloorTilemap의 범위로 맵 경계 계산
    void CalculateMapBounds()
{
    bool first = true;
    foreach (Vector3Int pos in floorTilemap.cellBounds.allPositionsWithin)
    {
        if (!floorTilemap.HasTile(pos)) continue;

        Vector3 world = floorTilemap.GetCellCenterWorld(pos);
        if (first)
        {
            mapLeft = mapRight = world.x;
            mapBottom = mapTop = world.y;
            first = false;
        }
        else
        {
            mapLeft   = Mathf.Min(mapLeft,   world.x);
            mapRight  = Mathf.Max(mapRight,  world.x);
            mapBottom = Mathf.Min(mapBottom, world.y);
            mapTop    = Mathf.Max(mapTop,    world.y);
        }
    }

    mapLeft   -= 0.5f;
    mapRight  += 0.5f;
    mapBottom -= 0.5f;
    mapTop    += 0.5f;
}
    
    // 카메라를 맵 중앙에 배치하고, 맵 전체가 보이도록 zoom 설정
    void FitCameraToMap()
    {
        float centerX = (mapLeft + mapRight) / 2f;
        float centerY = (mapBottom + mapTop) / 2f;
        mainCamera.transform.position = new Vector3(centerX, centerY, -10f);
        
        float mapWidth  = mapRight - mapLeft;
        float mapHeight = mapTop - mapBottom;
        float aspect    = (float)Screen.width / Screen.height;
        
        float zoomForWidth  = mapWidth / (2f * aspect);
        float zoomForHeight = mapHeight / 2f;
        
        maxZoom = Mathf.Max(zoomForWidth, zoomForHeight);
        mainCamera.orthographicSize = maxZoom;
    }
    
    void Update()
    {
        HandleZoom();
        HandlePan();
        ClampCamera();
    }
    
    // 스크롤로 확대/축소
    void HandleZoom()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll != 0f)
        {
            float newSize = mainCamera.orthographicSize - scroll * zoomSpeed;
            mainCamera.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
    }
    
    // 중간 마우스 버튼 드래그로 이동
    void HandlePan()
    {
        if (Mouse.current.rightButton.isPressed && Selector.instance.SelectedUnits.Count == 0)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            
            // 픽셀 단위 마우스 이동을 월드 좌표로 변환
            float worldHeight = mainCamera.orthographicSize * 2f;
            float worldWidth  = worldHeight * mainCamera.aspect;
            
            float moveX = -(delta.x / Screen.width)  * worldWidth  * panSensitivity;
            float moveY = -(delta.y / Screen.height) * worldHeight * panSensitivity;
            
            mainCamera.transform.position += new Vector3(moveX, moveY, 0f);
        }
    }
    
    // 카메라가 맵 밖을 보지 못하도록 제한
    void ClampCamera()
    {
        float halfHeight = mainCamera.orthographicSize;
        float halfWidth  = halfHeight * mainCamera.aspect;
        
        float minX = mapLeft  + halfWidth;
        float maxX = mapRight - halfWidth;
        float minY = mapBottom + halfHeight;
        float maxY = mapTop   - halfHeight;
        
        float x = mainCamera.transform.position.x;
        float y = mainCamera.transform.position.y;
        
        // 카메라 뷰가 맵보다 크면 중앙 고정, 작으면 범위 내로 제한
        x = (minX < maxX) ? Mathf.Clamp(x, minX, maxX) : (mapLeft + mapRight) / 2f;
        y = (minY < maxY) ? Mathf.Clamp(y, minY, maxY) : (mapBottom + mapTop) / 2f;
        
        mainCamera.transform.position = new Vector3(x, y, mainCamera.transform.position.z);
    }
}
