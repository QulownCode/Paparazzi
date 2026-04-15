using UnityEngine;

public class PhotoSystem : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public PhotoBag photoBag;
    public PhotoUI photoUI;

    [Header("Photo Settings")]
    public float photoRange = 20f;
    public float developTime = 2.5f;
    public float shakeReduceAmount = 0.12f;

    [Header("Scoring")]
    public int normalScore = 100;
    public int zoomBonus = 100;

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
        {
            currentPhoto.Shake(shakeReduceAmount);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            PutPhotoInBag();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ThrowPhotoAway();
        }
    }

    void TakePhoto()
    {
        if (currentPhoto != null) return;

        bool isZooming = Input.GetMouseButton(1);
        float range = isZooming ? photoRange * 1.5f : photoRange;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        int photoScore = 0;

        if (Physics.Raycast(ray, out hit, range))
        {
            if (hit.collider.CompareTag("Celebrity"))
            {
                photoScore = normalScore;

                if (isZooming)
                {
                    photoScore += zoomBonus;
                }
            }
        }

        currentPhoto = new PhotoData(photoScore, developTime);

        if (photoUI != null)
        {
            photoUI.TriggerFlash();
        }
    }

    void PutPhotoInBag()
    {
        if (currentPhoto == null) return;
        if (photoBag == null) return;

        bool added = photoBag.TryAddPhoto(currentPhoto);

        if (added)
        {
            currentPhoto = null;
        }
    }

    void ThrowPhotoAway()
    {
        if (currentPhoto == null) return;
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
            {
                targetInSight = true;
            }
        }
    }
}