using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * MoveToken.cs
 *
 * This script will be attached to tokens that represent a move for the selected
 * piece.
 * Note that this does not include moves that would attack an enemy piece,
 * for moves that attack an enemy piece, see AttackToken.cs.
 *
 * Author: Nathan "Nathcat" Baines
 */


public class MoveToken : MonoBehaviour
{
    public ChessPiece parentPiece;  // The ChessPiece script attached to the selected piece

    void Start() {  // Start is called before the first frame that this object exists in
        // If this move token is outside the bounds of the chess board...
        if (transform.position.x > 8.0f || transform.position.x < 0.0f || transform.position.z > 8.0f || transform.position.z < 0.0f) {
            Destroy(gameObject);  // Destroy this move token
        }
    }

    void OnMouseDown() {  // If this token is clicked
        parentPiece.move(gameObject);  // Move the selected piece
    }
}
