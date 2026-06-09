using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotoUI : MonoBehaviour
{
    [Header("References")]
    public PhotoSystem photoSystem;
    public PhotoBag photoBag;

    public Image reticle;
    public Image flashImage;

    public GameObject photoCard;
    public Image photoCardImage;
    public TMP_Text photoCardText;

    public float flashFadeSpeed = 8f;
    private float flashAlpha = 0f;

    private float digitSpinTimer = 0f;
    private float digitSpinInterval = 0.03f;
    private int rouletteDisplayScore = 0;
    private bool revealStarted = false;

    [Header("Main UI")]
    public TMP_Text scoreText;
    public TMP_Text quotaText;
    public TMP_Text bagText;
    public TMP_Text currentPhotoText;
    public TMP_Text controlsText;

    [Header("Bag Slots")]
    public TMP_Text bagSlot1;
    public TMP_Text bagSlot2;
    public TMP_Text bagSlot3;
    public TMP_Text bagSlot4;
    public TMP_Text bagSlot5;

    void Update()
    {
        UpdateUI();
        UpdateFlash();
    }

    void UpdateUI()
    {
        if (photoSystem == null || photoBag == null) return;

        scoreText.text = "Score: " + photoBag.CurrentScore;
        quotaText.text = "Quota: " + photoBag.Quota;
        bagText.text = $"Bag: {photoBag.BagPhotos.Count}/{photoBag.BagCapacity}";

        controlsText.text =
            "LMB Take | RMB Zoom | E Shake | Q Bag | R Throw";

        UpdatePhotoStateUI();
        UpdateBagSlots();

        reticle.color = photoSystem.TargetInSight ? Color.green : Color.white;
        reticle.transform.localScale = photoSystem.TargetInSight ? Vector3.one * 1.2f : Vector3.one;
    }

    void UpdatePhotoStateUI()
    {
        PhotoData photo = photoSystem.CurrentPhoto;

        if (photo == null)
        {
            currentPhotoText.text = photoSystem.TargetInSight ? "Target in sight" : "No photo";
            photoCard.SetActive(false);

            revealStarted = false;
            rouletteDisplayScore = 0;
            return;
        }

        photoCard.SetActive(true);

        float percent = photo.GetDevelopPercent();
        photoCardImage.color = new Color(1, 1, 1, Mathf.Lerp(0.25f, 1f, percent));

        int finalScore = Mathf.RoundToInt(photo.score);

        if (photo.isDeveloped)
        {
            photoCardText.text = finalScore.ToString("D3");
            revealStarted = false;
            return;
        }

        // roulette start
        if (!revealStarted)
        {
            rouletteDisplayScore = Random.Range(0, 1000);
            revealStarted = true;
        }

        digitSpinTimer -= Time.deltaTime;

        if (digitSpinTimer <= 0f)
        {
            float p = percent;

            if (p < 0.4f)
                rouletteDisplayScore = Random.Range(0, 1000);
            else if (p < 0.7f)
                rouletteDisplayScore = finalScore / 100 * 100 + Random.Range(0, 100);
            else if (p < 0.9f)
                rouletteDisplayScore = finalScore / 10 * 10 + Random.Range(0, 10);
            else
                rouletteDisplayScore = finalScore;

            digitSpinInterval = Mathf.Lerp(0.02f, 0.2f, p);
            digitSpinTimer = digitSpinInterval;
        }

        photoCardText.text = rouletteDisplayScore.ToString("D3");
    }

    void UpdateBagSlots()
    {
        UpdateSlot(bagSlot1, 0);
        UpdateSlot(bagSlot2, 1);
        UpdateSlot(bagSlot3, 2);
        UpdateSlot(bagSlot4, 3);
        UpdateSlot(bagSlot5, 4);
    }

    void UpdateSlot(TMP_Text slot, int index)
    {
        if (slot == null) return;

        if (index >= photoBag.BagCapacity)
        {
            slot.text = "";
            return;
        }

        if (index >= photoBag.BagPhotos.Count)
        {
            slot.text = $"Slot {index + 1}: Empty";
            return;
        }

        PhotoData photo = photoBag.BagPhotos[index];

        if (photo.isDeveloped)
            slot.text = $"Slot {index + 1}: Score {photo.score}";
        else
            slot.text = $"Slot {index + 1}: Developing {photo.GetDevelopPercent() * 100f:0}%";
    }

    void UpdateFlash()
    {
        flashAlpha = Mathf.MoveTowards(flashAlpha, 0f, flashFadeSpeed * Time.deltaTime);

        if (flashImage != null)
        {
            Color c = flashImage.color;
            c.a = flashAlpha;
            flashImage.color = c;
        }
    }

    public void TriggerFlash(float alpha = 0.8f)
    {
        flashAlpha = alpha;
    }
}