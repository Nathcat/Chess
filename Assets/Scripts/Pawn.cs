using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    private GameManager gameManager;
    private Vector3[] legalMoves;
    public Vector3[] attackSquares;

    public int side;
    private int oppositeSide;
    private string[] sideNames = {"White", "Black"};

    public bool moving = false;

    void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (side == 0) {
            attackSquares = new Vector3[] {
                new Vector3(1.0f, 0.0f, 1.0f),
                new Vector3(-1.0f, 0.0f, 1.0f)
            };

            legalMoves = new Vector3[] {
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 0.0f, 2.0f)
            };

            oppositeSide = 1;

        } else {
            attackSquares = new Vector3[] {
                new Vector3(1.0f, 0.0f, -1.0f),
                new Vector3(-1.0f, 0.0f, -1.0f)
            };

            legalMoves = new Vector3[] {
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -2.0f)
            };

            oppositeSide = 0;

        }
    }

    void OnMouseDown() {

        if (gameManager.turn == side && !moving) {
            foreach (GameObject moveToken in GameObject.FindGameObjectsWithTag("LegalMoveToken")) {
                Destroy(moveToken);
            }

            foreach (Vector3 legalMove in legalMoves) {
                Collider[] piecesInWay = Physics.OverlapSphere(transform.position + legalMove, 0.5f);

                if (piecesInWay.Length == 0) {
                    GameObject moveToken = Instantiate(gameManager.legalMoveToken, transform.position + legalMove, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));

                    moveToken.GetComponent<MoveToken>().parentPiece = this;
                }
            }

            foreach (Vector3 attackSquare in attackSquares) {
                Collider[] attackedPieceCollider = Physics.OverlapSphere(transform.position + attackSquare, 0.5f);

                if (attackedPieceCollider.Length == 1) {
                    if (attackedPieceCollider[0].transform.gameObject.CompareTag(sideNames[oppositeSide])) {
                        GameObject attackToken = Instantiate(gameManager.attackToken, transform.position + attackSquare, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
                        attackToken.GetComponent<AttackToken>().parentPiece = this;
                        attackToken.GetComponent<AttackToken>().attackedPiece = attackedPieceCollider[0].transform.gameObject;
                    }
                }
            }
        }

    }

    override public void move(GameObject legalMoveToken) {

        StartCoroutine(moveOverTime(legalMoveToken.transform.position));

        foreach (GameObject moveToken in GameObject.FindGameObjectsWithTag("LegalMoveToken")) {
            Destroy(moveToken);
        }

        if (side == 0) { legalMoves = new Vector3[] { new Vector3(0.0f, 0.0f, 1.0f) }; } else { legalMoves = new Vector3[] { new Vector3(0.0f, 0.0f, -1.0f) }; }

        gameManager.togglePlayer();
    }
}
