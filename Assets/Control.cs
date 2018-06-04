using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Control : NetworkBehaviour {

    public Camera IdleCamera;
    private Camera mainCamera;

    //Reference to the swipe manager
    public SwipeManager swipeManager;

    //Vector that represent the "center" of the world from wich pivot the camera rotation
    private Vector3 rotationOrigin = new Vector3(-8.4f, -2.07f, 12.4f);

    //Variables to habdle the rotation of the Idle camera
    float worldCameraRotationAngle, worldCameraRotationDistance = 45f;

    //Angle between the current camera angle and the next Waypoint
    float cameraAngularRotation;

    //Offset to position the Active camera in an isometric view
    private Vector3 cameraOffset = new Vector3(0f, 2f, -6f);

    //Array to hold the world's Waypoints
    public GameObject[] worldWaypoints;
    private int currentWayPoint = 0;

    //Enum to easily differentiate which Player is playing which role
    public enum PlayerRole
    {
        Active = 0,
        Idle = 1
    }

    //Holds the current player's role
    private PlayerRole currentRole;

    //Value of the Player's movement magnitud
    private float playerMovementStep = 0f;

    //Value of the magnitud of the camera rotation
    private float cameraRotation = 0f;

    //Sync variable to handle the role switching timer across clients
    [SyncVar]
    float elapsedTime;

    // Use this for initialization
    void Start () {

        //Checks if this is the local instance of the script and prevents the initializatio of values
        //in a script that would not be used
        if (!isLocalPlayer)
        {
            return;
        }

        //Reference to world cameras
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        IdleCamera  = GameObject.FindGameObjectWithTag("Idel").GetComponent<Camera>();

        //Checks which role this Player while have based on if it was connected to the host
        //in a server or client capacity
        currentRole = isServer ? PlayerRole.Active : PlayerRole.Idle;

        //Looks up the Waypoints in the editor
        worldWaypoints = GameObject.FindGameObjectsWithTag("WayPoint");

        //Checks which camera should be active based on the Player's role
        if(IdleCamera != null && currentRole == PlayerRole.Active)
        {
            IdleCamera.enabled = false;
        }

        //Turns the Player in the direction of the next Waypoint
        transform.LookAt(worldWaypoints[currentWayPoint].transform);

        MoveCamera();
    }


    // Update is called once per frame
    void Update () {

        //Used to prevent from runnig any code that does not belong to this instance
        if (!isLocalPlayer)
        {
            return;
        }

        //Checks if two players are now ready to play
        if (NetworkServer.connections.Count == 2)
        {
           //Calls an RPC function that serves as a timer across clients. This is used to check when the Player roles should be reverted
            RpcUpdateTimer(Time.deltaTime);
        }

        //Checks the Player's role and activates the corresponding camera
        if (currentRole == PlayerRole.Idle)
        {
            IdleCamera.enabled = true;
            mainCamera.enabled = false;
        }
        else
        {
            IdleCamera.enabled = false;
            mainCamera.enabled = true;
        }

        //Checks if the SwipeManager has recorded a SwipeUp
        if (swipeManager.SwipeUp)
        {
            playerMovementStep = 25f;

        }

        //Checks if the SwipeManager has recorded a SwipeUp
        if (swipeManager.DraggedLeft)
        {
            //Ads rotation
            cameraRotation += 70f;
        }

        //Checks if the SwipeManager has recorded a SwipeUp
        if (swipeManager.DraggedRight)
        {
            //Substracts rotation
            cameraRotation -= 70f;
                
        }


        //Checks if the Player has reached a Waypoint
        if (Vector3.Distance(worldWaypoints[currentWayPoint].transform.position, transform.position) <= 2f)
        {
            currentWayPoint++;

            //Prevents overflow
            if(currentWayPoint <= worldWaypoints.Length)
                transform.LookAt(worldWaypoints[currentWayPoint].transform);

        }


        //Handles the movement for the Active player
        if(currentRole == PlayerRole.Active)
        {
            //Creates a Vector with the transfmormation of the Waypoint but sets the y value to the same as the player object
            //and moves it as far as the step value
            Vector3 normalizedWayPoint = worldWaypoints[currentWayPoint].transform.position;
            normalizedWayPoint.y = transform.position.y;
            transform.position = Vector3.MoveTowards(transform.position, normalizedWayPoint, playerMovementStep * Time.deltaTime);

            //Updates the position of the camera relative to the player object while keeping the offset and rotation
            mainCamera.transform.position = transform.position + cameraOffset;
            mainCamera.transform.RotateAround(transform.position, Vector3.up, Time.deltaTime * cameraRotation);
            cameraOffset = mainCamera.transform.position - transform.position;
            mainCamera.transform.LookAt(transform.position);

        }
        //Handles the movement for the Idle Player
        else
        {
            //Rotates the Idle camera
            worldCameraRotationAngle += cameraRotation * Time.deltaTime;
            if (worldCameraRotationAngle >= 360f)
                worldCameraRotationAngle -= 360;

            IdleCamera.transform.position = rotationOrigin;
            IdleCamera.transform.rotation = Quaternion.Euler(-10f, worldCameraRotationAngle, 0f);
            IdleCamera.transform.Translate(0f, worldCameraRotationDistance, -worldCameraRotationDistance);
            IdleCamera.transform.LookAt(rotationOrigin);
        }


        playerMovementStep = 0f;
        cameraRotation = 0f;

    }

    //Updates the role switching timer across the clients
    [ClientRpc]
    void RpcUpdateTimer(float time)
    {
        elapsedTime += time;

        //After 10 seconds have past, the roles are switched
        if (elapsedTime >= 10f)
        {
            elapsedTime = 0f;
            currentRole = currentRole == PlayerRole.Active ? PlayerRole.Idle : PlayerRole.Active;
        }
    }

    //Moves the camera to an isometric type of view behind the Player
    public void MoveCamera()
    {
        mainCamera.transform.position = transform.position;
        mainCamera.transform.rotation = Quaternion.Euler(20f,0f,0f);
        mainCamera.transform.LookAt(transform);
        mainCamera.transform.Translate(cameraOffset);
        
    }
}
