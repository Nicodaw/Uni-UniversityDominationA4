using UnityEngine;

public static class CameraExtensions
{
    /// <summary>
    /// Calculates the bounds of the camera in real-world coordinates.
    /// </summary>
    /// <returns>The bounds of the camera.</returns>
    /// <param name="camera">The camera to get the bounds of.</param>
    /// <remarks>
    /// Sourced from
    /// https://answers.unity.com/questions/501893/calculating-2d-camera-bounds.html.
    /// </remarks>
    public static Bounds OrthographicBounds(this Camera camera)
    {
        float cameraHeight = camera.orthographicSize * 2;
        return new Bounds(camera.transform.position, new Vector3(cameraHeight * camera.aspect, 0, cameraHeight));
    }
}
