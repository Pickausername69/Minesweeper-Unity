using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
//using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

///<summary>
///Handles the visual representation of the minesweeper and charts and so on...
///</summary>
public class MinesweeperHandler : MonoBehaviour
{
    [Header("Board settings")]
    MinesweeperGamestate gamestate;
    MinesweeperSettings settings;
    public int x;
    public int y;
    public int numberofmines;
    public Vector2Int[] operators = new Vector2Int[]
    {
        new Vector2Int(0,1),
        new Vector2Int(1,1),
        new Vector2Int(1,0),
        new Vector2Int(1,-1),
        new Vector2Int(0,-1),
        new Vector2Int(-1,-1),
        new Vector2Int(-1,0),
        new Vector2Int(-1,1)
    };
    public MinesweeperFirstStepRule firststeprule;
    public bool rendering;
    public MinesweeperUsageMode usagemode;
    public MinesweeperSolverType solvertype;
    public float MGDSSP_UncoverMax;
    public Vector2Int FirstGuess;
    public int MaxSolvingLength;
    [Header("Visual settings")]
    //size of each sprite (number of pixels per axis)
    public Vector2Int tilesize= new Vector2Int(16,16);
    public GameObject minesweepergameimage;
    public VisualDecorator vh;
    float relativenumberofmines;
    public GameObject overlayingamebutton;
    private Vector2Int updatepos;
    [Header("Minesweeper board settings")]
    private bool useai = false;
    private bool ClickToStep = true;
    private bool requireclick = false;
    public Button clicktostepbutton;
    public float updatetime = 1f;
    public int sleeptime = 0;

    [Header("AI settings")]
    public int samplesize = 1000;
    public int sampleindex = 0;
    public int samplewins = 0;
    public  GuessorType guessor;
    public Text indextext;
    public Text wintext;
    public Button backmainbtn;
    public GameObject InfoPanel;
    BaseMinesweeperSolver af;
    void SetUpSolver()
    {
        BaseGuessor g;
        if (FirstGuess == new Vector2Int(-1, -1))
        {
            switch (guessor)
            {
                case GuessorType.Random:
                    g = new RandomGuessor(af);
                    break;

                case GuessorType.Corner:
                    g = new CornerGuessor(af, settings.x, settings.y);
                    break;
                default:
                    throw new System.Exception("Uknown guessor type");

            }
        }
        else 
        {
            switch (guessor)
            {
                case GuessorType.Random:
                    g = new RandomGuessor(af,FirstGuess);
                    break;

                case GuessorType.Corner:
                    g = new CornerGuessor(af, settings.x, settings.y,FirstGuess);
                    break;
                default:
                    throw new System.Exception("Uknown guessor type");

            }
        }
        switch (solvertype)
        {
            case MinesweeperSolverType.Standard_DSSP:
                af = new DSSP(operators, new Vector2Int(x, y), g);
                break;
            case MinesweeperSolverType.Enhanced_DSSP:
                af = new MineGuessingDSSP(operators, new Vector2Int(x, y), g,MGDSSP_UncoverMax);
                break;
            case MinesweeperSolverType.SimpleBruteForce_CSP:
                af = new SimpleBruteForceCSP(operators, new Vector2Int(x, y), g, relativenumberofmines, MaxSolvingLength);
                break;
            case MinesweeperSolverType.SimpleBackTrack_CSP:
                af = new SimpleBackTrackCSP(operators, new Vector2Int(x, y), g, relativenumberofmines, MaxSolvingLength);
                break;
            case MinesweeperSolverType.BorderGuessing_DSSP_WithMineGuessing:
                af = new DSSP_ButRandomBorderGuess(operators, new Vector2Int(x, y), g, true);
                break;
            case MinesweeperSolverType.BorderGuessing_DSSP_WithUncovering:
                af = new DSSP_ButRandomBorderGuess(operators, new Vector2Int(x, y), g, false);
                break;
            default:
                throw new System.NotImplementedException("Unkown MinesweeperSolverType");
        }
        g.bms = af;

    }
    void Start()
    {
        SetUpInteractions();
        //UseTestsettings();
        SetUpSettings();
        SetUpVisualization();

        //new game
        NewGame();
        Debug.Log("Mines: " + (int)(numberofmines));
        updatepos = new Vector2Int(-1, -1);
        overlayingamebutton.transform.position = new Vector2(-10000, -10000);
        if (useai) { SetUpSolver(); }
        updatepos = new Vector2Int();

    }

