using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.InteropServices;
using System;
using UnityEngine.UI;


/*
 * GameManager.cs
 *
 * Game manager script, will be attached to a GameManager object.
 * Tracks various bits of data that allows the game to run smoothly.
 *
 * Author: Nathan "Nathcat" Baines
 */


public class GameManager : MonoBehaviour
{

    public Material red;  // Red material
    public Material[] pieceMaterials;  // The materials for the pieces on each side
    public bool shouldShowThreats;  // Should pieces under threat of attack be highlighted
    public float pieceMoveSpeed = 1.0f;  // The speed all pieces should move at
    public GameObject legalMoveToken;  // Legal move token GameObject
    public GameObject attackToken;  // Attack token GameObject
    public int turn = 0;  // Tracks which player's turn it is, 0 for white, 1 for black
    public GameObject[] pieces;  // Array of chess piece GameObjects
    public bool[] inCheck = {false, false};  // Array to show if each side is in check
    public GameObject checkText;  // UI text to tell the player they are in check
    public GameObject tempCollider;  // Object with a single collider component that is used for checking moves

    void Start() {
        Physics.queriesHitTriggers = true;  // Makes sure that player clicks will hit trigger colliders
    }

    void Update() {
      if (shouldShowThreats) {  // If the shouldShowThreats setting is true
        showThreats();  // Show all pieces under threat from the opponent
      }

      // Check if the either player is in check
      GameObject whiteKing = null;
      GameObject blackKing = null;

      foreach (GameObject piece in pieces) {
        if (piece == null) {
          continue;
        }

        if (piece.name == "King_white") {
          whiteKing = piece;
        }

        if (piece.name == "King_black") {
          blackKing = piece;
        }
      }

      inCheck[0] = isThreatened(whiteKing.transform.position, 1);
      inCheck[1] = isThreatened(blackKing.transform.position, 0);

      if (inCheck[turn]) {
        checkText.GetComponent<Text>().text = "Check!";
        checkText.SetActive(true);
      } else {
        checkText.SetActive(false);
      }
    }

    public void togglePlayer() {  // Move to the next player's turn
        if (turn == 0) {  // If it's currently white's turn, make it black's turn
            turn = 1;
        } else {  // Else, make it white's turn
            turn = 0;
        }
    }

    public bool isThreatened(Vector3 position, int enemy) {  // Check if a position is threatened by a piece

        foreach (GameObject piece in pieces) {  // Iterate for all pieces in the array

            if (piece == null) {  // If this piece has been captured (the GameObject is destroyed)
              continue;
            }

            if (piece.GetComponent<ChessPiece>().side != enemy) {  // If the current piece is not an enemy, skip it
              continue;
            }

            // Get a list of legal attack vectors for this piece
            object[] legalAttacks = piece.GetComponent<ChessPiece>().getLegalAttacks();

            // Check if any of the legal attack vectors match the given position
            foreach (object attack in legalAttacks) {
                Vector3 attackVector = (Vector3) attack;

                if ((attackVector.x == position.x) && (attackVector.z == position.z)) {
                  return true;
                }
            }

        }

        return false;

    }

    private void showThreats() {  // Show the threats to the current player's pieces
        // Iterate for all pieces
        foreach (GameObject piece in pieces) {
            // If the current piece does not exist, skip over it
            if (piece == null) { continue; }

            // If the current piece is the current player's piece
            if (piece.GetComponent<ChessPiece>().side == turn) {
                // If the current piece is threatened
                int oppositeSide;
                if (turn == 0) { oppositeSide = 1; } else { oppositeSide = 0; }

                if (isThreatened(piece.transform.position, oppositeSide)) {
                    // Change the colour of the piece to red
                    piece.GetComponent<MeshRenderer>().material = red;
                } else {
                    piece.GetComponent<MeshRenderer>().material = pieceMaterials[turn];
                }

            } else {
              piece.GetComponent<MeshRenderer>().material = pieceMaterials[piece.GetComponent<ChessPiece>().side];
            }
        }
    }

    public void updatePieceMovesForCheck() {  // In the case of a check, update the legal moves of pieces to only include moves that are legal in terms of check
      ArrayList legalMovePositions_ = new ArrayList();  // ArrayList containing all the moves the current player can make that are legal

      // Determine the current player's king
      GameObject kingPiece;
      foreach (GameObject piece in pieces) {
        if (turn == 0) {

          if (piece.name == "King_white") {
            kingPiece = piece;
          }

        } else {

          if (piece.name == "King_black") {
            kingPiece = piece;
          }

        }
      }

      foreach (GameObject piece in pieces) {  // Iterate for each piece in the array
        // Check that the piece exists, if not, skip it
        if (piece == null) {
          continue;
        }

        // Check that the piece is on the current player's side, if not, skip it
        if (piece.GetComponent<ChessPiece>().side != turn) {
          continue;
        }

        // Iterate for all of the current piece's legal moves
        foreach (Vector3 legalMove in piece.GetComponent<ChessPiece>().getLegalMoves()) {
          GameObject tempCollider_ = Instantiate(tempCollider, piece.transform.position + legalMove, new Quaternion());

          if (!isThreatened(kingPiece.transform.position, piece.GetComponent<ChessPiece>().oppositeSide)) {
            legalMovePositions_.Add(piece.transform.position + legalMove);
          }

          Destroy(tempCollider_);
        }
      }

      // Now we have a list of moves that can be made to get out of check, we must update each piece's legal
      // moves array to reflect these valid moves.
      // TODO
    }

}

public static class ArrayExt  // Array extension class to easily get rows and columns from a 2d array.
{
    public static T[] GetRow<T>(this T[,] matrix, int rowNumber)
    {
        return Enumerable.Range(0, matrix.GetLength(1))
                .Select(x => matrix[rowNumber, x])
                .ToArray();
    }

    public static T[] GetColumn<T>(this T[,] matrix, int columnNumber)
    {
        return Enumerable.Range(0, matrix.GetLength(0))
                .Select(x => matrix[x, columnNumber])
                .ToArray();
    }
}
