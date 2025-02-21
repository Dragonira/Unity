using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Transform _camera;
    // Update is called once per frame
    void Update()
    {
        transform.position = _camera.position;
    }
}
