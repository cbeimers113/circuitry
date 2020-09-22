using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private float scroll;
    private float scrollMin;
    private float scrollMax;
    private float startTimeScroll;
    private float startTimeZoom;

    private int dragSpeed;
    private int dragSpeedMin;
    private int dragSpeedMax;

    private float xMin;
    private float xMax;
    private float yMin;
    private float yMax;

    private float targetX;
    private float targetY;

    private bool dragging;
    private bool shift;
    private bool zooming;

    void Start()
    {
        scroll = -10;
        scrollMin = -12;
        scrollMax = -2;
        startTimeScroll = Time.time;
        startTimeZoom = Time.time;
        dragSpeedMin = 5;
        dragSpeedMax = 25;
        dragSpeed = GetDragSpeed();
        dragging = false;
        shift = false;
        zooming = false;

        GameObject board = GameObject.FindGameObjectWithTag("EditBoard");
        float b_width = board.transform.localScale.x;
        float b_height = board.transform.localScale.y;
        xMin = -0.5f - b_width / 2;
        xMax = b_width * 2 - 0.5f;
        yMin = 0.5f - b_height / 2;
        yMax = b_height * 2 - 0.5f;
    }

    void Update()
    {
        float x = transform.position.x;
        float y = transform.position.y;
        float z = Mathf.SmoothStep(transform.position.z, scroll, (Time.time - startTimeScroll) / 2.0f);
        transform.position = new Vector3(x, y, z);

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            shift = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            shift = false;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (!shift && scroll < scrollMax)
            {
                Zoom(true);
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (!shift && scroll > scrollMin)
            {
                Zoom(false);
            }
        }

        if (dragging)
        {
            float xOffs = Input.GetAxis("Mouse X") * dragSpeed * Time.deltaTime;
            float yOffs = Input.GetAxis("Mouse Y") * dragSpeed * Time.deltaTime;
            float x_target = x - xOffs;
            float y_target = y - yOffs;
            if (x_target >= xMin && x_target <= xMax && y_target >= yMin && y_target <= yMax)
                transform.position = new Vector3(x_target, y_target, z);
        }

        if (zooming)
        {
            x = Mathf.SmoothStep(transform.position.x, targetX, (Time.time - startTimeZoom) / 2.0f);
            y = Mathf.SmoothStep(transform.position.y, targetY, (Time.time - startTimeZoom) / 2.0f);
            transform.position = new Vector3(x, y, z);
        }

        if (Input.GetMouseButtonDown(2) && !shift)
        {
            dragging = true;
            zooming = false;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            dragging = false;
        }
    }

    private void Zoom(bool dir)
    {
        //dir: true=in, false=out
        scroll += dir ? 1 : -1;
        startTimeScroll = Time.time;
        dragSpeed = GetDragSpeed();
    }

    private int GetDragSpeed()
    {
        return dragSpeedMin + (int)((float)(dragSpeedMax - dragSpeedMin) * (1.0f - ((float)(scroll - scrollMin) / (float)(scrollMax - scrollMin))));
    }

    public void ZoomTo(float x, float y)
    {
        targetX = x;
        targetY = y;
        zooming = true;
        startTimeZoom = Time.time;
    }
}
