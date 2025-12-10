using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Camera Settings")]
    public float distance = 6f;
    public float smoothSpeed = 0.1f;
    public float mouseSensitivity = 3f;
    public float maxVerticalAngle = 80f;

    [Header("Height Settings")]
    public float normalHeight = 2f;
    public float lookUpHeight = 8f;
    public float lookDownHeight = 0.5f;

    private float mouseX;
    private float mouseY;
    private Vector3 currentPosition;
    private float currentHeight;

    void Start()
    {
        InitializeCamera();
    }

    public void InitializeCamera()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
            else
            {
                Invoke(nameof(InitializeCamera), 0.5f);
                return;
            }
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        mouseX = player.eulerAngles.y;
        mouseY = 0;
        currentHeight = normalHeight;
        currentPosition = GetCameraPosition(mouseX, mouseY, currentHeight);
        transform.position = currentPosition;
        transform.LookAt(player.position + Vector3.up * 1f);
    }

    void LateUpdate()
    {
        if (player == null) return;

        mouseX += Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        mouseY = Mathf.Clamp(mouseY, -maxVerticalAngle, maxVerticalAngle);

        float targetHeight = mouseY >= 0
            ? Mathf.Lerp(normalHeight, lookUpHeight, mouseY / maxVerticalAngle)
            : Mathf.Lerp(normalHeight, lookDownHeight, -mouseY / maxVerticalAngle);

        currentHeight = Mathf.Lerp(currentHeight, targetHeight, smoothSpeed * 2f);
        Vector3 targetPos = GetCameraPosition(mouseX, mouseY, currentHeight);
        targetPos.y = Mathf.Max(targetPos.y, player.position.y + 0.3f);

        currentPosition = Vector3.Lerp(currentPosition, targetPos, smoothSpeed);
        transform.position = currentPosition;
        transform.LookAt(player.position + Vector3.up * 1f);
    }

    Vector3 GetCameraPosition(float rotationX, float rotationY, float height)
    {
        Vector3 dir = new Vector3(0, 0, -distance);
        dir = Quaternion.Euler(rotationY, rotationX, 0) * dir;
        return player.position + dir + Vector3.up * height;
    }

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
        SnapBehindPlayer();
    }

    public void SnapBehindPlayer()
    {
        if (player == null) return;
        mouseX = player.eulerAngles.y;
        mouseY = 0;
        currentHeight = normalHeight;
        currentPosition = GetCameraPosition(mouseX, mouseY, currentHeight);
        transform.position = currentPosition;
        transform.LookAt(player.position + Vector3.up * 1f);
    }
}