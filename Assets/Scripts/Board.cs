using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class MatchType
{
    public int type;
    public string color;
}

[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;
}



public class Board : MonoBehaviour
{
    [Header("Scriptable Object Stuff")]
    public World world;
    public int level;

    public PlayState playState;
    [Header("Board Dimensions")]
    public int width,height,offset;

    [Header("Layout")]
    public TileType[] boardLayout;

    [Header("Prefabs")]
    public int[] scoreGoals;
    public GameObject breackableTilePrefab;
    public GameObject lockTilePrefab;
    public GameObject concreteTilePrefab;
    public GameObject slimePiecePrefab;


    private MatchFinder matchFinder;
    private ScoreManager scoreManager;
    private SoundManager soundManager;
    private int streakValue = 1;
    private bool[,] blankSpaces;
    private GoalManager goalManager;
    private BackgroundTile[,] breakableTiles;
    public BackgroundTile[,] lockTiles;
    private BackgroundTile[,] concreteTiles;
    private BackgroundTile[,] slimeTiles;
    private bool makeSlime = true;

    [Header("Match Stuff")]
    public MatchType matchType;
    public GameObject tilePrefab;
    public GameObject[] dots;
    public GameObject[,] allDots;
    public GameObject DestoryEffect;
    public Dots currentDot;
    public float refillDelay = 0.5f;
    public int basePieceValue = 20;


    private void Awake()
    {
        if (PlayerPrefs.HasKey("Current Level"))
        {
            level = PlayerPrefs.GetInt("Current Level");
        }
        if (world != null)
        {
            if (level < world.levels.Length)
            {
                if (world.levels[level] != null)
                {
                    width = world.levels[level].width;
                    height = world.levels[level].height;
                    dots = world.levels[level].dots;
                    scoreGoals = world.levels[level].scoreGoals;
                    boardLayout = world.levels[level].boardLayout;
                    
                }
            }
        }
    }

