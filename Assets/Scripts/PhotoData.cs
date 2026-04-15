using UnityEngine;

[System.Serializable]
public class PhotoData
{
    public int score;
    public float developTimer;
    public float maxDevelopTime;
    public bool isDeveloped;

    public PhotoData(int score, float developTime)
    {
        this.score = score;
        this.developTimer = developTime;
        this.maxDevelopTime = developTime;
        this.isDeveloped = false;
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