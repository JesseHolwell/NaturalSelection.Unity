using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    private float moveSpeed = 1f;
    private float zoomSpeed = 10f;

    private Camera cameraObj;

    internal life selected;
    internal text text;
    public GameObject world;

    // Start is called before the first frame update
    void Start()
    {
        cameraObj = this.GetComponent<Camera>();
        text = world.GetComponent<text>();
    }

    // Update is called once per frame
    void Update()
    {
        CameraControl();

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Pressed primary button.");
            Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            //Debug.DrawRay(rayPos, Vector2.up, Color.red, 10f);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f);

            if (hit)
            {
                selected = hit.transform.gameObject.GetComponent<life>();

                Debug.Log("hit");
                Debug.Log(hit.transform.name);
                //return ;

            }
            else
            {
                Debug.Log("no hit");
                //selected = null;
            }

        }

        if (selected != null)
        {
            this.transform.position =
                new Vector3(selected.transform.position.x,
                selected.transform.position.y, this.transform.position.z);

        }

        text.selected = selected;
    }

    void CameraControl()
    {
        //FOV: orth camera only
        //but that breaks the click raycast
        //var normalizedMoveSpeed = moveSpeed / cameraObj.fieldOfView / 60;

        //perspective:
        var normalizedMoveSpeed = moveSpeed;

        //up down left right
        // * Time.deltaTime ?
        if (Input.GetAxisRaw("Horizontal") > 0f)
        {
            transform.Translate(new Vector3(normalizedMoveSpeed, 0, 0));
            selected = null;
            //transform.Translate(new Vector3(moveSpeed * Time.deltaTime, 0, 0));
            Debug.Log("right");
        }
        if (Input.GetAxisRaw("Horizontal") < 0f)
        {
            transform.Translate(new Vector3(-normalizedMoveSpeed, 0, 0));
            selected = null;
            //transform.Translate(new Vector3(-moveSpeed * Time.deltaTime, 0, 0));
            Debug.Log("left");
        }
        if (Input.GetAxisRaw("Vertical") > 0f)
        {
            transform.Translate(new Vector3(0, normalizedMoveSpeed, 0));
            selected = null;
            //transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
            Debug.Log("up");
        }
        if (Input.GetAxisRaw("Vertical") < 0f)
        {
            transform.Translate(new Vector3(0, -normalizedMoveSpeed, 0));
            selected = null;

            Debug.Log("down");
        }

        if (transform.position.x > 20)
            transform.position = new Vector3(20, transform.position.y, transform.position.z);
        else if (transform.position.x < -20)
            transform.position = new Vector3(-20, transform.position.y, transform.position.z);
        if (transform.position.y > 10)
            transform.position = new Vector3(transform.position.x, 10, transform.position.z);
        else if (transform.position.y < -10)
            transform.position = new Vector3(transform.position.x, -10, transform.position.z);


        //perspective zoom zoomSpeed is comfortable around 50
        //zoom
        //transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime);

        //this.GetComponent<Camera>().fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;

        //if (cameraObj.fieldOfView >= 15 && cameraObj.fieldOfView <= 75)
        //{
        //cameraObj.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed; // * 0.01f;
        //}

        //if (cameraObj.fieldOfView < 15)
        //    cameraObj.fieldOfView = 15;

        //if (cameraObj.fieldOfView > 75)
        //    cameraObj.fieldOfView = 75;

        var delta = Input.GetAxis("Mouse ScrollWheel");
        if (delta != 0)
            cameraObj.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        //between 5 and 15

        if (cameraObj.orthographicSize > 15)
            cameraObj.orthographicSize = 15;
        else if (cameraObj.orthographicSize < 5)
            cameraObj.orthographicSize = 5;




    }
}
