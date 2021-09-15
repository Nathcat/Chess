﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * King.cs
 *
 * ChessPiece script for the King piece.
 *
 * Author: Nathan "Nathcat" Baines
 */


public class King : ChessPiece  // Derive from ChessPiece parent class
{
    private GameManager gameManager;  // Holds the GameManager script
    private Vector3[] legalMoves = {  // List of legal moves for this piece
        new Vector3(-1.0f, 0.0f, -1.0f),
        new Vector3(0.0f, 0.0f, -1.0f),
        new Vector3(1.0f, 0.0f, -1.0f),
        new Vector3(1.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 0.0f, 1.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(-1.0f, 0.0f, 1.0f),
        new Vector3(-1.0f, 0.0f, 0.0f)
    };

    public int side;  // Define which side this piece is on, 0 for white, 1 for black
    private int oppositeSide;
    // Array of side names which corresponds to the integer value of each side
    private string[] sideNames = {"White", "Black"};

    public bool moving = false;  // Bool which defines if this piece is moving

    void Start() {  // Start is called before the first frame in which this object exists
        // Get the GameManager script from the object that hosts it ("GameManager")
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (side == 0) {  // If this piece is white, the opposite side is the black side

            oppositeSide = 1;

        } else {  // Else the opposite side is the white side

            oppositeSide = 0;

        }
    }

    void OnMouseDown() {  // If this object is clicked

        if (gameManager.turn == side && !moving) {  // If it's this piece's side's turn and this piece is not moving

            // Destroy any existing move tokens
            foreach (GameObject moveToken in GameObject.FindGameObjectsWithTag("LegalMoveToken")) {
                Destroy(moveToken);
            }

            foreach (Vector3 legalMove in legalMoves) {  // For all the legal moves this piece has

                // Get a list of colliders in the space this legal move would take this piece to
                Collider[] piecesInSquare = Physics.OverlapSphere(transform.position + legalMove, 0.5f);

                if (piecesInSquare.Length == 1) {  // If there is a piece in this space
                    // If that piece is on the opposite side
                    if (piecesInSquare[0].transform.gameObject.CompareTag(sideNames[oppositeSide])) {
                        // Create an attack token in this space
                        GameObject attackToken = Instantiate(gameManager.attackToken, transform.position + legalMove, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
                        // Set the parent piece script to this class
                        attackToken.GetComponent<AttackToken>().parentPiece = this;
                        // Set the attacked piece to the piece in the space
                        attackToken.GetComponent<AttackToken>().attackedPiece = piecesInSquare[0].transform.gameObject;
                    }

                    // Else do nothing
                }

                if (piecesInSquare.Length == 0) {  // If there is not piece in the space
                    // Create a move token
                    GameObject moveToken = Instantiate(gameManager.legalMoveToken, transform.position + legalMove, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
                    // Set the parent piece of the token to this piece
                    moveToken.GetComponent<MoveToken>().parentPiece = this;
                }

            }
        }

    }

    override public void move(GameObject legalMoveToken) {  // Override parent move method

        // Use the moveOverTime coroutine to move this piece to the new space
        StartCoroutine(moveOverTime(legalMoveToken.transform.position));

        // Destroy all existing move tokens
        foreach (GameObject moveToken in GameObject.FindGameObjectsWithTag("LegalMoveToken")) {
            Destroy(moveToken);
        }

        // Move to the next player's turn
        gameManager.togglePlayer();
    }
}