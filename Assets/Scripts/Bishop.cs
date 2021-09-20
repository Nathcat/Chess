using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Bishop.cs
 *
 * ChessPiece script for the Bishop piece.
 *
 * Author: Nathan "Nathcat" Baines
 */


public class Bishop : ChessPiece  // Derive from the ChessPiece parent class
{


    // legalMoves stores a 2-dimensional array of Vector3 objects which represent
    // all the legal moves for this piece.
    // Each nested array contains all the legal moves in a certain direction,
    // this is so that when creating the move tokens, if there is another piece
    // in the way of a certain move, the rest of the moves in that direction
    // should also be impossible, so in that event, the program just skips
    // to the next nested array.
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
        }
    };

    // The names of each side in an array corresponding to the integer representations of each side
    private string[] sideNames = {"White", "Black"};

    void Start() {  // Start is called before the first frame in which this object exists
        // Get the GameManager script from the object that hosts it ("GameManager")
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Determine the integer value of the opposite side
        if (side == 0) {

            oppositeSide = 1;

        } else {

            oppositeSide = 0;

        }
    }

    void OnMouseDown() {  // If this piece is clicked

        // If it's this piece's side's turn, and this piece is not moving...
        if (gameManager.turn == side && !moving) {

            // Destroy all existing move tokens (which will have the tag "LegalMoveToken")
            foreach (GameObject moveToken in GameObject.FindGameObjectsWithTag("LegalMoveToken")) {
                Destroy(moveToken);
            }

            if (gameManager.inCheck[side]) {  // There is a special protocal for moving when the King is in check
              inCheckMove();
              return;
            }

            // Create the legal move tokens in the positions defined in the legalMoves array
            for (int legalMoveSet = 0; legalMoveSet < 4; legalMoveSet++) {  // legalMoves.Length should equal 4, but apparently it equals 28?!?!?!

                for (int legalMove = 0; legalMove < legalMoves.GetRow(legalMoveSet).Length; legalMove++) {

                     // Check for pieces in the current legal move space
                    Collider[] piecesInSquare = Physics.OverlapSphere(transform.position + legalMoves[legalMoveSet, legalMove], 0.5f);

                    if (piecesInSquare.Length == 1) {  // Show attack token and break the inner loop

                        if (piecesInSquare[0].transform.gameObject.CompareTag(sideNames[oppositeSide])) {  // Only create an attack token if the piece is on the opposing side
                            // Instantiate the attack token object
                            GameObject attackToken = Instantiate(gameManager.attackToken, transform.position + legalMoves[legalMoveSet, legalMove], new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
                            // Set the parent piece to this piece
                            attackToken.GetComponent<AttackToken>().parentPiece = this;
                            // Set the attacked piece to the piece in the current space
                            attackToken.GetComponent<AttackToken>().attackedPiece = piecesInSquare[0].transform.gameObject;
                        }

                        break;  // Break the inner loop, so that there are no more move tokens created in this direction

                    }

                    if (piecesInSquare.Length == 0) {  // Create a normal move token

                        // Instantiate the move token object
                        GameObject moveToken = Instantiate(gameManager.legalMoveToken, transform.position + legalMoves[legalMoveSet, legalMove], new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
                        // Set the parent piece to this piece
                        moveToken.GetComponent<MoveToken>().parentPiece = this;

                    }

                }

            }
        }

    }

    override public void move(GameObject legalMoveToken) {  // Move the piece to a given move token

        // Move this piece to the position of the move token
        StartCoroutine(moveOverTime(legalMoveToken.transform.position));

        // Destroy all existing move tokens
        foreach (GameObject moveToken in GameObject.FindGameObjectsWithTag("LegalMoveToken")) {
            Destroy(moveToken);
        }

        gameManager.togglePlayer();  // Move to the opposing sides turn
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

    void inCheckMove() {  // Special protocol for moves when the king is in check
      object[] checkingPieces = gameManager.checkingPieces.ToArray();

      // Check if this piece can take any of the pieces putting the king in check
      for (int x = 0; x < 4; x++) {
        for (int y = 0; y < 7; y++) {

          Vector2 legalMove = new Vector2(legalMoves[x, y].x, legalMoves[x, y].z) + new Vector2(transform.position.x, transform.position.z);

          foreach (GameObject piece in checkingPieces) {
            Vector2 piecePosition = new Vector2(piece.transform.position.x, piece.transform.position.z);

            if (legalMove == piecePosition) {  // If this attack is a valid move
              // Create an attack token
              GameObject attackToken = Instantiate(gameManager.attackToken, legalMoves[x, y] + transform.position, new Quaternion());
              attackToken.GetComponent<AttackToken>().parentPiece = this;  // Set the parent piece to this piece
              attackToken.GetComponent<AttackToken>().attackedPiece = piece;  // Set the attacked piece to the checking piece
            }
          }

        }
      }
    }
}
