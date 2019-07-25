using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dots : MonoBehaviour
{
    [Header("Board Variables")]
    public bool isMatched;
    public int column, row,targetX,targetY,previousColumn, previousRow;
    public float swipeAngle,swipeAngleLimt;
    public GameObject otherDot;
    private Board board;
    private MatchFinder matchFinder;
    private EndGameManager endGameManager;
    private Vector2 firstTouchPos,finalTouchPos, tempPosition;
    public GameObject rowArrowPrefab, columnArrowPrefab, colorBombPrefab,ajacentMarkerPreab;
    public bool isColumnBomb, isRowBomb, isColorBomb, isAdjacentBomb;
    private HintManager hintManager;
    
    private void Start()
    {
        swipeAngleLimt = 1f;
        isColorBomb = false;
        isRowBomb = false;
        isAdjacentBomb = false;
        isColorBomb = false;
        hintManager = FindObjectOfType<HintManager>();
        board = GameObject.FindGameObjectWithTag(Tags.Board).GetComponent<Board>();
        matchFinder= GameObject.FindGameObjectWithTag(Tags.MatchFinder).GetComponent<MatchFinder>();
        endGameManager = FindObjectOfType<EndGameManager>();
        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //column = targetX;
        //row = targetY;
        //previousColumn = column;
        //previousRow = row;
    }
    private void Update()
    {
       
        if(isMatched)
        {
            //SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
           // mySprite.color = new Color(1, 1, 1, 0.2f);
           // isMatched = false;
        }
        targetX = column;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            //Move Towards the target
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
                matchFinder.FindAllMatch();
            }
            
        }
        else
        {
            //Directly set the position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;

        }
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            //Move Towards the target
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
                matchFinder.FindAllMatch();
            }
           
        }
        else
        {
            //Directly set the position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }

    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isAdjacentBomb = true;
            GameObject colorbomb = Instantiate(ajacentMarkerPreab, transform.position, Quaternion.identity);
            colorbomb.transform.parent = this.transform;
        }
    }

    private void OnMouseDown()
    {
        if (board.playState == PlayState.Move)
        {
            firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        }
    }

    private void OnMouseUp()
    {
        if (board.playState == PlayState.Move)
        {
            finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
            
    }
    

    void CalculateAngle()
    {
        if(hintManager!=null)
        hintManager.DestroyHint();
        if (Mathf.Abs(finalTouchPos.y - firstTouchPos.y) > swipeAngleLimt
            || Mathf.Abs(finalTouchPos.x - firstTouchPos.x) > swipeAngleLimt)
        {
            board.playState = PlayState.Wait;
            swipeAngle = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y, finalTouchPos.x - firstTouchPos.x) * 180 / Mathf.PI;
            MovePieces();
            
            board.currentDot = this;
        }
        else
        {
            board.playState = PlayState.Move;
        }
    }

    void MovePieceActual(Vector2 angle)
    {
        otherDot = board.allDots[column + (int)angle.x, row + (int)angle.y];
        previousRow = row;
        previousColumn = column;
        if (board.lockTiles[column, row] == null && board.lockTiles[column + (int)angle.x, row + (int)angle.y] == null)
        {
           
            if (otherDot != null)
            {
                otherDot.GetComponent<Dots>().column += -1 * (int)angle.x;
                otherDot.GetComponent<Dots>().row += -1 * (int)angle.y;
                column += (int)angle.x;
                row += (int)angle.y;
                StartCoroutine(CheckIsMatch());
            }
            else
            {
                board.playState = PlayState.Move;
            }
        }
        else
        {
            board.playState = PlayState.Move;
        }
    }

    void MovePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
        {
            /* //Right Swipe
             otherDot = board.allDots[column + 1, row];
             previousRow = row;
             previousColumn = column;
             otherDot.GetComponent<Dots>().column -= 1;
             column += 1;
             StartCoroutine(CheckIsMatch());
             */
            MovePieceActual(Vector2.right);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            /*//Up Swipe
            otherDot = board.allDots[column, row + 1];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dots>().row -= 1;
            row += 1;
            StartCoroutine(CheckIsMatch());
            */
            MovePieceActual(Vector2.up);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            /* //Left Swipe
             otherDot = board.allDots[column - 1, row];
             previousRow = row;
             previousColumn = column;
             otherDot.GetComponent<Dots>().column += 1;
             column -= 1;
             StartCoroutine(CheckIsMatch());
             */
            MovePieceActual(Vector2.left);
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            /* //Down Swipe
             otherDot = board.allDots[column, row - 1];
             previousRow = row;
             previousColumn = column;
             otherDot.GetComponent<Dots>().row += 1;
             row -= 1;
             StartCoroutine(CheckIsMatch());
             */
            MovePieceActual(Vector2.down);
        }
        else
          board.playState = PlayState.Move;
        
        // StartCoroutine(CheckIsMatch());
    }//MovePiece Method


    public IEnumerator CheckIsMatch()
    {
        if(isColorBomb)//this piece is colorbomb, and other piece of same color to Destory
        {
            matchFinder.MatchPiecesOfColor(otherDot.tag);
            isMatched = true;
        }
        else if(otherDot.GetComponent<Dots>().isColorBomb)//other piece is colorbomb, and this piece of same color to Destory
        {
            matchFinder.MatchPiecesOfColor(this.gameObject.tag);
            otherDot.GetComponent<Dots>().isMatched = true;
        }
        yield return new WaitForSeconds(.5f);
        if(otherDot != null)
        {
            if(!otherDot.GetComponent<Dots>().isMatched && !isMatched)
            {
                otherDot.GetComponent<Dots>().row = row;
                otherDot.GetComponent<Dots>().column = column;
                row = previousRow;
                column = previousColumn;
                board.playState = PlayState.Move;
               // board.currentDot = null; 
            }
            else
            {
                if (endGameManager != null)
                {
                    if (endGameManager.requirements.gameType == GameType.Moves)
                    {
                        endGameManager.DecreaseCounterValue();
                    }
                }
                board.DestoryMatch();
            }
            
        }else
        board.playState = PlayState.Move;

    }
    /*  void FindMatch()
      {
          if(column>0&& column < board.width-1)
          {
              GameObject leftDot = board.allDots[column - 1, row];
              GameObject rightDot = board.allDots[column + 1, row];
              if (leftDot != null && rightDot != null)
              {
                  if (leftDot.tag == this.gameObject.tag && rightDot.tag == this.gameObject.tag)
                  {
                      leftDot.GetComponent<Dots>().isMatched = true;
                      rightDot.GetComponent<Dots>().isMatched = true;
                      isMatched = true;
                  }

              }
          }
          if (row > 0 && row <  board.height - 1)
          {
              GameObject UptDot = board.allDots[column, row+1];
              GameObject DownDot = board.allDots[column, row-1];
              if (UptDot != null && DownDot != null)
              {
                  if (UptDot.tag == this.gameObject.tag && DownDot.tag == this.gameObject.tag)
                  {
                      UptDot.GetComponent<Dots>().isMatched = true;
                      DownDot.GetComponent<Dots>().isMatched = true;
                      isMatched = true;
                  }

              }
          }

      }

      */

    public void MakeRowBomb()
    {
        if (!isColumnBomb && !isColorBomb && !isAdjacentBomb)
        {
            isRowBomb = true;
            GameObject rowArrow = Instantiate(rowArrowPrefab, transform.position, Quaternion.identity);
            rowArrow.transform.parent = transform;
        }
    }

    public void MakeColumnBomb()
    {
        if (!isRowBomb && !isColorBomb && !isAdjacentBomb)
        {
            isColumnBomb = true;
            GameObject colArrow = Instantiate(columnArrowPrefab, transform.position, Quaternion.identity);
            colArrow.transform.parent = transform;
        }
    }

    public void MakeColorBomb()
    {
        if (!isColumnBomb && !isRowBomb && !isAdjacentBomb)
        {
            isColorBomb = true;
            GameObject colorbomb = Instantiate(colorBombPrefab, transform.position, Quaternion.identity);
            colorbomb.transform.parent = transform;
            this.gameObject.tag = Tags.Color;
        }
    }

    public void MakeAdjacentBomb()
    {
        if (!isColumnBomb && !isRowBomb && !isColorBomb)
        {
            isAdjacentBomb = true;
            GameObject marker = Instantiate(ajacentMarkerPreab, transform.position, Quaternion.identity);
            marker.transform.parent = transform;
        }
    }


}//class




















































































































































