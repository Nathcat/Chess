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

    private int oppositeSide;
    private string[] sideNames = {"White", "Black"};


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

    override public object[] getLegalMoves() {  // Return a list of legal moves for this piece
      ArrayList legalMoveList = new ArrayList();

      for (int legalMoveSet = 0; legalMoveSet < 4; legalMoveSet++) {  // legalMoves.Length should equal 4, but apparently it equals 28?!?!?!

          for (int legalMove = 0; legalMove < legalMoves.GetRow(legalMoveSet).Length; legalMove++) {

               // Check for pieces in the current legal move space
              Collider[] piecesInSquare = Physics.OverlapSphere(transform.position + legalMoves[legalMoveSet, legalMove], 0.5f);

              if (piecesInSquare.Length == 1) {  // Add to list and break the inner loop

                  // Add this move to the list of legal moves
                  legalMoveList.Add(transform.position + legalMoves[legalMoveSet, legalMove]);

                  break;  // Break the inner loop, so that there are no more move tokens created in this direction

              }

              if (piecesInSquare.Length == 0) {  // Add to list

                  // Add this move to the list of legal moves
                  legalMoveList.Add(transform.position + legalMoves[legalMoveSet, legalMove]);

              }

          }

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
}
