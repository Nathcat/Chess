using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess;

public class GamePiece : MonoBehaviour
{
    public int index;
    public int side;
    public float yOffset;
    public GameManager gameManager;

    void Start() {
      gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
      yOffset = transform.position.y;
    }

    public Vector[][] GetLegalMoves(ChessEngine chess) {
      Vector position = new Vector(transform.position.x, transform.position.z);

      return chess.GetLegalMoves(position);
    }

    public bool MovePiece(ChessEngine chess, Vector newPosition) {
      Vector position = new Vector(transform.position.x, transform.position.z);

      bool result = chess.MovePiece(position, newPosition);
      if (result) {
        StartCoroutine(MoveOverTime(new Vector3(newPosition.x, yOffset, newPosition.y)));
      }

      return result;
    }

    void OnMouseDown() {
      if (gameManager.checkmate) {
        return;
      }

      if (side == gameManager.chess.turnCounter.turn) {
        gameManager.CreateTokens(GetLegalMoves(gameManager.chess), this);
      }
    }

    IEnumerator MoveOverTime(Vector3 newPosition) {
      while (transform.position != newPosition) {
        transform.position = transform.position + ((newPosition - transform.position) * gameManager.moveIncrement);

        if ((newPosition - transform.position).magnitude <= 0.01f) {
          transform.position = newPosition;
        }

        yield return new WaitForSeconds(gameManager.moveWait);
      }
    }
}
