using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : ChessPiece
{
    private GameManager gameManager;
    public Vector3[,] legalMoves = {
        {
            new Vector3(1.0f, 0.0f, 1.0f),
            new Vector3(2.0f, 0.0f, 2.0f),
            new Vector3(3.0f, 0.0f, 3.0f),
            new Vector3(4.0f, 0.0f, 4.0f),
            new Vector3(5.0f, 0.0f, 5.0f),
            new Vector3(6.0f, 0.0f, 6.0f),
            new Vector3(7.0f, 0.0f, 7.0f)
        },

        {
            new Vector3(-1.0f, 0.0f, -1.0f),
            new Vector3(-2.0f, 0.0f, -2.0f),
            new Vector3(-3.0f, 0.0f, -3.0f),
            new Vector3(-4.0f, 0.0f, -4.0f),
            new Vector3(-5.0f, 0.0f, -5.0f),
            new Vector3(-6.0f, 0.0f, -6.0f),
            new Vector3(-7.0f, 0.0f, -6.0f)
        },

        {
            new Vector3(-1.0f, 0.0f, 1.0f),
            new Vector3(-2.0f, 0.0f, 2.0f),
            new Vector3(-3.0f, 0.0f, 3.0f),
            new Vector3(-4.0f, 0.0f, 4.0f),
            new Vector3(-5.0f, 0.0f, 5.0f),
            new Vector3(-6.0f, 0.0f, 6.0f),
            new Vector3(-7.0f, 0.0f, 7.0f)
        },

        {
            new Vector3(1.0f, 0.0f, -1.0f),
            new Vector3(2.0f, 0.0f, -2.0f),
            new Vector3(3.0f, 0.0f, -3.0f),
            new Vector3(4.0f, 0.0f, -4.0f),
            new Vector3(5.0f, 0.0f, -5.0f),
            new Vector3(6.0f, 0.0f, -6.0f),
            new Vector3(7.0f, 0.0f, -7.0f)
        },

        {
            new Vector3(1.0f, 0.0f, 0.0f),
            new Vector3(2.0f, 0.0f, 0.0f),
            new Vector3(3.0f, 0.0f, 0.0f),
            new Vector3(4.0f, 0.0f, 0.0f),
            new Vector3(5.0f, 0.0f, 0.0f),
            new Vector3(6.0f, 0.0f, 0.0f),
            new Vector3(7.0f, 0.0f, 0.0f)
        },

        {
            new Vector3(-1.0f, 0.0f, 0.0f),
            new Vector3(-2.0f, 0.0f, 0.0f),
            new Vector3(-3.0f, 0.0f, 0.0f),
            new Vector3(-4.0f, 0.0f, 0.0f),
            new Vector3(-5.0f, 0.0f, 0.0f),
            new Vector3(-6.0f, 0.0f, 0.0f),
            new Vector3(-7.0f, 0.0f, 0.0f)
        },

        {
            new Vector3(0.0f, 0.0f, 1.0f),
            new Vector3(0.0f, 0.0f, 2.0f),
            new Vector3(0.0f, 0.0f, 3.0f),
            new Vector3(0.0f, 0.0f, 4.0f),
            new Vector3(0.0f, 0.0f, 5.0f),
            new Vector3(0.0f, 0.0f, 6.0f),
            new Vector3(0.0f, 0.0f, 7.0f)
        },

        {
            new Vector3(0.0f, 0.0f, -1.0f),
            new Vector3(0.0f, 0.0f, -2.0f),
            new Vector3(0.0f, 0.0f, -3.0f),
            new Vector3(0.0f, 0.0f, -4.0f),
            new Vector3(0.0f, 0.0f, -5.0f),
            new Vector3(0.0f, 0.0f, -6.0f),
            new Vector3(0.0f, 0.0f, -7.0f)
        }
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

            for (int legalMoveSet = 0; legalMoveSet < 8; legalMoveSet++) {

                for (int legalMove = 0; legalMove < legalMoves.GetRow(legalMoveSet).Length; legalMove++) {

                     // Check for pieces in the current legal move space
                    Collider[] piecesInSquare = Physics.OverlapSphere(transform.position + legalMoves[legalMoveSet, legalMove], 0.5f);

                    if (piecesInSquare.Length == 1) {  // Show attack token and break the inner loop

                        if (piecesInSquare[0].transform.gameObject.CompareTag(sideNames[oppositeSide])) {  // Only create an attack token if the piece is on the opposing side
                            GameObject attackToken = Instantiate(gameManager.attackToken, transform.position + legalMoves[legalMoveSet, legalMove], new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
                            attackToken.GetComponent<AttackToken>().parentPiece = this;
                            attackToken.GetComponent<AttackToken>().attackedPiece = piecesInSquare[0].transform.gameObject;
                        }

                        break;

                    }

                    if (piecesInSquare.Length == 0) {  // Create a normal move token

                        GameObject moveToken = Instantiate(gameManager.legalMoveToken, transform.position + legalMoves[legalMoveSet, legalMove], new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
                        moveToken.GetComponent<MoveToken>().parentPiece = this;

                    }

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
