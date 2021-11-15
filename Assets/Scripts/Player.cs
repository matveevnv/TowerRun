using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Camera camera;
    private bool isGrounded = true;
    public GameObject TapToStart;
    public  float maxForce;
    public  float jumpForce;
    public  float forceIncrement;
    public Text jumpForceText;
    public bool isGame;
    public float movementSpeed;

    public int size = 1;
    public MeshRenderer[] playerCubesMesh;
    public int maxSize;
    public GameObject cube;

    private Rigidbody rigidbody;

    private float force = 0;

    private void Awake()
    {
        TapToStart.SetActive(true);
        Time.timeScale = 0;
    }

    public void StartGame()
    {
        TapToStart.SetActive(false);
        Time.timeScale = 1;
    }

    // Start is called before the first frame update
    private void Start()
    {
        InitPlayerCubes();
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (isGame)
        {
            transform.Translate(Vector3.forward * movementSpeed);
        }

        if (Input.touchCount > 0 && isGrounded)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Stationary:
                    force += Time.deltaTime * forceIncrement;
                    break;
                case TouchPhase.Ended:
                    Jump();
                    break;
            }
        }
    }

    public void Jump()
    {
        isGrounded = false;

        if (force > maxForce) force = maxForce;
        if (force < jumpForce) force = jumpForce;
        rigidbody.AddForce(new Vector3(0, force, 0));
        force = 0;
    }

    private void CameraFoV()
    {
        if (size < 3) camera.fieldOfView = 40;
        if (size >= 3 && size < 6) camera.fieldOfView = 45;
        if (size >= 6 && size < 9) camera.fieldOfView = 50;
        if (size >= 9 && size < 12) camera.fieldOfView = 60;
        if (size >= 12 && size < 15) camera.fieldOfView = 80;
        if (size > 15) camera.fieldOfView = 100;
    }

    IEnumerator FOV(float newFoV)
    {
        if (newFoV > camera.fieldOfView)
        {
            while (camera.fieldOfView < newFoV)
            {
                camera.fieldOfView += 0.5f;
                yield return new WaitForSecondsRealtime(0.01f);
            }
        }
        else
        {
            while (camera.fieldOfView > newFoV)
            {
                camera.fieldOfView -= 0.5f;
                yield return new WaitForSecondsRealtime(0.01f);
            }
        }
    }

    private void GrowUp()
    {
        if(size < maxSize)
        {
            size++;
            RefreshSkin();
        }
    }

    private void GrowDown()
    {
        size--;
        if (size <= 0)
        {
            StartCoroutine(End("Wasted"));
        }
        else
        {
            HideBottom();
        }
    }

    private void HideBottom()
    {
        playerCubesMesh[0].enabled = false;
    }

    private void GameOver()
    {
        Debug.Log("Game over");
        Application.LoadLevel(0);
    }

    public void InitPlayerCubes()
    {
        playerCubesMesh = new MeshRenderer[maxSize];

        for (int i = 0; i < maxSize; i++)
        {
           GameObject newCube = Instantiate(cube, new Vector3(0, 0.5f + 1.01f * i, 0), Quaternion.identity);
           newCube.transform.SetParent(this.gameObject.transform);
           playerCubesMesh[i] = newCube.GetComponent<MeshRenderer>();
        }

        RefreshSkin();
    }

    public void RefreshSkin()
    {
        foreach (var a in playerCubesMesh) a.enabled = false;

        for( int i = 0; i < size; i++)
        {
            playerCubesMesh[i].enabled = true;
        }

        CameraFoV();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hit " + other.gameObject.name);

        if (other.gameObject.tag == "Neutral")
        {
            GrowUp();
            Debug.Log("neutral size " + size);
        }

        if (other.gameObject.tag == "Enemy")
        {
            GrowDown();
            Debug.Log("enemy size " + size);
        }

        if (other.gameObject.tag == "Finish")
        {
            StartCoroutine(End("WIN"));
            Debug.Log("enemy size " + size);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            RefreshSkin();
            Debug.Log("enemy exit size " + size);
        }
    }

    IEnumerator End(string roundResult)
    {
        Time.timeScale = 0;
        TapToStart.SetActive(true);
        jumpForceText.text = roundResult;
        yield return new WaitForSecondsRealtime(1.5f);
        GameOver();
    }


}
