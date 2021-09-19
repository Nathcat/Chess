using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Knight.cs
 *
 * ChessPiece script for the Knight piece.
 *
 * Author: Nathan "Nathcat" Baines
 */


public class Knight : ChessPiece  // Derive from ChessPiece parent class
{
    private Vector3[] legalMoves = {  // Array of legal moves for this piece
        new Vector3(1.0f, 0.0f, 2.0f),
        new Vector3(2.0f, 0.0f, 1.0f),
        new Vector3(-1.0f, 0.0f, 2.0f),
        new Vector3(-2.0f, 0.0f, 1.0f),
        new Vector3(1.0f, 0.0f, -2.0f),
        new Vector3(2.0f, 0.0f, -1.0f),
        new Vector3(-1.0f, 0.0f, -2.0f),
        new Vector3(-2.0f, 0.0f, -1.0f)
    };

    private int oppositeSide;
    private string[] sideNames = {"White", "Black"};


    void Start() {  // Start is called before the first frame for which this object exists
        // Get the GameManager script from the host object ("GameManager")
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Determine the opposite side
        if (side == 0) {

            oppositeSide = 1;

        } else {

            oppositeSide = 0;

        }
    }

    void OnMouseDown() {  // Called when this object is clicked

        // If it's this piece's side's turn, and this piece is not moving...
        if (gameManager.turn == side && !moving) {

            // Destroy all existing legal move tokens
            foreach (GameObject moveToken in GameObject.FindGameObjectsWithTag("LegalMoveToken")) {
                Destroy(moveToken);
            }

            // For each legal move Vector in the legal moves array
            foreach (Vector3 legalMove in legalMoves) {

                // Array of colliders found in the space this legal move will move this piece to
                Collider[] piecesInSquare = Physics.OverlapSphere(transform.position + legalMove, 0.5f);

                // If there is an object in the space...
                if (piecesInSquare.Length == 1) {
                    // If the object in the space is on the opposite side
                    if (piecesInSquare[0].transform.gameObject.CompareTag(sideNames[oppositeSide])) {
                        // Create an attack token
                        GameObject attackToken = Instantiate(gameManager.attackToken, transform.position + legalMove, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
                        // Set the parent piece of the attack token to this piece
                        attackToken.GetComponent<AttackToken>().parentPiece = this;
                        // Set the attacked piece to the piece in the square
                        attackToken.GetComponent<AttackToken>().attackedPiece = piecesInSquare[0].transform.gameObject;
                    }
                }

                // If there are no objects in the space
                if (piecesInSquare.Length == 0) {
                    // Create a new move token
                    GameObject moveToken = Instantiate(gameManager.legalMoveToken, transform.position + legalMove, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
                    // Set the parent piece of the move token to this piece
                    moveToken.GetComponent<MoveToken>().parentPiece = this;
                }

            }
        }

    }

    override public void move(GameObject legalMoveToken) {  // Override move method

        // Start the moveOverTime coroutine to move the piece over time to its new location
        StartCoroutine(moveOverTime(legalMoveToken.transform.position));

        // Destroy all existing move tokens
        foreach (GameObject moveToken in GameObject.FindGameObjectsWithTag("LegalMoveToken")) {
            Destroy(moveToken);
        }

        gameManager.togglePlayer();  // Move to the next player's turn
    }

    override public object[] getLegalMoves() {  // Get a list of the legal moves for this piece
        ArrayList legalMoveList = new ArrayList();

        foreach (Vector3 legalMove in legalMoves) {  // For all the legal moves this piece has

            // Get a list of colliders in the space this legal move would take this piece to
            Collider[] piecesInSquare = Physics.OverlapSphere(transform.position + legalMove, 0.5f);

            // Add to the list
            legalMoveList.Add(transform.position + legalMove);

        }

        return legalMoveList.ToArray();
    }

    public IEnumerator moveOverTime(Vector3 endPosition) {  // Move to a new position over time

        moving = true;  // This piece is moving

        while (transform.position != endPosition) {  // While this piece's current position is not equal to the given end position
            // If this is a white piece, move this piece up the board, if it's a black piece, move it down the board
            if (side == 0) { transform.position += (endPosition - transform.position) * gameManager.pieceMoveSpeed; } else { transform.position += (transform.position - endPosition) * -gameManager.pieceMoveSpeed; }

            // If this piece is within 0.1 of it's destination, move it to its destination
            Vector3 distance = endPosition - transform.position;
            float distanceMagnitude = sqrt((distance.x * distance.x) + (distance.z * distance.z));

            if (distanceMagnitude <= 0.1f && distanceMagnitude >= -0.1f) {
                transform.position = endPosition;
            }

            yield return new WaitForSeconds(0.01f);  // Wait for 0.01 seconds
        }

        moving = false;  // The piece has finished moving

    }

    override public void resetLegalMoves() {  // Reset this piece's legal moves array
      legalMoves = new Vector3[] {  // Array of legal moves for this piece
          new Vector3(1.0f, 0.0f, 2.0f),
          new Vector3(2.0f, 0.0f, 1.0f),
          new Vector3(-1.0f, 0.0f, 2.0f),
          new Vector3(-2.0f, 0.0f, 1.0f),
          new Vector3(1.0f, 0.0f, -2.0f),
          new Vector3(2.0f, 0.0f, -1.0f),
          new Vector3(-1.0f, 0.0f, -2.0f),
          new Vector3(-2.0f, 0.0f, -1.0f)
      };
    }
}
