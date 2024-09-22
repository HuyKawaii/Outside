using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    private Vector3[] rotationList = new Vector3[4];
    private Vector3 defaultRotaition = new Vector3(10, 90, 0);
    private float verticalMaxOffset = 2;
    private float horizontalMaxOffset = 2;
    private float panningMaxTimer = 20.0f;
    private float panningTimer;
    private int rotationIndex;
   
    void Start()
    {
        rotationIndex = 0;
        panningTimer = panningMaxTimer / 2;

        rotationList[0] = defaultRotaition + new Vector3 (-verticalMaxOffset, -horizontalMaxOffset, 0);     
        rotationList[1] = defaultRotaition + new Vector3 (verticalMaxOffset, horizontalMaxOffset, 0);
        rotationList[2] = defaultRotaition + new Vector3 (verticalMaxOffset, -horizontalMaxOffset, 0);     
        rotationList[3] = defaultRotaition + new Vector3 (-verticalMaxOffset, horizontalMaxOffset, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (panningTimer < panningMaxTimer)
        {
            panningTimer += Time.deltaTime;
            cameraTransform.rotation = Quaternion.Euler(Vector3.Lerp(rotationList[GetPreviousIndex()], rotationList[rotationIndex], panningTimer / panningMaxTimer));
        }
        else
        {
            panningTimer = 0;
            rotationIndex = GetNextIndex();
        }
    }

    private int GetPreviousIndex()
    {
        if (rotationIndex == 0)
            return rotationList.Length - 1;
        else
            return rotationIndex - 1;
    }

    private int GetNextIndex()
    {
        if (rotationIndex == rotationList.Length - 1)
            return 0;
        else
            return rotationIndex + 1;
    }
}
