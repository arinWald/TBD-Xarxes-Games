using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float horizontalInput = 0f;
    public float verticalInput = 0f;
    public bool spaceInput = false;
    public bool spaceInputUp = false;
    public bool spaceInputDown = false;
    public bool shiftInput = false;
    public float rotationAngle = 0f;

    [Header("Movement")]
    public float movementSpeed;

    [Header("Ball")]
    public GameObject ball;
    public float kickStrenght;
    public float kickMaxTime;

    private Rigidbody ballRb;
    private Ball ball_c;
    private float kickTime;
    public bool ballPossesion = false;
    private GameObject ballSensor;

    [Header("Snatch")]
    public float snatchMaxTime;

    private float snatchTime;

    [HideInInspector]
    public bool snatch = false;

    public bool IAmRemote;

    void Start()
    {
        ballRb = ball.GetComponent<Rigidbody>();
        ball_c = ball.GetComponent<Ball>();

        ballSensor = transform.Find("BallSensor").gameObject;

        ballPossesion = false;
    }

    void Update()
    {

        if(!IAmRemote)
        {
            GetInput();
        }

        //Kick ball
        if (ballPossesion)
        {
            if (spaceInput)
            {

                if (kickTime < kickMaxTime) kickTime += Time.deltaTime;
            }
            else if (spaceInputUp)
            {
                //Debug.Log("SpaceUp: " + kickTime);
                float normTime = kickTime / kickMaxTime;
                float strenght = normTime * kickStrenght;

                ballRb.AddForce(gameObject.transform.forward * strenght);
                ball_c.ChangePossesion(null);
                ballPossesion = false;
            }
            else
            {
                kickTime = 0;
            }
        }

        //Move Player
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);

        if (movement.magnitude > 1)
        {
            movement.Normalize();
        }

        transform.position += movement * movementSpeed * Time.deltaTime; ;


        //Player Orientation

        transform.rotation = Quaternion.Euler(new Vector3(0, -rotationAngle + 90, 0));

        //Snatch Ball

        if (spaceInputDown && ballPossesion == false && snatchTime <= 0)
        {

            snatch = true;
            snatchTime = snatchMaxTime;
        }

        if (snatch)
        {
            if (snatchTime <= 0)
            {
                snatch = false;
            }
            else
            {
                snatchTime--;
            }
        }
    }

    void GetInput()
    {
        Vector3 mouse_pos = Input.mousePosition;
        Vector3 object_pos = Camera.main.WorldToScreenPoint(transform.position);
        mouse_pos.x = mouse_pos.x - object_pos.x;
        mouse_pos.y = mouse_pos.y - object_pos.y;
        
        rotationAngle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        
        spaceInput = Input.GetKey(KeyCode.Space);
        spaceInputUp = Input.GetKeyUp(KeyCode.Space);
        spaceInputDown = Input.GetKeyDown(KeyCode.Space);
        shiftInput = Input.GetKey(KeyCode.LeftShift);
    }
}
