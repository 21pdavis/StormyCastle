using Cinemachine;
using System.Collections;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    Camera cam;
    CinemachineVirtualCamera virtualCam; // to manipulate the camera's size/tracking target
    CinemachineFramingTransposer transposer; // to manipulate the camera's position

    void Start()
    {
        cam = Camera.main;
        StartCoroutine(WaitForCameraInitialization());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Room"))
        {
            //virtualCam.m_Lens.OrthographicSize = 1;
            virtualCam.Follow = collision.transform;
            virtualCam.LookAt = collision.transform;
            transposer.m_TrackedObjectOffset = new Vector3(0f, 0f, 0f);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Room Group"))
        {
            //virtualCam.m_Lens.OrthographicSize = 1;
            virtualCam.Follow = transform;
            virtualCam.LookAt = transform;
            // offset of player is slightly up because player Transform is anchored to bottom of sprite and that's where camera focuses
            transposer.m_TrackedObjectOffset = new Vector3(0, 0.5f, 0);
        }
    }

    private IEnumerator WaitForCameraInitialization()
    {
        // Cinemachine camera is initialized after the first frame, so we wait for the end of the first frame before we try to access it
        yield return new WaitForEndOfFrame();

        CinemachineBrain brain = cam.GetComponent<CinemachineBrain>();

        if (brain != null && brain.ActiveVirtualCamera != null)
        {
            virtualCam = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
            transposer = virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
    }
}
