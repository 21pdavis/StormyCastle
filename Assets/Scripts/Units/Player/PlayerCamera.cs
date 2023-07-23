using Cinemachine;
using System.Collections;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    Camera cam;
    CinemachineVirtualCamera virtualCam;

    void Start()
    {
        cam = Camera.main;
        StartCoroutine(WaitForCameraInitialization());
    }

    void Update()
    {
        if (virtualCam != null)
        {
            // virtualCam.m_Lens.OrthographicSize = 1;
        }
    }

    private IEnumerator WaitForCameraInitialization()
    {
        yield return new WaitForEndOfFrame();

        CinemachineBrain brain = cam.GetComponent<CinemachineBrain>();

        if (brain != null && brain.ActiveVirtualCamera != null)
        {
            virtualCam = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        }
    }
}
