using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeManager : MonoBehaviour
{

    private Vector2 startTouch, swipeDelta, endTouch, continuousTouch;
    private bool tap, isDragging, swipeUp, draggedRight, draggedLeft;

    private float SWIPE_THRESHOLD = 100f, TIME_TRESHHOLD = 0.3f, TIME_TOUCHING;

    // Update is called once per frame
    void Update()
    {

        tap = swipeUp = false;

        //If using a mouse

        //Checks if a click has been performed and records where it happened
        if (Input.GetMouseButtonDown(0))
        {
            ResetSwipe();

            tap = true;
            startTouch = Input.mousePosition;
        }

        //Records where the click ended
        if (Input.GetMouseButtonUp(0))
        {
            endTouch = Input.mousePosition;

            ResetDragg();
        }

        //Records the current position of the click
        if (Input.GetMouseButton(0))
        {
            isDragging = true;
            TIME_TOUCHING += Time.deltaTime;
            continuousTouch = Input.mousePosition;
        }

        //If using Touch inputs
        if (Input.touches.Length > 0)
        {
            //Checks if a click has been performed and records where it happened
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                tap = true;
                startTouch = Input.touches[0].position;
            }

            //Records where the click ended
            if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                endTouch = Input.touches[0].position;

                ResetDragg();
            }

            //Records the current position of the click
            if (Input.touches[0].phase == TouchPhase.Moved || Input.touches[0].phase == TouchPhase.Moved)
            {
                isDragging = true;
                TIME_TOUCHING += Time.deltaTime;
                continuousTouch = Input.touches[0].position;
            }
            
        }

        swipeDelta = Vector2.zero;

        //Checks if the touch has ended in a rapid motion, signalizing a swipe
        if (endTouch != Vector2.zero && TIME_TOUCHING < TIME_TRESHHOLD)
        {
            swipeDelta = endTouch - startTouch;

        }

        //Chekcs the lenght of the swipe and also if it was big enough to be considered a swipe
        if (swipeDelta.magnitude > SWIPE_THRESHOLD)
        {
            float x = swipeDelta.x;
            float y = swipeDelta.y;

            //Chekcs the direction
            if (Mathf.Abs(x) < Mathf.Abs(y))
            {
                //We just care about the swipe up
                if (y > 0)
                    swipeUp = true;
            }

            ResetSwipe();

        }

        //Cheks if the swipe is still ongoing and has lasted for a "long" time signalizing a dragging motion
        if (isDragging && TIME_TOUCHING > TIME_TRESHHOLD)
        {
            swipeDelta = continuousTouch - startTouch;

            //Cheks if the dragg is "big enough" to be considered
            if(swipeDelta.magnitude > SWIPE_THRESHOLD)
            {
                float x = swipeDelta.x;
                float y = swipeDelta.y;

                //Checks the direction of the dragging motion
                if(Mathf.Abs(x) > Mathf.Abs(y))
                {
                    if (x > 0)
                        draggedRight = true;
                    else
                        draggedLeft = true;
                }

            }

        }
    }

    //Getters to be acces by the Control script
    public bool SwipeUp { get { return swipeUp; } }
    public bool DraggedRight { get { return draggedRight; } }
    public bool DraggedLeft { get { return draggedLeft; } }

    //Reset the swipe values
    void ResetSwipe()
    {
        startTouch = swipeDelta = endTouch = Vector2.zero;
    }

    //Reset the drag values
    void ResetDragg()
    {
        TIME_TOUCHING = 0f;
        isDragging = draggedLeft = draggedRight = false;
    }

}
