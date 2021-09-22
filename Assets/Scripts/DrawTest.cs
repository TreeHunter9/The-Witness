using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class DrawTest : MonoBehaviour
{
    public Camera m_camera;
    public GameObject brush;

    LineRenderer currentLineRenderer;

    Vector3 lastPos;
    private List<Vector3> list;
    

    private void Update()
    {
        Drawing();
    }

    void Drawing() 
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            CreateBrush();
        }
        else if (Input.GetKey(KeyCode.Mouse0))
        {
            PointToMousePos();
        }
        else 
        {
            currentLineRenderer = null;
        }
    }

    void CreateBrush() 
    {
        GameObject brushInstance = Instantiate(brush);
        currentLineRenderer = brushInstance.GetComponent<LineRenderer>();

        //because you gotta have 2 points to start a line renderer,
        var mouse = Input.mousePosition;
        mouse.z = 10;
        Vector3 mousePos = m_camera.ScreenToWorldPoint(mouse);

        currentLineRenderer.SetPosition(0, mousePos);
        currentLineRenderer.SetPosition(1, mousePos);
        list = new List<Vector3>() {Vector3.zero};

    }

    void AddAPoint(Vector3 pointPos) 
    {
        currentLineRenderer.positionCount++;
        int positionIndex = currentLineRenderer.positionCount - 1;
        currentLineRenderer.SetPosition(positionIndex, pointPos);
        list.Add(pointPos);
    }

    void PointToMousePos() 
    {
        var mouse = Input.mousePosition;
        mouse.z = 10;
        Vector3 mousePos = m_camera.ScreenToWorldPoint(mouse);
        if (list.Last() == mousePos)
        {
            currentLineRenderer.positionCount--;
        }
        else if (lastPos != mousePos) 
        {
            AddAPoint(mousePos);
            lastPos = mousePos;
        }
    }
}
