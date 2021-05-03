using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsHandler : MonoBehaviour
{
    //i can assign values from the inspector, and i didn't have time
    public InputField sizex;
    public InputField sizey;
    public InputField relnummines;
    public Text minenumtext;
    public InputField samplesize;
    public Dropdown useai;
    public Toggle render;
    public Dropdown firststeprule;
    public Dropdown solvertype;
    public Dropdown guessstrategy;
    public InputField sleeptime;
    public InputField emsMAX;
    public InputField DefSolve;
    public InputField MaxBruteForce;
    public InputField[] FirstGuess;
    private MinesweeperSettings settings;


    void Awake()
    {
        //try to load
        settings=MinesweeperGame.Settings;
    }
    // Start is called before the first frame update
    void Start()
    {
        
        sizex.text = settings.x.ToString();
        sizey.text = settings.y.ToString();
        minenumtext.text = "("+settings.NumberOfMines.ToString()+")";
        relnummines.text = ((float)settings.NumberOfMines / (settings.x * settings.y)).ToString();
        //update minenumber
        sizey.onEndEdit.AddListener(delegate { UpdateNumberOfMines(); });
        sizex.onEndEdit.AddListener(delegate { UpdateNumberOfMines(); });
        relnummines.onEndEdit.AddListener(delegate { UpdateNumberOfMines(); });
        samplesize.text = settings.SampleSize.ToString();
        useai.ClearOptions();
        List<Dropdown.OptionData> usagedropdown = new List<Dropdown.OptionData>();
        foreach (var item in (MinesweeperUsageMode[])Enum.GetValues(typeof(MinesweeperUsageMode)))
        {
            usagedropdown.Add(new Dropdown.OptionData(item.ToString()));
        }
        useai.AddOptions(usagedropdown);
        useai.value=useai.options.FindIndex(x => x.text == settings.usageMode.ToString());

        render.isOn = settings.Rendering;

        firststeprule.ClearOptions();
        List<Dropdown.OptionData> fsrod = new List<Dropdown.OptionData>();
        foreach (var item in (MinesweeperFirstStepRule[])Enum.GetValues(typeof(MinesweeperFirstStepRule)))
        {
            fsrod.Add(new Dropdown.OptionData(item.ToString()));
        }
        firststeprule.AddOptions(fsrod);
        firststeprule.value = firststeprule.options.FindIndex(x => x.text == settings.FirstStepRule.ToString());

        guessstrategy.ClearOptions();
        List<Dropdown.OptionData> gsod = new List<Dropdown.OptionData>();
        foreach (var item in (GuessorType[])Enum.GetValues(typeof(GuessorType)))
        {
            gsod.Add(new Dropdown.OptionData(item.ToString()));
        }
        guessstrategy.AddOptions(gsod);
        guessstrategy.value = guessstrategy.options.FindIndex(x => x.text == settings.guessorType.ToString());

        solvertype.ClearOptions();
        List<Dropdown.OptionData> sod = new List<Dropdown.OptionData>();
        foreach (var item in (MinesweeperSolverType[])Enum.GetValues(typeof(MinesweeperSolverType)))
        {
            sod.Add(new Dropdown.OptionData(item.ToString()));
        }
        solvertype.AddOptions(sod);
        solvertype.value = solvertype.options.FindIndex(x => x.text == settings.SolverType.ToString());

        sleeptime.text = settings.SleepTime.ToString();

        emsMAX.text = settings.MGDSSP_UncoverMax.ToString();

        DefSolve.text = settings.DefaultSolvingSpeed.ToString();
        MaxBruteForce.text = settings.MaxSolvingLength.ToString();

        FirstGuess[0].text = settings.FirstGuess.x.ToString();
        FirstGuess[1].text = settings.FirstGuess.y.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public MinesweeperSettings GetCurrentSettings() 
    {
        
        int x = int.Parse(sizex.text);
        int y = int.Parse(sizey.text);
        float rmines = float.Parse(relnummines.text);
        MinesweeperSettings ms= new MinesweeperSettings(
            new Vector2Int(int.Parse(FirstGuess[0].text), int.Parse(FirstGuess[1].text)),
            x,
            y,
            (int)(rmines * (x*y)),
            (MinesweeperFirstStepRule)Enum.Parse(typeof(MinesweeperFirstStepRule), firststeprule.value.ToString()),
            render.isOn,
            (MinesweeperUsageMode)Enum.Parse(typeof(MinesweeperUsageMode), useai.value.ToString()),
            int.Parse(samplesize.text),
            int.Parse(sleeptime.text),
            (MinesweeperSolverType)Enum.Parse(typeof(MinesweeperSolverType), solvertype.value.ToString()),
            (GuessorType)Enum.Parse(typeof(GuessorType), guessstrategy.value.ToString()),
            float.Parse(emsMAX.text),
            float.Parse(DefSolve.text),
            int.Parse(MaxBruteForce.text)
            );
        return ms;
    }
    public void UpdateNumberOfMines() 
    {
        try
        {
            minenumtext.text = "(" + ((int)(float.Parse(relnummines.text) * int.Parse(sizex.text) * int.Parse(sizey.text))).ToString() + ")";
        }
        catch (System.FormatException) { Debug.LogError("Invalid format in relative number of mines field"); }
        catch (Exception) { Debug.LogError("Error in relative number of mines field"); }
    }
}
