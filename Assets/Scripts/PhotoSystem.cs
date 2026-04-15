using UnityEngine;

public class PhotoSystem : MonoBehaviour
{
    public Camera playerCamera;
    public float photoRange = 20f;

    public int currentScore = 0;
    public int quota = 500;

    public int normalScore = 100;
    public int zoomBonus = 100;

    public float developTime = 4f;
    public float shakeReduceAmount = 0.35f;

    private bool isDeveloping = false;
    private float developTimer = 0f;

    void Update()
    {
        if (isDeveloping)
        {
            HandleDeveloping();
            return;
        }

        CheckTarget();

        if (Input.GetMouseButtonDown(0))
        {
            TakePhoto();
        }
    }

    void HandleDeveloping()
    {
        developTimer -= Time.deltaTime;

        if (Input.GetKey(KeyCode.E))
        {
            developTimer -= shakeReduceAmount * Time.deltaTime * 5f;
        }

        if (developTimer <= 0f)
        {
            isDeveloping = false;
            developTimer = 0f;
            Debug.Log("Photo developed. Ready for next shot.");
        }
    }

    void TakePhoto()
    {
        bool isZooming = Input.GetMouseButton(1);
        float range = isZooming ? photoRange * 1.5f : photoRange;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
        {
            if (hit.collider.CompareTag("Celebrity"))
            {
                int scoreToAdd = normalScore;

                if (isZooming)
                {
                    scoreToAdd += zoomBonus;
                    Debug.Log("PERFECT SHOT!");
                }

                currentScore += scoreToAdd;
                Debug.Log("Nice photo! +" + scoreToAdd + " | Total: " + currentScore);

                if (currentScore >= quota)
                {
                    Debug.Log("Quota reached!");
                }
            }
            else
            {
                Debug.Log("That wasn't a celebrity.");
            }
        }
        else
        {
            Debug.Log("No target in photo.");
        }

        StartDeveloping();
    }

    void StartDeveloping()
    {
        isDeveloping = true;
        developTimer = developTime;
        Debug.Log("Developing photo... Press E to shake it faster!");
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
                Debug.Log("Target in sight!");
            }
        }
    }
}