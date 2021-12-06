using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject[] pieces;
    public ChessEngine chess;
    public GameObject legalMoveToken;
    public GameObject legalAttackToken;
    public GameObject gameStateText;
    public GameObject turnImage;
    public Sprite[] playerSprites;
    public bool checkmate = false;
    public float moveIncrement = 0.1f;
    public float moveWait = 0.1f;

    void Start() {
      chess = new ChessEngine();
    }

    void Update() {
      foreach (GameObject piece in pieces) {
        if (piece == null) {
          continue;
        }

        int index = piece.GetComponent<GamePiece>().index;
        bool found = false;

        foreach (Piece piece_ in chess.pieces) {
          if (piece_ == null) {
            continue;
          }

          if (piece_.index == index) {
            found = true;
            //piece.transform.position = new Vector3(piece_.position.x, piece.GetComponent<GamePiece>().yOffset, piece_.position.y);
          }
        }

        if (!found) {
          Destroy(piece);
        }
      }

      if (chess.inCheck) {
        gameStateText.GetComponent<Text>().text = "Check!";

      } else {
        gameStateText.GetComponent<Text>().text = "";
      }

      if (chess.checkmate) {
        gameStateText.GetComponent<Text>().text = "Checkmate!";
        checkmate = true;
      }

      turnImage.GetComponent<Image>().sprite = playerSprites[chess.turnCounter.turn];
    }

    public void DestroyAllTokens() {
      foreach (GameObject token in GameObject.FindGameObjectsWithTag("LegalMoveToken")) {
        Destroy(token);
      }
    }

    public void CreateTokens(Vector[][] tokens, GamePiece parentPiece) {
      DestroyAllTokens();

      foreach (Vector move in tokens[0]) {
        GameObject token = Instantiate(legalMoveToken, new Vector3(move.x, 0.5f, move.y), new Quaternion());
        token.GetComponent<LegalMoveToken>().parentPiece = parentPiece;
      }

      foreach (Vector attack in tokens[1]) {
        GameObject token = Instantiate(legalAttackToken, new Vector3(attack.x, 0.5f, attack.y), new Quaternion());
        token.GetComponent<LegalMoveToken>().parentPiece = parentPiece;
      }
    }
}
