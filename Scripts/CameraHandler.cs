using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    private float orthographicSize;
    private float targetorthographicSize;
    // Start is called before the first frame update
    private void Start()
    {
        orthographicSize = cinemachineVirtualCamera.m_Lens.OrthographicSize;
        targetorthographicSize = orthographicSize;
    }

    private void HandleZoom()
    {
        float zoomAmount = 2f;
        float minorthographicSize = 10;
        float maxorthographicSize = 30;
        float zoomSpeed = 5f;

        orthographicSize += Input.mouseScrollDelta.y * zoomAmount;
        targetorthographicSize = Mathf.Clamp(orthographicSize, minorthographicSize, maxorthographicSize);
        orthographicSize = Mathf.Lerp(orthographicSize, targetorthographicSize, Time.deltaTime * zoomSpeed);
        cinemachineVirtualCamera.m_Lens.OrthographicSize = orthographicSize;

    }
    private void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = new Vector3(x, y).normalized;
        float moveSpeed = 30f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

    }
    private void Update()
    {
        HandleMovement();
        HandleZoom();



    }
}
