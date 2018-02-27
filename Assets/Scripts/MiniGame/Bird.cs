using System;
using System.Collections;
using UnityEngine;

//Added by Jack
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

    /// <summary>
    /// 
    /// Sets up bird from scene components
    /// 
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        renderer = GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// 
    /// Update bird position
    /// 
    /// </summary>
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
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            yVel = -jumpForce;
            renderer.material = states[1];
            StartCoroutine(SwapStates());
            
        }
        rb.velocity = new Vector3(-dx, -yVel, 0);
        Debug.Log(rb.velocity);

    }

    IEnumerator SwapStates()
    {
        yield return new WaitForSeconds(0.1f);
        renderer.material = states[0];
    }

    /// <summary>
    /// 
    /// Gets the player's current score
    /// 
    /// </summary>
    /// <returns>The player's current score</returns>
    public int GetScore()
    {
        return score;
    }

    /// <summary>
    /// 
    /// Returns if the bird is dead or not
    /// 
    /// </summary>
    /// <returns>True if bird is dead, else false</returns>
    public bool IsDead()
    {
        return dead;
    }

    /// <summary>
    /// 
    /// Unpauses the mini game
    /// 
    /// </summary>
    public void UnPause()
    {
        paused = false;
    }

    /// <summary>
    /// 
    /// Handle the bird colliding with something
    /// 
    /// </summary>
    /// <param name="collision">The collision event</param>
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

    /// <summary>
    /// 
    /// Returns if the game is paused or not
    /// 
    /// </summary>
    /// <returns>True if game is paused else false</returns>
    internal bool IsPaused()
    {
        return paused;
    }

    /// <summary>
    /// 
    /// Pauses the game
    /// 
    /// </summary>
    public void Pause()
    {
        paused = true;
    }

}