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
    public ArrayList checkingPieces = new ArrayList();  // ArrayList containing all the pieces putting the king in check
    public GameObject turnIndicator;  // UI image indicating which player's turn it is
    public Sprite[] turnImages;  // Array of images representing each side (white and black)
    public bool playing = true;  // Defines whether a game is currently in process or not
    private int lastFrameTurn = 0;


    void Start() {
        Physics.queriesHitTriggers = true;  // Makes sure that player clicks will hit trigger colliders
    }

    void Update() {

      turnIndicator.GetComponent<Image>().sprite = turnImages[turn];  // Show which player's turn it is via the image

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

      inCheck[0] = (bool) isThreatened(whiteKing.transform.position, 1)[0];
      inCheck[1] = (bool) isThreatened(blackKing.transform.position, 0)[0];

      if (inCheck[turn]) {
        // This will only evaluate to true when a player is in checkmate,
        // so, if a player is in checkmate, exit the method.
        if (checkText.GetComponent<Text>().text != "Check!") {
          return;
        }

        checkText.GetComponent<Text>().text = "Check!";
        checkText.SetActive(true);
        if (turn == 0) {
          object[] threatReport = isThreatened(whiteKing.transform.position, 1);
          checkingPieces = new ArrayList();

          for (int x = 1; x < threatReport.Length; x++) {
            checkingPieces.Add(threatReport[x]);
          }
        } else if (turn == 1) {
          object[] threatReport = isThreatened(blackKing.transform.position, 0);
          checkingPieces = new ArrayList();

          for (int x = 1; x < threatReport.Length; x++) {
            checkingPieces.Add(threatReport[x]);
          }
        }
      } else {
        checkText.SetActive(false);
      }

      if (inCheck[turn] && turn != lastFrameTurn) {  // If the current player is in check, check if they are in checkmate
        checkForCheckmate();
      }

      lastFrameTurn = turn;
    }

    public void togglePlayer() {  // Move to the next player's turn
        if (turn == 0) {  // If it's currently white's turn, make it black's turn
            turn = 1;
        } else {  // Else, make it white's turn
            turn = 0;
        }
    }

    public object[] isThreatened(Vector3 position, int enemy, bool friendlyFire=false) {  // Check if a position is threatened by a piece

        ArrayList threatReport = new ArrayList();
        threatReport.Add(false);

        foreach (GameObject piece in pieces) {  // Iterate for all pieces in the array

            if (piece == null) {  // If this piece has been captured (the GameObject is destroyed)
              continue;
            }

            if (piece.GetComponent<ChessPiece>().side != enemy) {  // If the current piece is not an enemy, skip it
              continue;
            }

            // Get a list of legal attack vectors for this piece
            object[] legalAttacks = piece.GetComponent<ChessPiece>().getLegalAttacks(friendlyFire);

            // Check if any of the legal attack vectors match the given position
            foreach (object attack in legalAttacks) {
                Vector3 attackVector = (Vector3) attack;

                if ((attackVector.x == position.x) && (attackVector.z == position.z)) {
                  if (((bool) threatReport[0]) != true) {
                    threatReport[0] = true;
                    threatReport.Add(piece);
                  }
                }
            }

        }

        return threatReport.ToArray();

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

                if (((bool) isThreatened(piece.transform.position, oppositeSide)[0])) {
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

    public bool validCheckBlock(Vector3 movePosition, int side) {  // Check if a move to block check is valid
      // Get the king that is in check
      GameObject kingPiece = null;
      foreach (GameObject piece in pieces) {
        if (piece == null) {
          continue;
        }

        if (piece.GetComponent<ChessPiece>().side == side && piece.name.Contains("King")) {
          kingPiece = piece;
        }
      }

      // Create a temporary collider object at movePosition
      GameObject tempCollider_ = Instantiate(tempCollider, movePosition, new Quaternion());

      int oppositeSide = -1;
      if (side == 0) {
        oppositeSide = 1;
      } else {
        oppositeSide = 0;
      }

      // Check if the king is still in check
      bool isInCheck = (bool) isThreatened(kingPiece.transform.position, oppositeSide)[0];

      // Destroy the temporary collider
      Destroy(tempCollider_);

      // Return !isInCheck, ie, if the king is still in check, this is an invalid move (return false)
      return !isInCheck;
    }

    public void checkForCheckmate() {  // Check if the current player is in checkmate
      int totalValidMoves = 0;

      foreach (GameObject piece in pieces) {
        // Destroy all existing move tokens
        foreach (GameObject moveToken in GameObject.FindGameObjectsWithTag("LegalMoveToken")) {
            Destroy(moveToken);
        }

        // Make sure that the current piece exists, if not, skip it
        if (piece == null) {
          continue;
        }

        // Make sure that the current piece is on this player's side, if not, skip it
        if (piece.GetComponent<ChessPiece>().side != turn) {
          continue;
        }

        // Execute the inCheckMove function for this piece
        object[] moves = piece.GetComponent<ChessPiece>().inCheckMove();

        // Add the number of legal move tokens to the total
        totalValidMoves += moves.Length;
      }

      // If there are no valid moves, this player is in checkmate
      if (totalValidMoves == 0) {
        string playerColour = "";
        if (turn == 0) {
          playerColour = "White";
        } else {
          playerColour = "Black";
        }

        checkText.GetComponent<Text>().text = playerColour + " is in checkmate!";
        checkText.SetActive(true);
        playing = false;
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
