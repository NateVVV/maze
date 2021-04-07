using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    public Rigidbody rb;
    public int movementSpeed;

    private bool movementEnabled = true;
    public Text looseText;
    public Text winText;
    public Text countAttemptsText;
    private int countAttempts;

    private Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {
        // Preventing mobile devices going in to sleep mode 
        // (problem occurs if only accelerometer input is used)
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        looseText.enabled = false;
        winText.enabled = false;
        countAttempts = 1;
        SetAttemptsText();
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (movementEnabled && transform.position.y <= 0.3)
        {
            var movement = GetMovement();
            try
            {
                rb.AddForce(movement * movementSpeed * Time.deltaTime);
            } catch(System.Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Loose Hole"))
        {
            StartCoroutine(LoosingRoutine(1));
        } else if(other.gameObject.CompareTag("Win Hole"))
        {
            var currentLevelName = other.name;
            StartCoroutine(WinRoutine(1, currentLevelName));
        }
    }

    private IEnumerator LoosingRoutine(float delay)
    {
        movementEnabled = false;
        looseText.enabled = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        countAttempts++;
        yield return new WaitForSeconds(delay);
        SetAttemptsText();
        transform.position = startPosition;
        looseText.enabled = false;
        //yield return new WaitForSeconds(delay);
        movementEnabled = true;
    }

    private IEnumerator WinRoutine(float delay, string currentLevelName)
    {
        movementEnabled = false;
        winText.enabled = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        yield return new WaitForSeconds(delay);
        var currentLevel = SceneManager.GetActiveScene().buildIndex;
        if (currentLevel + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(currentLevel + 1);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
        Debug.Log(currentLevelName);
        //transform.position = startPosition;
        //winText.enabled = false;
        //yield return new WaitForSeconds(delay);
        //movementEnabled = true;
    }

    private Vector3 GetMovement()
    {
        Vector3 movement;
        if(SystemInfo.deviceType == DeviceType.Desktop)
        {
            movement = GetDesktopMovement();
        } else if(SystemInfo.deviceType == DeviceType.Handheld)
        {
            movement = GetMobileMovement();
        } else
        {
            throw new System.Exception("Not a known device type is used");
        }
        return movement;
    }

    private Vector3 GetDesktopMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        return new Vector3(moveHorizontal, 0, moveVertical);
    }

    private Vector3 GetMobileMovement()
    {
        float moveHorizontal = Input.acceleration.x;
        float moveVertical = Input.acceleration.y;

        return new Vector3(moveHorizontal, 0, moveVertical);
    }

    private void SetAttemptsText()
    {
        countAttemptsText.text = "Attempts: " + (countAttempts < 10 ? countAttempts.ToString() : '\u221e'.ToString());
    }
}
