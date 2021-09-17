using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Rook.cs
 *
 * ChessPiece script for the Rook piece.
 *
 * Author: Nathan "Nathcat" Baines
 */


public class Rook : ChessPiece  // Derive from ChessPiece parent class
{
    private GameManager gameManager;  // GameManager script
    public Vector3[,] legalMoves = {  // Nested array of legal moves, see Bishop.cs for explanation behind the 2d array
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

    public int side;  // Integer value represnting the side this piece is on, 0 for white, 1 for black
    private int oppositeSide;
    private string[] sideNames = {"White", "Black"};

    public bool moving = false;

    void Start() {  // Start is called before the first frame in which this object is present
        // Get the GameManager script from the object that host it
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (side == 0) {  // Determine the integer value of the opposite side

            oppositeSide = 1;

        } else {

            oppositeSide = 0;

        }
    }

    void OnMouseDown() {  // OnMouseDown is called when this object's collider is clicked

        // If it's this piece's side's turn and this piece is not moving
        if (gameManager.turn == side && !moving) {

            // Destroy all existing move tokens
            foreach (GameObject moveToken in GameObject.FindGameObjectsWithTag("LegalMoveToken")) {
                Destroy(moveToken);
            }

            for (int legalMoveSet = 0; legalMoveSet < 4; legalMoveSet++) {  // legalMoves.Length should equal 4, but apparently it equals 28?!?!?!

                for (int legalMove = 0; legalMove < legalMoves.GetRow(legalMoveSet).Length; legalMove++) {

                     // Check for pieces in the current legal move space
                    Collider[] piecesInSquare = Physics.OverlapSphere(transform.position + legalMoves[legalMoveSet, legalMove], 0.5f);

                    if (piecesInSquare.Length == 1) {  // Show attack token and break the inner loop if there is a piece in the space

                        if (piecesInSquare[0].transform.gameObject.CompareTag(sideNames[oppositeSide])) {  // Only create an attack token if the piece is on the opposing side
                            // Instantiate an attack token
                            GameObject attackToken = Instantiate(gameManager.attackToken, transform.position + legalMoves[legalMoveSet, legalMove], new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
                            // Set the parent piece of the attack token to this piece
                            attackToken.GetComponent<AttackToken>().parentPiece = this;
                            // Set the attack piece to the piece in the space
                            attackToken.GetComponent<AttackToken>().attackedPiece = piecesInSquare[0].transform.gameObject;
                        }

                        // Break the inner loop so that no more move tokens are created in this direction
                        break;

                    }

                    if (piecesInSquare.Length == 0) {  // Create a normal move token if there are no pieces in the space

                        // Instantiate a move token
                        GameObject moveToken = Instantiate(gameManager.legalMoveToken, transform.position + legalMoves[legalMoveSet, legalMove], new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
                        // Set the parent piece of the move token to this piece
                        moveToken.GetComponent<MoveToken>().parentPiece = this;

                    }

                }

            }
        }

    }

    override public void move(GameObject legalMoveToken) {  // Override move method

        // Start coroutine to move this piece over time to it's new location
        StartCoroutine(moveOverTime(legalMoveToken.transform.position));

        // Destroy all existing move tokens
        foreach (GameObject moveToken in GameObject.FindGameObjectsWithTag("LegalMoveToken")) {
            Destroy(moveToken);
        }

        gameManager.togglePlayer();  // Move to the next player's turn
    }
}
