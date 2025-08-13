using UnityEngine;
using Unity.Cinemachine;

public class FolowCamera : MonoBehaviour
{
    private CinemachineCamera virtualCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the CinemachineVirtualCamera component
        virtualCamera = GetComponent<CinemachineCamera>();
        
        // Find the player in the scene
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            // Set the player as the follow target
            virtualCamera.Follow = player.transform;
        }
        else
        {
            Debug.LogWarning("Player not found in the scene! Make sure the player has the 'Player' tag.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
