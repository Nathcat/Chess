using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.InteropServices;
using System;


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

    void Start() {
        Physics.queriesHitTriggers = true;  // Makes sure that player clicks will hit trigger colliders
    }

    void Update() {
      if (shouldShowThreats) {  // If the shouldShowThreats setting is true
        showThreats();  // Show all pieces under threat from the opponent
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
            object[] legalAttacks = piece.GetComponent<ChessPiece>().getLegalMoves();

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
