using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverControl : MonoBehaviour {

    public float speed = 2f;
    public float amplitude = 0.2f;

    private float initialY;
    private Vector3 hoverOffset;

	// Use this for initialization
	void Start () {

        initialY = transform.position.y;
        hoverOffset = transform.position;
		
	}

    // Update is called once per frame
    void Update()
    {

        //Applies a light hover effect
        hoverOffset.y = initialY + amplitude * Mathf.Sin(speed * Time.time);
        transform.position = hoverOffset;

    }
}