    private void SetUpSettings()
    { 
        settings = MinesweeperGame.Settings;
        operators = settings.Operators;
        FirstGuess = settings.FirstGuess; 
        x = settings.x;
        y = settings.y;
        numberofmines = settings.NumberOfMines;
        firststeprule = settings.FirstStepRule;
        rendering = settings.Rendering;
        usagemode = settings.usageMode;
        // if (usagemode == MinesweeperUsageMode.NoAI) { ClickToStep = false; useai = false; } originally the user could player but this functionality was removed
        if (usagemode == MinesweeperUsageMode.ClickToStepAI) { ClickToStep = true; useai = true; }
        else { ClickToStep = false; useai = true; }
        samplesize = settings.SampleSize;
        sleeptime = settings.SleepTime;
        solvertype = settings.SolverType;
        guessor = settings.guessorType;
        MGDSSP_UncoverMax = settings.MGDSSP_UncoverMax;
        updatetime = settings.DefaultSolvingSpeed;
        thinkingtime.text = updatetime.ToString();
        gamestate = MinesweeperGame.Gamestate;
        relativenumberofmines = (float)settings.NumberOfMines / (settings.x * settings.y);
        MaxSolvingLength = settings.MaxSolvingLength;
    }
    private void UseTestsettings() {
        MinesweeperSettings mss = new MinesweeperSettings(operators, FirstGuess, x, y, numberofmines, firststeprule, rendering, usagemode, samplesize, sleeptime, solvertype, guessor, MGDSSP_UncoverMax, updatetime, MaxSolvingLength);
        MinesweeperGame.Settings = mss;
    }
    private void SetUpInteractions() {
        backmainbtn.onClick.AddListener(delegate {
            //need to be null, otherwise it would not create new Gamestate object on next start
            MinesweeperGame.Gamestate = null;
            SceneManager.LoadScene("MainMenu");
        });
        thinkingtime.onEndEdit.AddListener(delegate {
            updatetime = float.Parse(thinkingtime.text);
        });
        clicktostepbutton.gameObject.SetActive(ClickToStep);
        clicktostepbutton.onClick.AddListener(delegate {
            requireclick = false;
        });
    }
    private void SetUpVisualization() {
        Visualizer vbase = new Visualhandler(minesweepergameimage, Resources.Load("Highlightprefab") as GameObject, overlayingamebutton, new Vector2Int(settings.x, settings.y));
        vh = new VisualDecorator(vbase, rendering);
        vh.SetUpVisualMinesweeper();
    }

    private float currenttime = 0;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) 
        {
            InfoPanel.SetActive(!InfoPanel.activeInHierarchy);
        }
        if (!useai)
        {
            if (Input.GetMouseButtonDown(0))
            {
                MinesweeperInteractionManager.OnStep(new KeyValuePair<MinesweeperActionType, Vector2Int>(MinesweeperActionType.Uncover, new Vector2Int(updatepos.x, -updatepos.y + settings.y - 1)),
                    gamestate, settings.Operators);
                vh.UpdateVisualMinesweeperComplete(gamestate);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                MinesweeperInteractionManager.OnStep(new KeyValuePair<MinesweeperActionType, Vector2Int>(MinesweeperActionType.Flag,new Vector2Int(updatepos.x, -updatepos.y + settings.y - 1)),
                    gamestate, settings.Operators);
                vh.UpdateVisualMinesweeperComplete(gamestate);
            }
        }
        if (sampleindex <= samplesize) { currenttime = updatetime; }
        while (useai && currenttime >= 0 && !requireclick)
        {
            if (!gamestate.GameOver)
            {
                Vector2Int[] needtobeupdated;
                KeyValuePair<MinesweeperActionType, Vector2Int> aistep = af.ChooseStep();
                //Debug.Log(aistep);
                if (gamestate.FirstStep)
                {
                    needtobeupdated = MinesweeperInteractionManager.FirstOnReveal(gamestate, aistep.Value, settings.FirstStepRule, settings.Operators);
                    gamestate.FirstStep = false;
                }
                else
                {
                    needtobeupdated = MinesweeperInteractionManager.OnStep(aistep, gamestate, settings.Operators);
                }
                //update database, and request tiles
                af.GetRelevantBoard(MinesweeperElementInfo.GetFilteredTiles(gamestate, needtobeupdated), aistep.Key);
                //answer to requested tiles
                //KeyValuePair<AIRequestType, KeyValuePair<Vector2Int, MinesweeperElementInfo>[]>[] answertiles = AIRequestProvider.GetRequestedMinesweeperElementInfos(gamestate, requestedtiles);
                //update database with requested tiles
                //af.AnswerProcecessor(answertiles);

                vh.UpdateVisualMinesweeperQuick(MinesweeperElementInfo.GetFilteredTiles(gamestate, needtobeupdated));


            }

            if (sampleindex <= samplesize)
            {
                if (gamestate.GameOver)
                {
                    NewGame();
                    SetUpSolver();
                }
                else if (af.Flaged.Count == settings.NumberOfMines)
                {
                    samplewins++;
                    NewGame();
                    SetUpSolver();
                }
            }
            if (ClickToStep)
            {
                requireclick = true;
            }

            currenttime -= Time.deltaTime;
        }

        //helps to see the current state of the game
        if (useai && settings.Rendering)
        {
            Vector2Int[] v2is = af.InvalidTiles.GetPositions(true);
            vh.HighlightTiles(af.Mine.ToArray(), AIDataType.Mine);
            vh.HighlightTiles(af.Safe.ToArray(), AIDataType.Safe);
            vh.HighlightTiles(af.Open.Keys.ToArray(), AIDataType.Open);
            vh.HighlightTiles(af.Closed.Keys.ToArray(), AIDataType.Closed);
            vh.HighlightTiles(v2is, AIDataType.Invalid);

            vh.FinalizeUpdate();
        }


        if (sleeptime > 0) { Thread.Sleep(sleeptime); }
        indextext.text = (sampleindex-1).ToString();
        wintext.text = samplewins.ToString();
        //fps
        fpsText.text = ((int)(1.0f / Time.deltaTime)).ToString();
    }
    //write out fps
    public Text fpsText;
    public InputField thinkingtime;
    void NewGame()
    {
        sampleindex++;
        MinesweeperGame.CreateNewGame();
        vh.UpdateVisualMinesweeperComplete(gamestate);
    }
}
