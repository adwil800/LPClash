using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UICameraMovement : MonoBehaviour
{




    private Camera UIcam;
    public Transform target;
    private Vector3 previousPosition;

    private void Start()
    {
        UIcam = GetComponent<Camera>();
    }


    void Update()
    {
        movePlayerPreview();    

    }
    //CHECK PlAYER CONTROLLER AND DETERMINE HOW IT CAN MOVE EVEN IF ITS DISABLED AND RESTORE MOUSE AT CENTER, DETERMINE THE CLICKED ITEM TO BE ONLY WITHIN THE PLAYEPREVIEW RANGE TO MOVE THE PREVIEW

    public void movePlayerPreview()
    {

        if (Input.GetMouseButtonDown(0) )
        {
            previousPosition = UIcam.ScreenToViewportPoint(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
        }
        if (Input.GetMouseButton(0))
        {



            Vector3 direction = previousPosition - UIcam.ScreenToViewportPoint(Input.mousePosition);

            UIcam.transform.position = target.position;// new Vector3();

            //UIcam.transform.Rotate(new Vector3(1, 0, 0), direction.y * 180); //Rotate up and down
            UIcam.transform.Rotate(new Vector3(0, 1, 0), -direction.x * 180); //Rotate left and right
            UIcam.transform.Translate(new Vector3(0, -0.5f, -1.5f));

            previousPosition = UIcam.ScreenToViewportPoint(Input.mousePosition);

        }
    }

}
