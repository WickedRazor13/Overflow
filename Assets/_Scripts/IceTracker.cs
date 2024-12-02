using UnityEngine;
using UnityEngine.UI;

public class IceTracker : MonoBehaviour
{
    [Header("Platform Slider Settings")]
    public Slider platformSlider;
    public PlayerCube playerCube;

    void Start()
    {
        if (playerCube == null)
        {
            Debug.LogError("PlayerCube reference is not assigned");
            return;
        }

        if (platformSlider != null)
        {
            // Initialize the slider
            platformSlider.maxValue = PlayerCube.maxPlatforms;
            platformSlider.value = playerCube.platformNumber;
        }
    }

    void Update()
    {
        if (platformSlider != null && playerCube != null)
        {
            // Update the slider to match the current platform count
            platformSlider.value = playerCube.platformNumber;
        }
    }
}
