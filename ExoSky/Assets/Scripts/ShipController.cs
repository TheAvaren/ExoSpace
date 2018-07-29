using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour {

    public float maxSpeed = 100f;
    public float reverseSpeed = -10f;
    public float stallSpeed = 5f;

    public float speed = 1f;

    public float acceleration = 1f;
    public float deacceleration = 1f;
    public float turnStrength = 10;


    float pitchInput;
    float rollInput;
    float yawInput;

    float rotZ0ResetTime = 0.33f;
    float currentResetTime;
    bool recentStopRotating = false;


    new Rigidbody rigidbody;
    Transform ship;

    float timeSinceCrash;
    float timeToResetCrashState = 1f;
    bool recentCrash;

#if UNITY_EDITOR
    [Header("Transform:")]
    public float rotX;
    public float rotY;
    public float rotZ;
#endif

    // Use this for initialization
    void Start () {
        ship = transform.GetChild(0);
        currentResetTime = rotZ0ResetTime;
        rigidbody = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        TimerUpdate();
        ShipParallelMovement();

        rollInput = Input.GetAxis("Mouse X");
        pitchInput = -Input.GetAxis("Mouse Y");
        yawInput = Input.GetAxis("Horizontal");

        ship.Rotate(new Vector3(pitchInput, yawInput, -rollInput));

#if UNITY_EDITOR
        rotX = transform.localEulerAngles.x;
        rotY = transform.localEulerAngles.y;
        rotZ = transform.localEulerAngles.z;
#endif
    }

    void TimerUpdate()
    {
        if (recentCrash)
        {
            timeSinceCrash += Time.deltaTime;
            if (timeSinceCrash >= timeToResetCrashState)
            {
                timeSinceCrash = 0f;
                recentCrash = false;
            }
        }
        
    }

    void ShipParallelMovement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            speed += Time.deltaTime * acceleration;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            speed -= Time.deltaTime * acceleration;
        }
        else //if (Mathf.Abs(speed) > stallSpeed)
        {
            bool reverse = speed < 0;
            speed -= (reverse ? -Time.deltaTime : Time.deltaTime) * deacceleration;
        }

        speed = Mathf.Clamp(speed, reverseSpeed, maxSpeed);

        if (!recentCrash)
        {
            rigidbody.velocity = ship.forward * speed;
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag != "Ship")
        {
            speed = rigidbody.velocity.magnitude;
            recentCrash = true;
        }
    }
}

