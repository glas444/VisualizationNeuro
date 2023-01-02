using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
  Script which enables changing orientation of the observer (rotates camera
  around itself) using the mouse in the desktop setting. Adjust xSens and ySens
  inside Unity to change the sensitivity (how much the camera rotates based on
  mouse movements).
*/

public class LookDirection : MonoBehaviour
{
    public GameObject marker;
    public Material transparentMat;
    public Material solidMat;
    protected bool transparencyEnabled;

    public float xSens;
    public float ySens;
    float xRot;
    float yRot;

    public float xSensfix;
    public float ySensfix;
    float xRotfix;
    float yRotfix;

    private Quaternion targetRot;
    private Quaternion currentRot;
    private Vector3 dirToMark;
    private float deltacam;
    public float slerpSmoothValue = 0.3f;
    private bool startrot;

    public Vector3 markerDist = new Vector3(0, 0, 1);
    public Transform lookDir;
    public Transform orientation;
    // Start is called before the first frame update
    void Start()
    {
        startrot = true;
        marker.transform.position = this.transform.position + transform.rotation * markerDist;
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //RIGHTMOUSEBUTTON
        if (Input.GetMouseButton(1))
        {
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            // mouse input
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * xSens;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * ySens;

            xRot -= mouseY;
            yRot += mouseX;
            //Debug.Log(xRot);

            // limits PoV up/down
            xRot = Mathf.Clamp(xRot, -90f, 90f);

            // rotate cam
            lookDir.rotation = Quaternion.Euler(xRot, yRot, 0);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            
            Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.Confined;
            Cursor.lockState = CursorLockMode.None;
        }

        //LEFTMOUSEBUTTON
        if (Input.GetMouseButton(0) && marker.activeSelf == true)
        {
           
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            // mouse input
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * xSens;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * ySens;

            xRot -= mouseY;
            yRot += mouseX;

            //Debug.Log(xRotfix);
            
            Vector3 tempV = new Vector3(xRot, yRot, 0);
            targetRot = Quaternion.Euler(tempV); //We are setting the rotation around X, Y, Z axis respectively

            //Rotate Camera
            currentRot = Quaternion.Slerp(currentRot, targetRot, Time.smoothDeltaTime * slerpSmoothValue * 50);  //let cameraRot value gradually reach newQ which corresponds to our touch
            if (startrot)
            {
                Debug.Log("Ea");
                deltacam = Vector3.Distance(marker.transform.position, transform.position);
                dirToMark = new Vector3(0, 0, -deltacam);
                startrot = false;
                currentRot = transform.rotation;
            }                                                                                                        //Multiplying a quaternion by a Vector3 is essentially to apply the rotation to the Vector3
                                                                                                                     //This case it's like rotate a stick the length of the distance between the camera and the target and then look at the target to rotate the camera.
            transform.position = marker.transform.position + currentRot * dirToMark;
            transform.LookAt(marker.transform.position);
            this.GetComponent<ObserverMovement>().observerPosition = transform.position;
        }
        else if (Input.GetMouseButtonUp(0))
        {

            
            startrot = true;
            Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.Confined;
            Cursor.lockState = CursorLockMode.None;
        }



        if (Input.GetMouseButtonDown(2))
        {
            if (marker.activeSelf == false)
            {
                transparencyEnabled = false;
            }
            ToggleTransparency();
        }

        if (transparencyEnabled)
        {
            marker.transform.position = this.transform.position + transform.rotation * markerDist;
        }

        Vector3 pos = marker.transform.position;
        pos.z += Input.mouseScrollDelta.y * 1;
        marker.transform.position = pos;


        //else if (Input.GetMouseButtonUp(2))
        //{
        //    ToggleTransparency();
        //}
    }
    void ToggleTransparency()
    {
        Renderer markerRenderer = marker.GetComponent<Renderer>();
        if (!transparencyEnabled)
        {
            //markerDist = this.transform.position - marker.transform.position;
            //marker.transform.position = this.transform.position + transform.rotation * markerDist;
            markerDist = this.transform.position - marker.transform.position;
            markerDist = new Vector3(0, 0, markerDist.magnitude);
            markerTransparent();
            transparencyEnabled = true;
        }
        else if (transparencyEnabled)
        {
            markerSolid();
            transparencyEnabled = false;

        }
    }

    void markerTransparent()
    {
        Renderer skullRenderer = marker.GetComponent<Renderer>();
        skullRenderer.material = transparentMat;
    }
    void markerSolid()
    {
        Renderer skullRenderer = marker.GetComponent<Renderer>();
        skullRenderer.material = solidMat;
    }
}
