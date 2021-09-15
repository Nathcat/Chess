using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * AttackToken.cs
 *
 * This script will be attached to move tokens that represent an opportunity
 * to attack an enemy piece.
 *
 * Author: Nathan "Nathcat" Baines
 */


public class AttackToken : MonoBehaviour
{
    public ChessPiece parentPiece;  // The ChessPiece script of the piece that the player has selected
    public GameObject attackedPiece;  // The GameObject of the piece that the move this token represents would attack

    void Start() {  // Start is called before the first frame that this object exists in
        // If this token is outside the bounds of the board...
        if (transform.position.x > 8.0f || transform.position.x < 0.0f || transform.position.z > 8.0f || transform.position.z < 0.0f) {
            Destroy(gameObject);  // Destroy this GameObject
        }
    }

    void OnMouseDown() {  // If this object is clicked
        parentPiece.move(gameObject);  // Move the selected piece to this token
        Destroy(attackedPiece.gameObject);  // Destroy the GameObject of the attacked piece
    }
}
