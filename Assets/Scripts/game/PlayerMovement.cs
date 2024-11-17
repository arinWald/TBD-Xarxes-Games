using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    //enum INPUTS
    //{
    //    UNKNOWN = -1,
    //    HORIZONTAL,
    //    VERTICAL,
    //    SPACE,
    //    SHIFT
    //}

    float horizontalInput = 0f;
    float verticalInput = 0f;
    bool spaceInput = false;
    bool spaceInputUp = false;
    bool spaceInputDown = false;
    bool shiftInput = false;

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

    void Start()
    {
        ballRb = ball.GetComponent<Rigidbody>();
        ball_c = ball.GetComponent<Ball>();

        ballSensor = transform.Find("BallSensor").gameObject;

        ballPossesion = false;
    }

    // Update is called once per frame
    void Update()
    {

        GetInput();

        //Kick ball
        if (ballPossesion)
        {
            if (spaceInput)
            {

                Debug.Log("Space: " + kickTime);

                if (kickTime < kickMaxTime) kickTime += Time.deltaTime;
            }
            else if (spaceInputUp)
            {
                Debug.Log("SpaceUp: " + kickTime);
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

        Vector3 mouse_pos = Input.mousePosition;
        Vector3 object_pos = Camera.main.WorldToScreenPoint(transform.position);
        mouse_pos.x = mouse_pos.x - object_pos.x;
        mouse_pos.y = mouse_pos.y - object_pos.y;
        float angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, -angle + 90, 0));

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
        // 4 bytes
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        // 1 bytes
        spaceInput = Input.GetKey(KeyCode.Space);
        spaceInputUp = Input.GetKeyUp(KeyCode.Space);
        spaceInputDown = Input.GetKeyDown(KeyCode.Space);
        shiftInput = Input.GetKey(KeyCode.LeftShift);
    }
}
