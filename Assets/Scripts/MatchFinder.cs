using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchFinder : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();

    private void Start()
    {
        board = GameObject.FindGameObjectWithTag(Tags.Board).GetComponent<Board>();
    }

    public  void FindAllMatch()
    {
        StartCoroutine(FindAllMatches());
    }

    List<GameObject>IsRowBomb(Dots dot1,Dots dot2, Dots dot3)//Not Used
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot1.row));
            board.BombRow(dot1.row);
        }

        if (dot2.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot2.row));
            board.BombRow(dot2.row);
        }

        if (dot3.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot3.row));
            board.BombRow(dot3.row);
        }
        return currentDots;
    }


    List<GameObject> IsAdjacentBomb(Dots dot1, Dots dot2, Dots dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot1.column, dot1.row));
        }

        if (dot2.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot2.column, dot2.row));
        }

        if (dot3.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot3.column, dot3.row));
        }
        return currentDots;
    }



    List<GameObject> IsColumnBomb(Dots dot1, Dots dot2, Dots dot3)//Not Used
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot1.column));
            board.BombColumn(dot1.column);
        }

        if (dot2.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot2.column));
            board.BombColumn(dot2.column);
        }

        if (dot3.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot3.column));
            board.BombColumn(dot3.column);
        }
        return currentDots;
    }

    private void AddToListAndMatch(GameObject dot)
    {
        if (!currentMatches.Contains(dot))
        {
            currentMatches.Add(dot);
        }
        dot.GetComponent<Dots>().isMatched = true;
    }

    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3)
    {
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);
    }


    IEnumerator FindAllMatches()
    {
        yield return null;
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentDot = board.allDots[i, j];

                if (currentDot != null)
                {
                    Dots currentDotDot = currentDot.GetComponent<Dots>();
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftDot = board.allDots[i - 1, j];

                        GameObject rightDot = board.allDots[i + 1, j];


                        if (leftDot != null && rightDot != null)
                        {
                            Dots rightDotDot = rightDot.GetComponent<Dots>();
                            Dots leftDotDot = leftDot.GetComponent<Dots>();
                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                            {
                                currentMatches.Union(IsRowBomb(leftDotDot, currentDotDot, rightDotDot));

                                currentMatches.Union(IsColumnBomb(leftDotDot, currentDotDot, rightDotDot));

                                currentMatches.Union(IsAdjacentBomb(leftDotDot, currentDotDot, rightDotDot));


                                GetNearbyPieces(leftDot, currentDot, rightDot);


                            }
                        }

                    }

                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upDot = board.allDots[i, j + 1];

                        GameObject downDot = board.allDots[i, j - 1];

                        if (upDot != null && downDot != null)
                        {
                            Dots downDotDot = downDot.GetComponent<Dots>();
                            Dots upDotDot = upDot.GetComponent<Dots>();
                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                            {
                                currentMatches.Union(IsColumnBomb(upDotDot, currentDotDot, downDotDot));

                                currentMatches.Union(IsRowBomb(upDotDot, currentDotDot, downDotDot));

                                currentMatches.Union(IsAdjacentBomb(upDotDot, currentDotDot, downDotDot));


                                GetNearbyPieces(upDot, currentDot, downDot);

                            }
                        }

                    }
                }
            }
        }
    }//FindAllMatches

    public void MatchPiecesOfColor(string color)
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                //Check if that piece exists
                if (board.allDots[i, j] != null)
                {
                    //Check the tag on that dot
                    if (board.allDots[i, j].tag == color)
                    {
                        //Set that dot to be matched
                        board.allDots[i, j].GetComponent<Dots>().isMatched = true;
                    }
                }
            }
        }
    }

    List<GameObject> GetAdjacentPieces(int column, int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = column - 1; i <= column + 1; i++)
        {
            for (int j = row - 1; j <= row + 1; j++)
            {
                //Check if the piece is inside the board
                if (i >= 0 && i < board.width && j >= 0 && j < board.height)
                {
                    if (board.allDots[i, j] != null)
                    {
                        dots.Add(board.allDots[i, j]);
                        board.allDots[i, j].GetComponent<Dots>().isMatched = true;
                    }
                }
            }
        }
        return dots;
    }//GetAjacentPiece



    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.height; i++)
        {
            if (board.allDots[column, i] != null)
            {
                Dots dot = board.allDots[column, i].GetComponent<Dots>();
                if (dot.isRowBomb)
                {
                    dots.Union(GetRowPieces(i)).ToList();
                }

                dots.Add(board.allDots[column, i]);
                dot.isMatched = true;
            }
        }
        return dots;
    }//GetColumnPieces

    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allDots[i, row] != null)
            {
                Dots dot = board.allDots[i, row].GetComponent<Dots>();
                if (dot.isColumnBomb)
                {
                    dots.Union(GetColumnPieces(i)).ToList();
                }
                dots.Add(board.allDots[i, row]);
                dot.isMatched = true;
            }
        }
        return dots;
    }//GetRowPieces

    public void CheckBombs(MatchType matchType)
    {
        //Did the player move something?
        if (board.currentDot != null)
        {
            //Is the piece they moved matched?
            if (board.currentDot.isMatched && board.currentDot.tag == matchType.color)
            {
                //make it unmatched
                board.currentDot.isMatched = false;
                if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45)
                   || (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
                {
                    board.currentDot.MakeRowBomb();
                }
                else
                {
                    board.currentDot.MakeColumnBomb();
                }
            }
            //Is the other piece matched?
            else if (board.currentDot.otherDot != null)
            {
                Dots otherDot = board.currentDot.otherDot.GetComponent<Dots>();
                //Is the other Dot matched?
                if (otherDot.isMatched && otherDot.tag == matchType.color)
                {
                    //Make it unmatched
                    otherDot.isMatched = false;
                    if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45)
                   || (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
                    {
                        otherDot.MakeRowBomb();
                    }
                    else
                    {
                        otherDot.MakeColumnBomb();
                    }
                }
            }

        }
    }

}//class










































































































































































































































