using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBox : MonoBehaviour
{
    public enum PowerUpType
    {
        TurnBall,
        Shield
    }

    public PowerUpType myType;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            GetComponent<Animator>().SetTrigger("DeflateParachute");
        }
        else if (collision.collider.CompareTag("Ball"))
        {
            Destroy(gameObject);
        }
    }
}
