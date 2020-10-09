using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private float scroll;
    private float scrollMin;
    private float scrollMax;
    private float startTimeScroll;
    private float startTimeZoom;
    private float xMin;
    private float xMax;
    private float yMin;
    private float yMax;
    private float targetX;
    private float targetY;
    private float dragSpeed;

    private bool dragging;
    private bool shift;
    private bool zooming;


    private Vector3 lastPosition;

    void Start()
    {
        scroll = -10;
        scrollMin = -32;
        scrollMax = -2;
        startTimeScroll = Time.time;
        startTimeZoom = Time.time;
        dragSpeed = 0.01f;

        shift = false;
        zooming = false;

        GameObject board = GameObject.FindGameObjectWithTag("EditBoard");
        float b_width = board.transform.localScale.x;
        float b_height = board.transform.localScale.y;
        xMin = -0.5f - b_width / 2;
        xMax = b_width * 2 - 0.5f;
        yMin = 0.5f - b_height / 2;
        yMax = b_height * 2 - 0.5f;
        lastPosition = Vector3.zero;
    }

    void Update()
    {
        if (GameObject.FindGameObjectWithTag("EditBoard").GetComponent<Board>().IsUIOpen())
            return;
       
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
            ScreenPan();
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

        lastPosition = Input.mousePosition;
    }

    private void Zoom(bool dir)
    {
        //dir: true=in, false=out
        scroll += dir ? 1 : -1;
        startTimeScroll = Time.time;
    }

    public void FocusOn(float x, float y)
    {
        targetX = x;
        targetY = y;
        zooming = true;
        startTimeZoom = Time.time;
    }
    public void ScreenPan()
    {
        Vector3 delta = lastPosition - Input.mousePosition;
        transform.Translate(delta.x * dragSpeed, delta.y * dragSpeed, 0);
    }
}
