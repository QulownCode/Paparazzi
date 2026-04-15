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
    private float randomUpdateTimer = 0f;
    private int displayedScore = 0;
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

    [Header("Bag Slot UI")]
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

        if (scoreText != null)
            scoreText.text = "Score: " + photoBag.CurrentScore;

        if (quotaText != null)
            quotaText.text = "Quota: " + photoBag.Quota;

        if (bagText != null)
            bagText.text = "Bag: " + photoBag.BagPhotos.Count + "/" + photoBag.BagCapacity;

        if (controlsText != null)
            controlsText.text = "LMB Take | RMB Zoom | E Shake | Q Bag | R Throw | 1-5 Select | T Trash Bag Photo";

        if (currentPhotoText != null)
        {
            PhotoData currentPhoto = photoSystem.CurrentPhoto;

            if (currentPhoto == null)
            {
                currentPhotoText.text = photoSystem.TargetInSight ? "Target in sight" : "No photo in hand";
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
            UpdatePhotoCard();
        }

        UpdateBagSlotText(bagSlot1, 0);
        UpdateBagSlotText(bagSlot2, 1);
        UpdateBagSlotText(bagSlot3, 2);
        UpdateBagSlotText(bagSlot4, 3);
        UpdateBagSlotText(bagSlot5, 4);

        if (reticle != null)
        {
            reticle.color = photoSystem.TargetInSight ? Color.green : Color.white;
            reticle.transform.localScale = photoSystem.TargetInSight ? Vector3.one * 1.2f : Vector3.one;
        }
    }

    void UpdateBagSlotText(TMP_Text slotText, int index)
    {
        if (slotText == null) return;

        string prefix = (index == photoBag.SelectedSlot) ? "> " : "";

        if (index >= photoBag.BagCapacity)
        {
            slotText.text = "";
            return;
        }

        if (index >= photoBag.BagPhotos.Count)
        {
            slotText.text = prefix + "Slot " + (index + 1) + ": Empty";
            return;
        }

        PhotoData photo = photoBag.BagPhotos[index];

        if (photo.isDeveloped)
        {
            slotText.text = prefix + "Slot " + (index + 1) + ": Score " + photo.score;
        }
        else
        {
            float percent = photo.GetDevelopPercent() * 100f;
            slotText.text = prefix + "Slot " + (index + 1) + ": Developing " + percent.ToString("F0") + "%";
        }
    }
    void UpdateFlash()
    {
        if (flashImage == null) return;

        flashAlpha = Mathf.MoveTowards(flashAlpha, 0f, flashFadeSpeed * Time.deltaTime);

        Color color = flashImage.color;
        color.a = flashAlpha;
        flashImage.color = color;
    }
    public void TriggerFlash(float alpha = 0.8f)
    {
        flashAlpha = alpha;

        if (flashImage != null)
        {
            Color color = flashImage.color;
            color.a = flashAlpha;
            flashImage.color = color;
        }
    }
    void UpdatePhotoCard()
    {
        if (photoCard == null || photoCardImage == null || photoCardText == null || photoSystem == null)
            return;

        PhotoData currentPhoto = photoSystem.CurrentPhoto;

        if (currentPhoto == null)
        {
            photoCard.SetActive(false);
            randomUpdateTimer = 0f;
            displayedScore = 0;
            rouletteDisplayScore = 0;
            digitSpinTimer = 0f;
            digitSpinInterval = 0.03f;
            revealStarted = false;
            return;
        }

        photoCard.SetActive(true);

        Color cardColor = photoCardImage.color;
        float percent = currentPhoto.GetDevelopPercent();

        cardColor.a = Mathf.Lerp(0.25f, 1f, percent);
        photoCardImage.color = cardColor;

        int finalScore = Mathf.Clamp(currentPhoto.score, 0, 999);

        if (currentPhoto.isDeveloped)
        {
            revealStarted = false;
            rouletteDisplayScore = finalScore;
            photoCardText.text = rouletteDisplayScore.ToString("D3");
            return;
        }

        if (!revealStarted)
        {
            rouletteDisplayScore = Random.Range(0, 1000);
            digitSpinTimer = 0f;
            digitSpinInterval = 0.03f;
            revealStarted = true;
        }

        digitSpinTimer -= Time.deltaTime;

        if (digitSpinTimer <= 0f)
        {
            int finalHundreds = finalScore / 100;
            int finalTens = (finalScore / 10) % 10;
            int finalOnes = finalScore % 10;

            int shownHundreds = rouletteDisplayScore / 100;
            int shownTens = (rouletteDisplayScore / 10) % 10;
            int shownOnes = rouletteDisplayScore % 10;

            // Early: all digits spin
            // Mid: hundreds locks
            // Late: tens locks
            // End: ones locks
            if (percent < 0.4f)
            {
                shownHundreds = Random.Range(0, 10);
                shownTens = Random.Range(0, 10);
                shownOnes = Random.Range(0, 10);
            }
            else if (percent < 0.7f)
            {
                shownHundreds = finalHundreds;
                shownTens = Random.Range(0, 10);
                shownOnes = Random.Range(0, 10);
            }
            else if (percent < 0.9f)
            {
                shownHundreds = finalHundreds;
                shownTens = finalTens;
                shownOnes = Random.Range(0, 10);
            }
            else
            {
                shownHundreds = finalHundreds;
                shownTens = finalTens;
                shownOnes = finalOnes;
            }

            rouletteDisplayScore = shownHundreds * 100 + shownTens * 10 + shownOnes;

            // slow down over time
            digitSpinInterval = Mathf.Lerp(0.02f, 0.2f, percent);
            digitSpinTimer = digitSpinInterval;
        }

        photoCardText.text = rouletteDisplayScore.ToString("D3");
    }
}