using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AIController : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField]
    float forwardSpeed;
    [SerializeField]
    LineRenderer line;
    [SerializeField]
    Transform BallTransform;
    [SerializeField]
    Transform PlayerTailPos;
    [SerializeField]
    Transform BallParent;

    //public bool isNearEdge;
    public Transform CarFront;

    public Canvas HeadCanvas;
    public Animator DriverAnimator;

    public GameObject AngryEmoji, LaughEmoji;
    public GameObject TireRight, TireLeft;
    public float turnSide;

    public Transform front, right, left;
    RaycastHit hit;
    int layerMask;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        HeadCanvas.transform.LookAt(Camera.main.transform, Vector3.up);
        // Bit shift the index of the layer (8) to get a bit mask
        layerMask = 1 << 7;
    }

    // Update is called once per frame
    void Update()
    {
        line.SetPosition(0, PlayerTailPos.position);
        line.SetPosition(1, BallTransform.position);
        HeadCanvas.transform.LookAt(Camera.main.transform, Vector3.up);

        if (!GameManager.Instance.isGameStarted || GameManager.Instance.isGameOver)
        {
            return;
        }
        if (!Physics.Raycast(front.position, -transform.up * 2, out hit, 3, layerMask))
        {
            Debug.Log("front");
            transform.Rotate(Vector3.up, 1 * 270 * Time.deltaTime);
        }
        else if (!Physics.Raycast(right.position, -transform.up * 2, out hit, 3, layerMask))
        {
            Debug.Log("right");
            transform.Rotate(Vector3.up, -1 * 270 * Time.deltaTime);
        }
        else if (!Physics.Raycast(left.position, -transform.up * 2, out hit, 3, layerMask))
        {
            Debug.Log("left");
            transform.Rotate(Vector3.up, 1 * 270 * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.isGameStarted || GameManager.Instance.isGameOver)
        {
            return;
        }

        rb.AddForce(transform.forward * forwardSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            Debug.Log("AI fall");
            forwardSpeed = 0;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            BallParent.GetComponentInChildren<Rigidbody>().velocity = Vector3.zero;
            BallParent.gameObject.SetActive(false);
            line.gameObject.SetActive(false);
            GameManager.Instance.players.Remove(transform.parent.gameObject);
            GameManager.Instance.CheckPlayerCountForWin();
            transform.parent.gameObject.SetActive(false);
        }
        else if (other.CompareTag("Player"))
        {
            transform.DORotate(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + Random.Range(-2, 3) * 90, transform.eulerAngles.z), 3f);
        }
    }
}
