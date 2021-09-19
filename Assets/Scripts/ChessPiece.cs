using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


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
    public bool moving;
    public int side;
    public GameManager gameManager;

    abstract public void move(GameObject legalMoveToken);  // Abstract move method
    abstract public object[] getLegalMoves();  // getLegalMoves abstract method
    abstract public void resetLegalMoves();  // resetLegalMoves abstract method

    public virtual object[] getLegalAttacks() {  // getLegalAttacks mehtod
      return getLegalMoves();
    }

    public float sqrt(float x) {  // Allows subclasses to use Math.Sqrt without having to import System
      return (float) Math.Sqrt(x);
    }
}
