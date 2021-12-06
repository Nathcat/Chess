using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess;

public class LegalMoveToken : MonoBehaviour
{
    public GamePiece parentPiece;

    void OnMouseDown() {
      parentPiece.MovePiece(GameObject.Find("GameManager").GetComponent<GameManager>().chess, new Vector(transform.position.x, transform.position.z));
      GameObject.Find("GameManager").GetComponent<GameManager>().DestroyAllTokens();
    }
}
