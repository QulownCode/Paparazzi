using UnityEngine;

public class SimpleThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 4f;
    public float height = 2f;
    public float mouseSensitivity = 3f;
    public float smoothSpeed = 12f;
    public float minPitch = -20f;
    public float maxPitch = 60f;
    public float normalDistance = 3.5f;
    public float zoomDistance = 1.8f;

    public float normalSensitivity = 3f;
    public float zoomSensitivity = 1.5f;

    private float currentDistance;

    private float yaw;
    private float pitch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = 15f;
        currentDistance = normalDistance;
    }

    void LateUpdate()
    {
        if (target == null) return;

        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 targetPosition = target.position;
        Vector3 desiredPosition = targetPosition - rotation * Vector3.forward * currentDistance + Vector3.up * height + rotation * Vector3.right * 0.5f;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, smoothSpeed * Time.deltaTime);
        bool isZooming = Input.GetMouseButton(1);

        float targetDistance = isZooming ? zoomDistance : normalDistance;
        float targetSensitivity = isZooming ? zoomSensitivity : normalSensitivity;

        currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * 8f);
        mouseSensitivity = targetSensitivity;
    }
}