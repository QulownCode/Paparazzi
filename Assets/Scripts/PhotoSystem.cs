using UnityEngine;

public class PhotoSystem : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public PhotoBag photoBag;
    public PhotoUI photoUI;

    [Header("Photo Settings")]
    public float photoRange = 20f;
    public float developTime = 6f;
    public float shakeReduceAmount = 0.4f;

    [Header("Scoring Weights")]
    public float distanceWeight = 40f;
    public float framingWeight = 40f;
    public float zoomWeight = 20f;

    public ScoreDebugUI debugUI;

    private PhotoData currentPhoto;
    private bool targetInSight;

    public PhotoData CurrentPhoto => currentPhoto;
    public bool TargetInSight => targetInSight;

    void Update()
    {
        targetInSight = false;

        if (currentPhoto != null)
        {
            HandleCurrentPhoto();
        }
        else
        {
            CheckTarget();

            if (Input.GetMouseButtonDown(0))
            {
                TakePhoto();
            }
        }
    }

    void HandleCurrentPhoto()
    {
        currentPhoto.UpdateDevelopment(Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.E))
            currentPhoto.Shake(shakeReduceAmount);

        if (Input.GetKeyDown(KeyCode.Q))
            PutPhotoInBag();

        if (Input.GetKeyDown(KeyCode.R))
            ThrowPhotoAway();
    }

    void TakePhoto()
    {
        if (currentPhoto != null) return;

        bool isZooming = Input.GetMouseButton(1);
        float range = isZooming ? photoRange * 1.5f : photoRange;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, range)) return;
        if (!hit.collider.CompareTag("Celebrity")) return;

        Transform target = hit.transform;

        // ---------------- DISTANCE ----------------
        float distance = Vector3.Distance(playerCamera.transform.position, target.position);
        float distanceScore = Mathf.Clamp01(1f - distance / photoRange) * distanceWeight;

        // ---------------- FRAMING ----------------
        Vector3 screenPos = playerCamera.WorldToViewportPoint(target.position);

        float centerDist = Vector2.Distance(
            new Vector2(screenPos.x, screenPos.y),
            new Vector2(0.5f, 0.5f)
        );

        float framingScore = Mathf.Clamp01(1f - centerDist) * framingWeight;

        // ---------------- ZOOM ----------------
        float zoomScore = isZooming ? zoomWeight : 0f;

        // ---------------- BASE ----------------
        float baseScore = PhotoScoreCalculator.CalculateBaseScore(
            distanceScore,
            framingScore,
            zoomScore
        );

        // ---------------- POSE (placeholder) ----------------
        float poseBonus = 0f;

        // ---------------- FINAL ----------------
        float finalScore = PhotoScoreCalculator.CalculateFinalScore(baseScore, poseBonus);

        bool overflow = PhotoScoreCalculator.CanOverflow(baseScore);

        currentPhoto = new PhotoData(finalScore, developTime);

        if (photoUI != null)
            photoUI.TriggerFlash();

        if (debugUI != null)
        {
            debugUI.UpdateDebug(
                distanceScore,
                framingScore,
                zoomScore,
                baseScore,
                finalScore,
                overflow
            );
        }
    }

    void PutPhotoInBag()
    {
        if (currentPhoto == null) return;
        if (photoBag == null) return;

        if (photoBag.TryAddPhoto(currentPhoto))
            currentPhoto = null;
    }

    void ThrowPhotoAway()
    {
        currentPhoto = null;
    }

    void CheckTarget()
    {
        bool isZooming = Input.GetMouseButton(1);
        float range = isZooming ? photoRange * 1.5f : photoRange;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
        {
            if (hit.collider.CompareTag("Celebrity"))
                targetInSight = true;
        }
    }
}