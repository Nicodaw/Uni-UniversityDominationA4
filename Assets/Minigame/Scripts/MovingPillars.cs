using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPillars : MonoBehaviour {

    [SerializeField] float speed;

    static bool stopped = false;

	// Update is called once per frame
	void Update () {
        transform.position -= new Vector3(stopped ? 0 : speed * Time.deltaTime, 0, 0);
	}

    public static void Stop()
    {
        stopped = true;
    }

    public static void Reset()
    {
        stopped = false;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
}
