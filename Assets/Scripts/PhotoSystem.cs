using UnityEngine;

public class PhotoSystem : MonoBehaviour
{
    public Camera playerCamera;
    public float photoRange = 20f;
    public int photoScore = 100;

    public int currentScore = 0;
    public int quota = 500;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TakePhoto();
        }
    }

    void TakePhoto()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, photoRange))
        {
            if (hit.collider.CompareTag("Celebrity"))
            {
                currentScore += photoScore;
                Debug.Log("Nice photo! Score: " + currentScore);

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
    }
}