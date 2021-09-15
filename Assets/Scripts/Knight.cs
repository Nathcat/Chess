using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece
{
    private GameManager gameManager;
    private Vector3[] legalMoves = {
        new Vector3(1.0f, 0.0f, 2.0f),
        new Vector3(2.0f, 0.0f, 1.0f),
        new Vector3(-1.0f, 0.0f, 2.0f),
        new Vector3(-2.0f, 0.0f, 1.0f),
        new Vector3(1.0f, 0.0f, -2.0f),
        new Vector3(2.0f, 0.0f, -1.0f),
        new Vector3(-1.0f, 0.0f, -2.0f),
        new Vector3(-2.0f, 0.0f, -1.0f)
    };

    public int side;
    private int oppositeSide;
    private string[] sideNames = {"White", "Black"};

    public bool moving = false;

    void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (side == 0) {

            oppositeSide = 1;

        } else {

            oppositeSide = 0;

        }
    }

    void OnMouseDown() {

        if (gameManager.turn == side && !moving) {

            foreach (GameObject moveToken in GameObject.FindGameObjectsWithTag("LegalMoveToken")) {
                Destroy(moveToken);
            }

            foreach (Vector3 legalMove in legalMoves) {

                Collider[] piecesInSquare = Physics.OverlapSphere(transform.position + legalMove, 0.5f);

                if (piecesInSquare.Length == 1) {
                    if (piecesInSquare[0].transform.gameObject.CompareTag(sideNames[oppositeSide])) {
                        GameObject attackToken = Instantiate(gameManager.attackToken, transform.position + legalMove, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
                        attackToken.GetComponent<AttackToken>().parentPiece = this;
                        attackToken.GetComponent<AttackToken>().attackedPiece = piecesInSquare[0].transform.gameObject;
                    }
                }

                if (piecesInSquare.Length == 0) {
                    GameObject moveToken = Instantiate(gameManager.legalMoveToken, transform.position + legalMove, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
                    moveToken.GetComponent<MoveToken>().parentPiece = this;
                }

            }
        }

    }

    override public void move(GameObject legalMoveToken) {

        StartCoroutine(moveOverTime(legalMoveToken.transform.position));

        foreach (GameObject moveToken in GameObject.FindGameObjectsWithTag("LegalMoveToken")) {
            Destroy(moveToken);
        }

        gameManager.togglePlayer();
    }
}
