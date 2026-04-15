using TMPro;
using UnityEngine;

public class PhotoUI : MonoBehaviour
{
    [Header("References")]
    public PhotoSystem photoSystem;
    public PhotoBag photoBag;

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
            PhotoSystem.PhotoData currentPhoto = photoSystem.CurrentPhoto;

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

        PhotoSystem.PhotoData photo = photoBag.BagPhotos[index];

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
}