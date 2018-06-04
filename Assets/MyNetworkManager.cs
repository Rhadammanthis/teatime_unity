using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MyNetworkManager : NetworkManager{

    private Camera IdleCamera, MainCamera;

    private bool canRotate = true;

    float rotation, distance = 45f;

    private Vector3 rotationOrigin = new Vector3(-8.4f, -2.07f, 12.4f);

    private int clientCon;

    Text InformationText;

    // Use this for initialization
    void Start () {

        //Local instances of both game cameras
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        IdleCamera = GameObject.FindGameObjectWithTag("Idel").GetComponent<Camera>();

        InformationText = FindObjectOfType<Text>();
    }
	
	// Update is called once per frame
	void Update () {

        //Rotates the IdleCamera around the "center" of the world map
        if (!canRotate)
            return;

        IdleCamera.enabled = true;
        MainCamera.enabled = false;

        rotation += 24 * Time.deltaTime;
        if (rotation >= 360f)
            rotation -= 360;

        IdleCamera.transform.position = rotationOrigin;
        IdleCamera.transform.rotation = Quaternion.Euler(-10f, rotation, 0f);
        IdleCamera.transform.Translate(0f, distance, -distance);
        IdleCamera.transform.LookAt(rotationOrigin);

	}

    public override void OnStartClient(NetworkClient client)
    {
        canRotate = false;
    }

    public override void OnStartHost()
    {
        canRotate = false;
    }

    public override void OnStopClient()
    {
        canRotate = true;
    }

    public override void OnStopHost()
    {
        canRotate = true;
        InformationText.text = "";

    }

    public override void OnServerConnect(NetworkConnection connection)
    {

        //Cheks which message should be displayed according to the number of connections to the server
        if (NetworkServer.connections.Count < 2)
            InformationText.text = "Waiting for another player...";
        else
            InformationText.text = "";
    }

    public void UpdateText(string text)
    {
        InformationText.text = text;
    }

}
