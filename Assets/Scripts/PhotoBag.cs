using System.Collections.Generic;
using UnityEngine;

public class PhotoBag : MonoBehaviour
{
    public int bagCapacity = 5;
    public int quota = 500;

    public List<PhotoSystem.PhotoData> bagPhotos = new List<PhotoSystem.PhotoData>();

    private int selectedSlot = -1;
    private int currentScore = 0;

    public int CurrentScore => currentScore;
    public int SelectedSlot => selectedSlot;
    public int Quota => quota;
    public int BagCapacity => bagCapacity;
    public List<PhotoSystem.PhotoData> BagPhotos => bagPhotos;

    void Update()
    {
        UpdateBagPhotos();
        HandleSlotSelection();
        HandleSlotActions();
        RecalculateScore();
    }

    public bool TryAddPhoto(PhotoSystem.PhotoData photo)
    {
        if (bagPhotos.Count >= bagCapacity) return false;

        bagPhotos.Add(photo);
        return true;
    }

    void UpdateBagPhotos()
    {
        for (int i = 0; i < bagPhotos.Count; i++)
        {
            bagPhotos[i].UpdateDevelopment(Time.deltaTime);
        }
    }

    void RecalculateScore()
    {
        currentScore = 0;

        for (int i = 0; i < bagPhotos.Count; i++)
        {
            if (bagPhotos[i].isDeveloped)
            {
                currentScore += bagPhotos[i].score;
            }
        }
    }

    void HandleSlotSelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedSlot = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) selectedSlot = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) selectedSlot = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4)) selectedSlot = 3;
        if (Input.GetKeyDown(KeyCode.Alpha5)) selectedSlot = 4;
    }

    void HandleSlotActions()
    {
        if (selectedSlot < 0 || selectedSlot >= bagPhotos.Count)
            return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            bagPhotos.RemoveAt(selectedSlot);
            selectedSlot = -1;
        }
    }
}