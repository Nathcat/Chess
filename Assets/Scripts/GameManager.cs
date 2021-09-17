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

    public float pieceMoveSpeed = 1.0f;  // The speed all pieces should move at
    public GameObject legalMoveToken;  // Legal move token GameObject
    public GameObject attackToken;  // Attack token GameObject
    public int turn = 0;  // Tracks which player's turn it is, 0 for white, 1 for black
    public GameObject[] pieces;  // Array of chess piece GameObjects

    void Start() {
        Physics.queriesHitTriggers = true;  // Makes sure that player clicks will hit trigger colliders

        isThreatened(new Vector3(), 1);
    }

    void Update() {
      isThreatened(new Vector3(0.5f, 0.5f, 3.5f), 0);
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

            if (piece.GetComponent<ChessPiece>().side != enemy) {  // If the current piece is not an enemy, skip it
              continue;
            }

            // Get a list of legal attack vectors for this piece
            object[] legalAttacks = piece.GetComponent<ChessPiece>().getLegalMoves();

            // Check if any of the legal attack vectors match the given position
            foreach (object attack in legalAttacks) {
                Debug.Log("" + (Vector3) attack + position);
                if (((Vector3) attack) == position) {
                  return true;
                }
            }

        }

        return false;

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
