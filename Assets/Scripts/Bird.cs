using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bird : MonoBehaviour
{

    [SerializeField] float speed;
    [SerializeField] float gravity;
    [SerializeField] float jumpForce;
    [SerializeField] float maxYVelocity = 0.1f;
    [SerializeField] Material[] states = new Material[2];

    new MeshRenderer renderer;

    int score;
    float yVel;

    Rigidbody rb;


    bool dead = false;
    bool paused = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        renderer = GetComponent<MeshRenderer>();
    }

    public void Update()
    {
        float dx = 0;
        yVel = Mathf.Min(yVel + gravity, maxYVelocity);
        if (paused && !dead)
        {
            rb.velocity = Vector3.zero;
            return;
        }
        if (dead)
        {
            rb.velocity = new Vector3(rb.velocity.x, -yVel, 0);
            return;
        }
        /*if (Input.GetKey(KeyCode.RightArrow))
        {
            dx = -speed;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            dx = speed;
        }*/
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            yVel = -jumpForce;
            renderer.material = states[1];
            StartCoroutine(swapStates());
            
        }
        rb.velocity = new Vector3(-dx, -yVel, 0);
        Debug.Log(rb.velocity);

    }

    IEnumerator swapStates()
    {
        yield return new WaitForSeconds(0.1f);
        renderer.material = states[0];
    }

    public int GetScore()
    {
        return score;
    }

    public bool IsDead()
    {
        return dead;
    }

    public void UnPause()
    {
        paused = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Collision Detected!");
        if (collision.transform.tag == "Coin")
        {
            Destroy(collision.gameObject);
            score++;
        } else if (collision.transform.tag == "Ground")
        {
            MovingPillars.Stop();
            paused = true;
            dead = true;
            rb.constraints = RigidbodyConstraints.FreezePositionZ;
        }
    }

    internal bool IsPaused()
    {
        return paused;
    }

    public void Die()
    {
        paused = true;
        Debug.Log("Fucking dead bitch");
    }

}