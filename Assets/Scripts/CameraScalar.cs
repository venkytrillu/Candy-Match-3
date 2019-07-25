using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScalar : MonoBehaviour
{
    public float cameraOffset;
    private float aspectRatio;
    public float padding;
    float width, height;
    private Board board;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        if(Screen.width<Screen.height)
        {
            width = (float)Screen.width;
            height = (float)Screen.height;
        }
        else
        {
            height = (float)Screen.width;
            width = (float)Screen.height;
        }
        aspectRatio = width /height;//  (float)10 / (float)16;
        //print(aspectRatio);
        
        if(board!=null)
        {
            RotateCamera(board.width - 1, board.height - 1);
        }
    }

    void RotateCamera(float x,float y)
    {
        Vector3 postion = new Vector3(x / 2, y / 2, cameraOffset);
        transform.position = postion;
        if (board.width >= board.height)
            Camera.main.orthographicSize =Mathf.Clamp(((board.width/2)+ padding) / aspectRatio,7,8);
        else
            Camera.main.orthographicSize = Mathf.Clamp((board.height / 2) + padding, 7, 8);

        
    }

    
}//class























































