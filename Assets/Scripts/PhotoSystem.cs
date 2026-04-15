using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PhotoSystem : MonoBehaviour
{
    [System.Serializable]
    public class PhotoData
    {
        public int score;
        public float developTimer;
        public float maxDevelopTime;
        public bool isDeveloped;
        public bool scoreAdded;

        public PhotoData(int score, float developTime)
        {
            this.score = score;
            this.developTimer = developTime;
            this.maxDevelopTime = developTime;
            this.isDeveloped = false;
            this.scoreAdded = false;
        }

        public void UpdateDevelopment(float deltaTime)
        {
            if (isDeveloped) return;

            developTimer -= deltaTime;

            if (developTimer <= 0f)
            {
                developTimer = 0f;
                isDeveloped = true;
            }
        }

        public void Shake(float amount)
        {
            if (isDeveloped) return;

            developTimer -= amount;

            if (developTimer <= 0f)
            {
                developTimer = 0f;
                isDeveloped = true;
            }
        }

        public float GetDevelopPercent()
        {
            if (maxDevelopTime <= 0f) return 1f;
            return 1f - (developTimer / maxDevelopTime);
        }
    }

    [Header("Camera")]
    public Camera playerCamera;
    public float photoRange = 20f;

    [Header("Scoring")]
    public int currentScore = 0;
    public int quota = 500;
    public int normalScore = 100;
    public int zoomBonus = 100;

    [Header("Photo Development")]
    public float developTime = 2.5f;
    public float shakeReduceAmount = 0.12f;

    [Header("Bag")]
    public int bagCapacity = 5;
    public List<PhotoData> bagPhotos = new List<PhotoData>();

    [Header("Main UI")]
    public TMP_Text scoreText;
    public TMP_Text quotaText;
    public TMP_Text bagText;
    public TMP_Text currentPhotoText;
    public TMP_Text controlsText;

    [Header("Bag Slot UI")]
    public TMP_Text bagSlot1;
    public TMP_Text bagSlot2;
    public TMP_Text bagSlot3;
    public TMP_Text bagSlot4;
    public TMP_Text bagSlot5;

    private PhotoData currentPhoto = null;
    private bool targetInSight = false;

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        targetInSight = false;

        UpdateBagPhotos();

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

        UpdateUI();
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

    void UpdateBagPhotos()
    {
        for (int i = 0; i < bagPhotos.Count; i++)
        {
            PhotoData photo = bagPhotos[i];
            photo.UpdateDevelopment(Time.deltaTime);

            if (photo.isDeveloped && !photo.scoreAdded)
            {
                currentScore += photo.score;
                photo.scoreAdded = true;
            }
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
    }

    void PutPhotoInBag()
    {
        if (currentPhoto == null) return;
        if (bagPhotos.Count >= bagCapacity) return;

        bagPhotos.Add(currentPhoto);
        currentPhoto = null;
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

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + currentScore;

        if (quotaText != null)
            quotaText.text = "Quota: " + quota;

        if (bagText != null)
            bagText.text = "Bag: " + bagPhotos.Count + "/" + bagCapacity;

        if (controlsText != null)
            controlsText.text = "LMB Take | RMB Zoom | E Shake | Q Bag | R Throw";

        if (currentPhotoText != null)
        {
            if (currentPhoto == null)
            {
                currentPhotoText.text = targetInSight ? "Target in sight" : "No photo in hand";
            }
            else if (currentPhoto.isDeveloped)
            {
                currentPhotoText.text = "In Hand: Score " + currentPhoto.score + " | Q Bag | R Throw";
            }
            else
            {
                float percent = currentPhoto.GetDevelopPercent() * 100f;
                currentPhotoText.text = "In Hand: Developing " + percent.ToString("F0") + "% | E Shake | Q Bag | R Throw";
            }
        }

        UpdateBagSlotText(bagSlot1, 0);
        UpdateBagSlotText(bagSlot2, 1);
        UpdateBagSlotText(bagSlot3, 2);
        UpdateBagSlotText(bagSlot4, 3);
        UpdateBagSlotText(bagSlot5, 4);
    }

    void UpdateBagSlotText(TMP_Text slotText, int index)
    {
        if (slotText == null) return;

        if (index >= bagCapacity)
        {
            slotText.text = "";
            return;
        }

        if (index >= bagPhotos.Count)
        {
            slotText.text = "Slot " + (index + 1) + ": Empty";
            return;
        }

        PhotoData photo = bagPhotos[index];

        if (photo.isDeveloped)
        {
            slotText.text = "Slot " + (index + 1) + ": Score " + photo.score;
        }
        else
        {
            float percent = photo.GetDevelopPercent() * 100f;
            slotText.text = "Slot " + (index + 1) + ": Developing " + percent.ToString("F0") + "%";
        }
    }
}