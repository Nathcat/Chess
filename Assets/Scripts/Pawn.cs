using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Pawn.cs
 *
 * ChessPiece script for the Pawn piece.
 * The Pawn is a bit annoying, because it moves and attacks in different
 * directions depending on which side it's on.
 *
 * Author: Nathan "Nathcat" Baines
 */


public class Pawn : ChessPiece  // Derive from ChessPiece parent class
{
    private GameManager gameManager;  // Holds the GameManager script
    private Vector3[] legalMoves;  // Array to hold the legal moves for this piece
    public Vector3[] attackSquares;  // Array to hold the spaces this piece can attack

    public int side;
    private int oppositeSide;
    private string[] sideNames = {"White", "Black"};

    public bool moving = false;

    void Start() {  // Start is called before the first frame in which this object is present
        // Get the GameManager script from the object that hosts it ("GameManager")
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (side == 0) {
            // If this is a white piece, moves should go up the board
            attackSquares = new Vector3[] {
                new Vector3(1.0f, 0.0f, 1.0f),
                new Vector3(-1.0f, 0.0f, 1.0f)
            };

            legalMoves = new Vector3[] {
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 0.0f, 2.0f)
            };

            oppositeSide = 1;

        } else {
            // Else they should go down the board
            attackSquares = new Vector3[] {
                new Vector3(1.0f, 0.0f, -1.0f),
                new Vector3(-1.0f, 0.0f, -1.0f)
            };

            legalMoves = new Vector3[] {
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -2.0f)
            };

            oppositeSide = 0;

        }
    }

    void OnMouseDown() {  // Called when this object is clicked

        // If its this piece's side's turn, and this piece is not moving...
        if (gameManager.turn == side && !moving) {
            // Destroy all existing move tokens
            foreach (GameObject moveToken in GameObject.FindGameObjectsWithTag("LegalMoveToken")) {
                Destroy(moveToken);
            }

            foreach (Vector3 legalMove in legalMoves) {
                // Check for a piece in the current legal move being checked
                Collider[] piecesInWay = Physics.OverlapSphere(transform.position + legalMove, 0.5f);

                if (piecesInWay.Length == 0) {  // If there are no pieces in the legal move space
                    // Create a move token
                    GameObject moveToken = Instantiate(gameManager.legalMoveToken, transform.position + legalMove, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
                    // Set the parent piece of the move token to this piece
                    moveToken.GetComponent<MoveToken>().parentPiece = this;
                }
            }

            foreach (Vector3 attackSquare in attackSquares) {
                // Check the attack squares for pieces
                Collider[] attackedPieceCollider = Physics.OverlapSphere(transform.position + attackSquare, 0.5f);

                // If there is a piece in the current attack square
                if (attackedPieceCollider.Length == 1) {
                    // If the piece in the attack square is on the opposite side
                    if (attackedPieceCollider[0].transform.gameObject.CompareTag(sideNames[oppositeSide])) {
                        // Create an attack token
                        GameObject attackToken = Instantiate(gameManager.attackToken, transform.position + attackSquare, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
                        // Set the parent piece to this piece
                        attackToken.GetComponent<AttackToken>().parentPiece = this;
                        // Set the attacked piece to the piece in the square
                        attackToken.GetComponent<AttackToken>().attackedPiece = attackedPieceCollider[0].transform.gameObject;
                    }
                }
            }
        }

    }

    override public void move(GameObject legalMoveToken) {  // Override move method

        // Start a coroutine to move this piece over time to the new position
        StartCoroutine(moveOverTime(legalMoveToken.transform.position));

        // Destroy all existing move tokens
        foreach (GameObject moveToken in GameObject.FindGameObjectsWithTag("LegalMoveToken")) {
            Destroy(moveToken);
        }

        // Pawns can move two spaces on their first move, so set the legal moves array to only have one move in them
        // because pawns cannot move two spaces after their first move.
        if (side == 0) { legalMoves = new Vector3[] { new Vector3(0.0f, 0.0f, 1.0f) }; } else { legalMoves = new Vector3[] { new Vector3(0.0f, 0.0f, -1.0f) }; }

        gameManager.togglePlayer();  // Move to the next player's turn
    }
}
