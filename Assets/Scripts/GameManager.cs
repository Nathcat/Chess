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
    }

    public void togglePlayer() {  // Move to the next player's turn
        if (turn == 0) {  // If it's currently white's turn, make it black's turn
            turn = 1;
        } else {  // Else, make it white's turn
            turn = 0;
        }
    }

    // TODO: Yeah just redo this entire method, it looks awful and probably doesn't work lol.
    public bool isThreatened(Vector3 checkSpace, GameObject checkingPiece) {  // Check if a piece is threatened

        ArrayList threats = new ArrayList();

        foreach(GameObject piece in pieces) {

            if (piece.name == checkingPiece.name) {
                continue;
            }

            // Work out which spaces the current piece is threatening

            // TODO: Redo the checking algorithms, I should just be able to get a list of legal
            // move from the chess piece's script, then I wouldn't have to make it specific to
            // each piece.
            if (piece.name.Contains("Pawn")) {

                ArrayList tempThreats = new ArrayList();

                Vector3[] legalThreats = {};

                if (piece.CompareTag("White")) {

                    legalThreats = new Vector3[] {
                        new Vector3(1.0f, 0.0f, 1.0f),
                        new Vector3(-1.0f, 0.0f, 1.0f)
                    };

                } else {

                    legalThreats = new Vector3[] {
                        new Vector3(1.0f, 0.0f, -1.0f),
                        new Vector3(-1.0f, 0.0f, -1.0f)
                    };

                }

                foreach (Vector3 legalThreat in legalThreats) {
                    if (Physics.OverlapSphere(piece.transform.position + legalThreat, 0.5f).Length == 0) {
                        tempThreats.Add(legalThreat);
                    }
                }

                foreach (Vector3 tempThreat in tempThreats) {
                    threats.Add(tempThreat);
                }

            }

            if (piece.name.Contains("Rook")) {

                ArrayList tempThreats = new ArrayList();

                Vector3[] legalMoves = {

                    new Vector3(1.0f, 0.0f, 0.0f),
                    new Vector3(2.0f, 0.0f, 0.0f),
                    new Vector3(3.0f, 0.0f, 0.0f),
                    new Vector3(4.0f, 0.0f, 0.0f),
                    new Vector3(5.0f, 0.0f, 0.0f),
                    new Vector3(6.0f, 0.0f, 0.0f),
                    new Vector3(7.0f, 0.0f, 0.0f),

                    new Vector3(-1.0f, 0.0f, 0.0f),
                    new Vector3(-2.0f, 0.0f, 0.0f),
                    new Vector3(-3.0f, 0.0f, 0.0f),
                    new Vector3(-4.0f, 0.0f, 0.0f),
                    new Vector3(-5.0f, 0.0f, 0.0f),
                    new Vector3(-6.0f, 0.0f, 0.0f),
                    new Vector3(-7.0f, 0.0f, 0.0f),

                    new Vector3(0.0f, 0.0f, 1.0f),
                    new Vector3(0.0f, 0.0f, 2.0f),
                    new Vector3(0.0f, 0.0f, 3.0f),
                    new Vector3(0.0f, 0.0f, 4.0f),
                    new Vector3(0.0f, 0.0f, 5.0f),
                    new Vector3(0.0f, 0.0f, 6.0f),
                    new Vector3(0.0f, 0.0f, 7.0f),

                    new Vector3(0.0f, 0.0f, -1.0f),
                    new Vector3(0.0f, 0.0f, -2.0f),
                    new Vector3(0.0f, 0.0f, -3.0f),
                    new Vector3(0.0f, 0.0f, -4.0f),
                    new Vector3(0.0f, 0.0f, -5.0f),
                    new Vector3(0.0f, 0.0f, -6.0f),
                    new Vector3(0.0f, 0.0f, -7.0f)
                };

                foreach (Vector3 legalMove in legalMoves) {
                    if (Physics.OverlapSphere(piece.transform.position + legalMove, 0.5f).Length == 0) {
                        tempThreats.Add(legalMove);
                    }
                }

                foreach (Vector3 tempThreat in tempThreats) {
                    threats.Add(tempThreat);
                }

            }

            if (piece.name.Contains("Bishop")) {

                ArrayList tempThreats = new ArrayList();

                Vector3[] legalMoves = {
                    new Vector3(1.0f, 0.0f, 1.0f),
                    new Vector3(2.0f, 0.0f, 2.0f),
                    new Vector3(3.0f, 0.0f, 3.0f),
                    new Vector3(4.0f, 0.0f, 4.0f),
                    new Vector3(5.0f, 0.0f, 5.0f),
                    new Vector3(6.0f, 0.0f, 6.0f),
                    new Vector3(7.0f, 0.0f, 7.0f),

                    new Vector3(-1.0f, 0.0f, -1.0f),
                    new Vector3(-2.0f, 0.0f, -2.0f),
                    new Vector3(-3.0f, 0.0f, -3.0f),
                    new Vector3(-4.0f, 0.0f, -4.0f),
                    new Vector3(-5.0f, 0.0f, -5.0f),
                    new Vector3(-6.0f, 0.0f, -6.0f),
                    new Vector3(-7.0f, 0.0f, -6.0f),

                    new Vector3(-1.0f, 0.0f, 1.0f),
                    new Vector3(-2.0f, 0.0f, 2.0f),
                    new Vector3(-3.0f, 0.0f, 3.0f),
                    new Vector3(-4.0f, 0.0f, 4.0f),
                    new Vector3(-5.0f, 0.0f, 5.0f),
                    new Vector3(-6.0f, 0.0f, 6.0f),
                    new Vector3(-7.0f, 0.0f, 7.0f),

                    new Vector3(1.0f, 0.0f, -1.0f),
                    new Vector3(2.0f, 0.0f, -2.0f),
                    new Vector3(3.0f, 0.0f, -3.0f),
                    new Vector3(4.0f, 0.0f, -4.0f),
                    new Vector3(5.0f, 0.0f, -5.0f),
                    new Vector3(6.0f, 0.0f, -6.0f),
                    new Vector3(7.0f, 0.0f, -7.0f)

                };

                foreach (Vector3 legalMove in legalMoves) {
                    if (Physics.OverlapSphere(piece.transform.position + legalMove, 0.5f).Length == 0) {
                        tempThreats.Add(legalMove);
                    }
                }

                foreach (Vector3 tempThreat in tempThreats) {
                    threats.Add(tempThreat);
                }

            }

            if (piece.name.Contains("Queen")) {

                ArrayList tempThreats = new ArrayList();

                Vector3[] legalMoves = {
                    new Vector3(1.0f, 0.0f, 1.0f),
                    new Vector3(2.0f, 0.0f, 2.0f),
                    new Vector3(3.0f, 0.0f, 3.0f),
                    new Vector3(4.0f, 0.0f, 4.0f),
                    new Vector3(5.0f, 0.0f, 5.0f),
                    new Vector3(6.0f, 0.0f, 6.0f),
                    new Vector3(7.0f, 0.0f, 7.0f),

                    new Vector3(-1.0f, 0.0f, -1.0f),
                    new Vector3(-2.0f, 0.0f, -2.0f),
                    new Vector3(-3.0f, 0.0f, -3.0f),
                    new Vector3(-4.0f, 0.0f, -4.0f),
                    new Vector3(-5.0f, 0.0f, -5.0f),
                    new Vector3(-6.0f, 0.0f, -6.0f),
                    new Vector3(-7.0f, 0.0f, -6.0f),

                    new Vector3(-1.0f, 0.0f, 1.0f),
                    new Vector3(-2.0f, 0.0f, 2.0f),
                    new Vector3(-3.0f, 0.0f, 3.0f),
                    new Vector3(-4.0f, 0.0f, 4.0f),
                    new Vector3(-5.0f, 0.0f, 5.0f),
                    new Vector3(-6.0f, 0.0f, 6.0f),
                    new Vector3(-7.0f, 0.0f, 7.0f),

                    new Vector3(1.0f, 0.0f, -1.0f),
                    new Vector3(2.0f, 0.0f, -2.0f),
                    new Vector3(3.0f, 0.0f, -3.0f),
                    new Vector3(4.0f, 0.0f, -4.0f),
                    new Vector3(5.0f, 0.0f, -5.0f),
                    new Vector3(6.0f, 0.0f, -6.0f),
                    new Vector3(7.0f, 0.0f, -7.0f),

                    new Vector3(1.0f, 0.0f, 0.0f),
                    new Vector3(2.0f, 0.0f, 0.0f),
                    new Vector3(3.0f, 0.0f, 0.0f),
                    new Vector3(4.0f, 0.0f, 0.0f),
                    new Vector3(5.0f, 0.0f, 0.0f),
                    new Vector3(6.0f, 0.0f, 0.0f),
                    new Vector3(7.0f, 0.0f, 0.0f),

                    new Vector3(-1.0f, 0.0f, 0.0f),
                    new Vector3(-2.0f, 0.0f, 0.0f),
                    new Vector3(-3.0f, 0.0f, 0.0f),
                    new Vector3(-4.0f, 0.0f, 0.0f),
                    new Vector3(-5.0f, 0.0f, 0.0f),
                    new Vector3(-6.0f, 0.0f, 0.0f),
                    new Vector3(-7.0f, 0.0f, 0.0f),

                    new Vector3(0.0f, 0.0f, 1.0f),
                    new Vector3(0.0f, 0.0f, 2.0f),
                    new Vector3(0.0f, 0.0f, 3.0f),
                    new Vector3(0.0f, 0.0f, 4.0f),
                    new Vector3(0.0f, 0.0f, 5.0f),
                    new Vector3(0.0f, 0.0f, 6.0f),
                    new Vector3(0.0f, 0.0f, 7.0f),

                    new Vector3(0.0f, 0.0f, -1.0f),
                    new Vector3(0.0f, 0.0f, -2.0f),
                    new Vector3(0.0f, 0.0f, -3.0f),
                    new Vector3(0.0f, 0.0f, -4.0f),
                    new Vector3(0.0f, 0.0f, -5.0f),
                    new Vector3(0.0f, 0.0f, -6.0f),
                    new Vector3(0.0f, 0.0f, -7.0f)

                };

                foreach (Vector3 legalMove in legalMoves) {
                    if (Physics.OverlapSphere(piece.transform.position + legalMove, 0.5f).Length == 0) {
                        tempThreats.Add(legalMove);
                    }
                }

                foreach (Vector3 tempThreat in tempThreats) {
                    threats.Add(tempThreat);
                }

            }

            if (piece.name.Contains("King")) {

                ArrayList tempThreats = new ArrayList();

                Vector3[] legalMoves = {

                    new Vector3(-1.0f, 0.0f, -1.0f),
                    new Vector3(0.0f, 0.0f, -1.0f),
                    new Vector3(1.0f, 0.0f, -1.0f),
                    new Vector3(1.0f, 0.0f, 0.0f),
                    new Vector3(1.0f, 0.0f, 1.0f),
                    new Vector3(0.0f, 0.0f, 1.0f),
                    new Vector3(-1.0f, 0.0f, 1.0f),
                    new Vector3(-1.0f, 0.0f, 0.0f)

                };

                foreach (Vector3 legalMove in legalMoves) {
                    if (Physics.OverlapSphere(piece.transform.position + legalMove, 0.5f).Length == 0) {
                        tempThreats.Add(legalMove);
                    }
                }

                foreach (Vector3 tempThreat in tempThreats) {
                    threats.Add(tempThreat);
                }

            }

            if (piece.name.Contains("Knight")) {

                ArrayList tempThreats = new ArrayList();

                Vector3[] legalMoves = {

                    new Vector3(1.0f, 0.0f, 2.0f),
                    new Vector3(2.0f, 0.0f, 1.0f),
                    new Vector3(-1.0f, 0.0f, 2.0f),
                    new Vector3(-2.0f, 0.0f, 1.0f),
                    new Vector3(1.0f, 0.0f, -2.0f),
                    new Vector3(2.0f, 0.0f, -1.0f),
                    new Vector3(-1.0f, 0.0f, -2.0f),
                    new Vector3(-2.0f, 0.0f, -1.0f)

                };

                foreach (Vector3 legalMove in legalMoves) {
                    if (Physics.OverlapSphere(piece.transform.position + legalMove, 0.5f).Length == 0) {
                        tempThreats.Add(legalMove);
                    }
                }

                foreach (Vector3 tempThreat in tempThreats) {
                    threats.Add(tempThreat);
                }

            }

        }

        for (int x = 0; x < threats.Count; x++) {  // Iterate through the list of threats
            if (threats.Contains(checkSpace)) {  // If checkSpace in in the check space, checkSpace is threatened
                return true;
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
