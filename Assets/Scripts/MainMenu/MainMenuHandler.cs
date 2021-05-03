using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    private AssetBundle startscene;
    public GameObject mainmenu;
    public GameObject optionsmenu;
    public OptionsHandler optionsHandler;

    //options
    public Button startbutton;
    public Button optionsbutton;
    public Button loadbutton;
    public Button exitbutton;
    public Button backtomain;

    //saveloadpanel
    public Button openslpanel;
    public GameObject slpanel;
    public Button slsavebtn;
    public Button slloabtn;
    public Button slexitbtn;
    void Awake()
    {
        mainmenu.SetActive(true);
        optionsmenu.gameObject.SetActive(false);
        optionsHandler.enabled = false;
        slpanel.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        //main
        startbutton.onClick.AddListener(OnStartButtonClick);
        exitbutton.onClick.AddListener(OnExitButtonClick);

        //options
        optionsbutton.onClick.AddListener(OnOptionsButtonClick);
        backtomain.onClick.AddListener(OnBackToMainClick);

        //SL panel
        openslpanel.onClick.AddListener(OnOpenSaveAndLoadPanel);
        slexitbtn.onClick.AddListener(OnExitSaveAndLoadPanel);
        slsavebtn.onClick.AddListener(OnSaveOptions);
        slloabtn.onClick.AddListener(OnLoadOptions);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnOptionsButtonClick() 
    {
        mainmenu.SetActive(false);
        optionsmenu.SetActive(true);
        optionsHandler.enabled = true;
    }
    public void OnExitButtonClick()
    {
        Application.Quit();
    }
    public void OnStartButtonClick()
    {
        //startscene=new AssetBundle.Load.LoadFromFile("Assets/AssetBundles/scenes");
        SceneManager.LoadScene("MinesweeperScene");
    }
    void OnBackToMainClick()
    {
        try
        {
            MinesweeperGame.Settings = optionsHandler.GetCurrentSettings();
            mainmenu.SetActive(true);
            optionsmenu.gameObject.SetActive(false);
            optionsHandler.enabled = false;
        }
        catch (System.FormatException) { Debug.LogError("Bad fomating in relative number of mines field while tried to get back to main menu"); }
        catch (System.Exception) { Debug.LogError("Unkown exception when tried to go from options to main menu"); }
    }
    void OnOpenSaveAndLoadPanel() 
    {
        slpanel.SetActive(true);
    }
    void OnExitSaveAndLoadPanel()
    {
        slpanel.SetActive(false);
    }
    void OnSaveOptions() 
    {
        
    }
    void OnLoadOptions()
    {
    
    }
}
