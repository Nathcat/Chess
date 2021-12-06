using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocalPoint : MonoBehaviour
{
    public float speed = 20.0f;
    private GameManager gameManager;
    public Quaternion[] sideRotations;
    private int lastSide = 0;

    void Start() {
      gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.chess.turnCounter.turn != lastSide) {
          transform.rotation = sideRotations[gameManager.chess.turnCounter.turn];
        }

        transform.Rotate(Vector3.up * speed * -Input.GetAxis("Horizontal") * Time.deltaTime);
        transform.Rotate(Vector3.right * speed * Input.GetAxis("Vertical") * Time.deltaTime);

        float xRotation = transform.eulerAngles.x;
        if (xRotation < 5f) {
          xRotation = 5f;
        }

        if (xRotation > 60f) {
          xRotation = 60f;
        }

        transform.eulerAngles = new Vector3(xRotation, transform.eulerAngles.y, 0f);

        lastSide = gameManager.chess.turnCounter.turn;
    }
}
