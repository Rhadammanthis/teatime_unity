using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public GameObject Waypoint1;
    public GameObject Waypoint2;
    public GameObject Waypoint3;
    public GameObject Waypoint4;

    

    private int connectedPlayers;

    // Use this for initialization
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {

    }

    public void addNewPlayer()
    {
        connectedPlayers++;
    }



    public int ConnectedPlayers { get { return connectedPlayers; } }
}