    void Start()
    {
        
        playState = PlayState.Pause;
        breakableTiles = new BackgroundTile[width, height];
        lockTiles = new BackgroundTile[width, height];
        concreteTiles = new BackgroundTile[width, height];
        slimeTiles = new BackgroundTile[width, height];
        matchFinder = GameObject.FindGameObjectWithTag(Tags.MatchFinder).GetComponent<MatchFinder>();
        scoreManager = GameObject.FindGameObjectWithTag(Tags.ScoreManager).GetComponent<ScoreManager>();
        soundManager = GameObject.FindGameObjectWithTag(Tags.ScoreManager).GetComponent<SoundManager>();
        goalManager = FindObjectOfType<GoalManager>();
        blankSpaces = new bool[width, height];
        allDots = new GameObject[width, height];
        StartCoroutine(BoardSetup());

    }

 
    void GeneratteBlackSpaces()
    {
        for (int i = 0; i <=boardLayout.Length-1; i++)
        {
           if(boardLayout[i].tileKind==TileKind.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }

    void GenerateBreckableTiles()
    {
        //look at all in layout
        for (int i = 0; i < boardLayout.Length; i++)
        {
            //if a tile is jelly tile
            if (boardLayout[i].tileKind == TileKind.Breakable)
            {
                //create jelly tile at the position
                Vector2 tempPos = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(breackableTilePrefab,tempPos,Quaternion.identity);

                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
                
            }
        }
    }

    private void GenerateLockTiles()
    {
        //Look at all the tiles in the layout
        for (int i = 0; i < boardLayout.Length; i++)
        {
            //if a tile is a "Lock" tile
            if (boardLayout[i].tileKind == TileKind.Lock)
            {
                //Create a "Lock" tile at that position;
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(lockTilePrefab, tempPosition, Quaternion.identity);
                lockTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void GenerateConcreteTiles()
    {
        //Look at all the tiles in the layout
        for (int i = 0; i < boardLayout.Length; i++)
        {
            //if a tile is a "Lock" tile
            if (boardLayout[i].tileKind == TileKind.Concrete)
            {
                //Create a "Lock" tile at that position;
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(concreteTilePrefab, tempPosition, Quaternion.identity);
                concreteTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void GenerateSlimeTiles()
    {
        //Look at all the tiles in the layout
        for (int i = 0; i < boardLayout.Length; i++)
        {
            //if a tile is a "Lock" tile
            if (boardLayout[i].tileKind == TileKind.Slime)
            {
                //Create a "Lock" tile at that position;
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(slimePiecePrefab, tempPosition, Quaternion.identity);
                slimeTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }
    IEnumerator BoardSetup()
    {
        yield return new WaitForSeconds(refillDelay);
         GeneratteBlackSpaces();
         GenerateBreckableTiles();
         GenerateLockTiles();
         GenerateConcreteTiles();
         GenerateSlimeTiles();
        for (int i =0;i<width;i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j] && !concreteTiles[i, j] && !slimeTiles[i, j])
                {
                    GameObject tiles = Instantiate(tilePrefab, new Vector2(i, j), Quaternion.identity);
                    tiles.transform.parent = transform;
                    tiles.name = "(" + i + "," + j + ")";
                    DotsInsert(i, j, tiles);
                   
                }
            }
        }

    }

    void DotsInsert(int i,int j,GameObject parent)
    {
        int dotTouse = Random.Range(0, dots.Length);
        int maxLoop = 0;
        while(MatchAt(i,j,dots[dotTouse])&&maxLoop<150)
        {
           dotTouse = Random.Range(0, dots.Length);
            maxLoop++;
        }
        maxLoop = 0;
        Vector3 position = new Vector3(i,j, offset);
        GameObject newdot = Instantiate(dots[dotTouse], position, Quaternion.identity);
        newdot.GetComponent<Dots>().row = j;
        newdot.GetComponent<Dots>().column = i;
        newdot.transform.parent = transform;
        newdot.name = parent.name;
        allDots[i,j] = newdot;
    }

    bool MatchAt(int column, int row,GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
            {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
            if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
            {
                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }

        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
                {
                    if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
            if (column > 1)
            {
                if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
                {
                    if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private MatchType ColumnOrRow()
    { //Make a copy of the current matches
        List<GameObject> matchCopy = matchFinder.currentMatches as List<GameObject>;

        matchType.type = 0;
        matchType.color = "";

        //Cycle through all of match Copy and decide if a bomb needs to be made
        for (int i = 0; i < matchCopy.Count; i++)
        {
            //Store this dot
            Dots thisDot = matchCopy[i].GetComponent<Dots>();
            string color = matchCopy[i].tag;
            int column = thisDot.column;
            int row = thisDot.row;
            int columnMatch = 0;
            int rowMatch = 0;
            //Cycle through the rest of the pieces and compare
            for (int j = 0; j < matchCopy.Count; j++)
            {
                //Store the next dot
                Dots nextDot = matchCopy[j].GetComponent<Dots>();
                if (nextDot == thisDot)
                {
                    continue;
                }
                if (nextDot.column == thisDot.column && nextDot.tag == color)
                {
                    columnMatch++;
                }
                if (nextDot.row == thisDot.row && nextDot.tag == color)
                {
                    rowMatch++;
                }
            }
            // Return 3 if column or row match
            //Return 2 if adjacent
            //Return 1 if it's a color bomb
            if (columnMatch == 4 || rowMatch == 4)
            {
                matchType.type = 1;
                matchType.color = color;
                return matchType;
            }
            else if (columnMatch == 2 && rowMatch == 2)
            {
                matchType.type = 2;
                matchType.color = color;
                return matchType;
            }
            else if (columnMatch == 3 || rowMatch == 3)
            {
                matchType.type = 3;
                matchType.color = color;
                return matchType;
            }
        }
        matchType.type = 0;
        matchType.color = "";
        return matchType;
    }

    void CheckToMakeBombs()
    {
        //How many objects are in findMatches currentMatches?
        if (matchFinder.currentMatches.Count > 3)
        {
            //What type of match?
            MatchType typeOfMatch = ColumnOrRow();
            if (typeOfMatch.type == 1)
            {
                //Make a color bomb
                //is the current dot matched?
                if (currentDot != null && currentDot.isMatched && currentDot.tag == typeOfMatch.color)
                {
                    currentDot.isMatched = false;
                    currentDot.MakeColorBomb();
                }
                else
                {
                    if (currentDot.otherDot != null)
                    {
                        Dots otherDot = currentDot.otherDot.GetComponent<Dots>();
                        if (otherDot.isMatched && otherDot.tag == typeOfMatch.color)
                        {
                            otherDot.isMatched = false;
                            otherDot.MakeColorBomb();
                        }
                    }
                }
            }

            else if (typeOfMatch.type == 2)
            {
                //Make a adjacent bomb
                //is the current dot matched?
                if (currentDot != null && currentDot.isMatched && currentDot.tag == typeOfMatch.color)
                {
                    currentDot.isMatched = false;
                    currentDot.MakeAdjacentBomb();
                }
                else if (currentDot.otherDot != null)
                {
                    Dots otherDot = currentDot.otherDot.GetComponent<Dots>();
                    if (otherDot.isMatched && otherDot.tag == typeOfMatch.color)
                    {
                        otherDot.isMatched = false;
                        otherDot.MakeAdjacentBomb();
                    }
                }
            }
            else if (typeOfMatch.type == 3)
            {
                matchFinder.CheckBombs(typeOfMatch);
            }
        }
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column, row].GetComponent<Dots>().isMatched)
        {
            //How many elements are in the matched pieces list from findmatches?
            

            //Does a tile needto Break
            if (breakableTiles[column, row] != null)
            {
                //if it does, give one damage.
                breakableTiles[column, row].TakeDamage(1);
                if (breakableTiles[column, row].hitPoints <= 0)
                {
                    breakableTiles[column, row] = null;
                }

            }
            if (lockTiles[column, row] != null)
            {
                //if it does, give one damage.
                lockTiles[column, row].TakeDamage(1);
                if (lockTiles[column, row].hitPoints <= 0)
                {
                    lockTiles[column, row] = null;
                }
            }
            DamageConcrete(column, row);
            DamageSlime(column, row);

            if (goalManager!=null)
            {
                goalManager.CompareGoal(allDots[column, row].tag.ToString());
                goalManager.UpdateGoals();
                print("goaled");
            }
            if(soundManager!=null)
            {
                soundManager.PlayRandomDestroyNoise();
            }
            // matchFinder.currentDots.Remove(allDots[col, row]);//Note
            GameObject destoryeffect = Instantiate(DestoryEffect, allDots[column, row].transform.position, Quaternion.identity);
            ParticleSystem pr0 =
            destoryeffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
            pr0.startColor = allDots[column, row].GetComponent<SpriteRenderer>().color;
            ParticleSystem pr1 =
            destoryeffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>();
            pr1.startColor = allDots[column, row].GetComponent<SpriteRenderer>().color;
            Destroy(destoryeffect, refillDelay);
            Destroy(allDots[column, row]);
            scoreManager.IncreaseScore(basePieceValue*streakValue);
            allDots[column, row] = null;
        } 
    }

    public void DestoryMatch()
    {
        if (matchFinder.currentMatches.Count >= 4)
        {
            CheckToMakeBombs();
        }
        matchFinder.currentMatches.Clear();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {

                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRowCo_1());
    }

    private IEnumerator DecreaseRowCo_1()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //if current spoint is emety and blank....
                if (!blankSpaces[i, j] && allDots[i, j] == null && !concreteTiles[i, j] && !slimeTiles[i, j])
                {
                    for(int k=j+1;k<height;k++)//loop from space above to top of the column
                    {
                        //if a dot is found..
                        if (allDots[i, k]!= null)
                        {
                            allDots[i, k].GetComponent<Dots>().row = j;    //move dot to empty space
                            allDots[i, k] = null;       //rest the moved space to empty
                            break;
                        }
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.4f);
        StartCoroutine(FillDotsOnBoard());
    }

     private IEnumerator DecreaseRowCol()
    {
        int numCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    numCount++;
                }
                else if(numCount>0)
                {
                    allDots[i, j].GetComponent<Dots>().row -= numCount;
                    allDots[i, j] = null;
                }
            }
            numCount = 0;
        }
        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillDotsOnBoard());
    }

    private IEnumerator FillDotsOnBoard()
    {
        yield return new WaitForSeconds(refillDelay );
        RefillBoard();
        while (MatchesDotsOnBoard())
        {
            streakValue++;
            DestoryMatch();
            yield break;
           // yield return new WaitForSeconds(2*refillDelay);
            
        }
        currentDot = null;
        CheckToMakeSlime();
        if (IsDeadlocked())
        {
            StartCoroutine(ShuffleBoard());
            //Debug.Log("DeadLoack");
        }
        yield return new WaitForSeconds(refillDelay);
        System.GC.Collect();
        playState = PlayState.Move;
        makeSlime = true;
        streakValue = 1;
        //Debug.Log(playState);
    }


    public void BombRow(int row)
    {
        for (int i = 0; i < width; i++)
        {
            if (concreteTiles[i, row])
            {
                concreteTiles[i, row].TakeDamage(1);
                if (concreteTiles[i, row].hitPoints <= 0)
                {
                    concreteTiles[i, row] = null;
                }
            }
        }
    }

    public void BombColumn(int column)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (concreteTiles[i, j])
                {
                    concreteTiles[column, i].TakeDamage(1);
                    if (concreteTiles[column, i].hitPoints <= 0)
                    {
                        concreteTiles[column, i] = null;
                    }
                }
            }
        }
    }
    private void DamageConcrete(int column, int row)
    {
        if (column > 0)
        {
            if (concreteTiles[column - 1, row])
            {
                concreteTiles[column - 1, row].TakeDamage(1);
                if (concreteTiles[column - 1, row].hitPoints <= 0)
                {
                    concreteTiles[column - 1, row] = null;
                }
            }
        }
        if (column < width - 1)
        {
            if (concreteTiles[column + 1, row])
            {
                concreteTiles[column + 1, row].TakeDamage(1);
                if (concreteTiles[column + 1, row].hitPoints <= 0)
                {
                    concreteTiles[column + 1, row] = null;
                }
            }
        }
        if (row > 0)
        {
            if (concreteTiles[column, row - 1])
            {
                concreteTiles[column, row - 1].TakeDamage(1);
                if (concreteTiles[column, row - 1].hitPoints <= 0)
                {
                    concreteTiles[column, row - 1] = null;
                }
            }
        }
        if (row < height - 1)
        {
            if (concreteTiles[column, row + 1])
            {
                concreteTiles[column, row + 1].TakeDamage(1);
                if (concreteTiles[column, row + 1].hitPoints <= 0)
                {
                    concreteTiles[column, row + 1] = null;
                }
            }
        }
    }
    private void DamageSlime(int column, int row)
    {
        if (column > 0)
        {
            if (slimeTiles[column - 1, row])
            {
                slimeTiles[column - 1, row].TakeDamage(1);
                if (slimeTiles[column - 1, row].hitPoints <= 0)
                {
                    slimeTiles[column - 1, row] = null;
                }
                makeSlime = false;
            }
        }
        if (column < width - 1)
        {
            if (slimeTiles[column + 1, row])
            {
                slimeTiles[column + 1, row].TakeDamage(1);
                if (slimeTiles[column + 1, row].hitPoints <= 0)
                {
                    slimeTiles[column + 1, row] = null;
                }
                makeSlime = false;

            }
        }
        if (row > 0)
        {
            if (slimeTiles[column, row - 1])
            {
                slimeTiles[column, row - 1].TakeDamage(1);
                if (slimeTiles[column, row - 1].hitPoints <= 0)
                {
                    slimeTiles[column, row - 1] = null;
                }
                makeSlime = false;

            }
        }
        if (row < height - 1)
        {
            if (slimeTiles[column, row + 1])
            {
                slimeTiles[column, row + 1].TakeDamage(1);
                if (slimeTiles[column, row + 1].hitPoints <= 0)
                {
                    slimeTiles[column, row + 1] = null;
                }
                makeSlime = false;

            }
        }
    }

 
    private void CheckToMakeSlime()
    {
        //Check the slime tiles array
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (slimeTiles[i, j] != null && makeSlime)
                {
                    //Call another method to make a new slime
                    MakeNewSlime();
                    return;
                }
            }
        }
    }

    private Vector2 CheckForAdjacent(int column, int row)
    {
        print(column + ":" + row);
        if(column==0 || row==0)
        {
            return Vector2.zero;
            
        }
        if (allDots[column + 1, row]!=null && column < width-1 )
        {
            return Vector2.right;
        }
        if (allDots[column - 1, row]!=null && column > 0)
        {
            return Vector2.left;
        }
        if (allDots[column, row + 1] != null && row < height-1)
        {
            return Vector2.up;
        }
        if (allDots[column, row - 1] != null && row > 0)
        {
            return Vector2.down;
        }
        return Vector2.zero;
    }

    private void MakeNewSlime()
    {
        bool slime = false;
        int loops = 0;
        while (!slime && loops < 200)
        {
            int newX = Random.Range(0, width-1);
            int newY = Random.Range(0, height-1);
            if (slimeTiles[newX, newY] != null)
            {
                Vector2 adjacent = CheckForAdjacent(newX, newY);
                Debug.Log(adjacent);
                if (adjacent != Vector2.zero)
                {
                    if(allDots[newX + (int)adjacent.x, newY + (int)adjacent.y]!=null)
                    Destroy(allDots[newX + (int)adjacent.x, newY + (int)adjacent.y]);
                    Vector2  tempPosition = new Vector2(newX + (int)adjacent.x, newY + (int)adjacent.y);
                    GameObject tile = Instantiate(slimePiecePrefab, tempPosition, Quaternion.identity);
                    slimeTiles[newX + (int)adjacent.x, newY + (int)adjacent.y] = tile.GetComponent<BackgroundTile>();
                    slime = true;
                } 
            }
            loops++;

        }
    }

    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        //Take second Piece and Save it in holder
        GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y] as GameObject;
        //switch first dot to the seconf position
        allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
        //set the first dot to the second dot
        allDots[column, row] = holder;
    }

    private bool CheckForMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    //Make sure that one and two to the right are in the
                    //board
                    if (i < width - 2)
                    {
                        //Check if the dots to the right and two to the right exist
                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            if (allDots[i + 1, j].tag == allDots[i, j].tag
                               && allDots[i + 2, j].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }

                    }
                    if (j < height - 2)
                    {
                        //Check if the dots above exist
                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if (allDots[i, j + 1].tag == allDots[i, j].tag
                               && allDots[i, j + 2].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }

    private bool IsDeadlocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (i < width - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if (j < height - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    private IEnumerator ShuffleBoard()
    {
        yield return new WaitForSeconds(0.5f);
        //Create a list of game objects
        List<GameObject> newBoard = new List<GameObject>();
        //Add every piece to this list
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    newBoard.Add(allDots[i, j]);
                }
            }
        }
        yield return new WaitForSeconds(0.5f);
        //for every spot on the board. . . 
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //if this spot shouldn't be blank
                if (!blankSpaces[i, j] && !concreteTiles[i, j] && !slimeTiles[i, j])
                {
                    //Pick a random number
                    int pieceToUse = Random.Range(0, newBoard.Count);

                    //Assign the column to the piece
                    int maxIterations = 0;

                    while (MatchAt(i, j, newBoard[pieceToUse]) && maxIterations < 100)
                    {
                        pieceToUse = Random.Range(0, newBoard.Count);
                        maxIterations++;
                        Debug.Log(maxIterations);
                    }
                    //Make a container for the piece
                    Dots piece = newBoard[pieceToUse].GetComponent<Dots>();
                    maxIterations = 0;
                    piece.column = i;
                    //Assign the row to the piece
                    piece.row = j;
                    //Fill in the dots array with this new piece
                    allDots[i, j] = newBoard[pieceToUse];
                    //Remove it from the list
                    newBoard.Remove(newBoard[pieceToUse]);
                }
            }
        }
        //Check if it's still deadlocked
        if (IsDeadlocked())
        {
            StartCoroutine(ShuffleBoard());
        }
    }


    void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null && !blankSpaces[i, j] && !concreteTiles[i, j] && !slimeTiles[i, j])
                {
                   
                    Vector2 tempPosition = new Vector2(i, j + offset);
                    int dotToUse = Random.Range(0, dots.Length);
                    int maxIterations = 0;

                    while (MatchAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        dotToUse = Random.Range(0, dots.Length);
                    }

                    maxIterations = 0;
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dots>().row = j;
                    piece.GetComponent<Dots>().column = i;

                }
            }
        }
    }//RefillBoard

    private bool MatchesDotsOnBoard()
    {
        matchFinder.FindAllMatch();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j]!= null)
                {
                   if(allDots[i, j].GetComponent<Dots>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }//MatcheDotsOnBoard


    

}//class
























































































































































































































































































































