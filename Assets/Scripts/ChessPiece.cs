using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * ChessPiece.cs
 *
 * Parent class for the scripts controlling each chess piece.
 * This is required so that other scripts can reference pieces as one
 * class, rather than having to reference each chess piece class individually.
 * With this as their parent class, chess piece scripts can all be referenced
 * by the class name ChessPiece.
 *
 * Author: Nathan "Nathcat" Baines
 */


abstract public class ChessPiece : MonoBehaviour
{

    abstract public void move(GameObject legalMoveToken);  // Abstract move method

    public IEnumerator moveOverTime(Vector3 endPosition) {  // Move to a new position over time

        moving = true;  // This piece is moving

        while (transform.position != endPosition) {  // While this piece's current position is not equal to the given end position
            // If this is a white piece, move this piece up the board, if it's a black piece, move it down the board
            if (side == 0) { transform.position += (endPosition - transform.position) * gameManager.pieceMoveSpeed; } else { transform.position += (transform.position - endPosition) * -gameManager.pieceMoveSpeed; }

            yield return new WaitForSeconds(0.01f);  // Wait for 0.01 seconds
        }

        moving = false;  // The piece has finished moving

    }

}
