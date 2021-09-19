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
    public Vector3[] attackSquares;  // The squares that can be attacked, only used on the Pawn.cs script
    public Vector3[] legalMoves;  // Array of legal moves for this piece
    private int oppositeSide;
    private string[] sideNames = {"White", "Black"};


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

    override public object[] getLegalMoves() {  // Get a list of this piece's legal moves
        ArrayList legalMoveList = new ArrayList();

        foreach (Vector3 legalMove in legalMoves) {
          legalMoveList.Add(transform.position + legalMove);
        }

        return legalMoveList.ToArray();
    }

    override public object[] getLegalAttacks() {  // Get a list of legal attacks
      ArrayList legalAttacksList = new ArrayList();

      foreach (Vector3 legalAttack in attackSquares) {
        // Check if there is a piece in the legal attack space
        Collider[] piecesInSquare = Physics.OverlapSphere(transform.position + legalAttack, 0.5f);

        if (piecesInSquare.Length == 1) {  // If there is a piece in the space
          if (piecesInSquare[0].transform.gameObject.CompareTag(sideNames[oppositeSide])) {  // If the piece is on the opposite side
            legalAttacksList.Add(transform.position + legalAttack);
          }
        }
      }

      return legalAttacksList.ToArray();
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
