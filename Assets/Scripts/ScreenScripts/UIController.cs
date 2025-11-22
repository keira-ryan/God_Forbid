using System.Collections;
using TMPro;
using UnityEngine;



public class UIController : MonoBehaviour
{
    public static UIController Instance;
    
    public GameObject startScreen;
    public GameObject endScreen;
    public GameObject pauseScreen;
    public GameObject playUI;
    public TextMeshProUGUI controlsUI;
    public GameObject gameWonScreen;
    public GameObject gameLostScreen;
    
    public SchopenhauerAI schopenhauerAI;
    public GameController gameController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        startScreen.SetActive(true);
        pauseScreen.SetActive(false);
        endScreen.SetActive(false);
        controlsUI.gameObject.SetActive(false);
        gameWonScreen.SetActive(false);
        gameLostScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ShowStartScreen()
    {
        startScreen.SetActive(true);
        endScreen.SetActive(false);
        pauseScreen.SetActive(false);
        playUI.SetActive(false);
        
    }

    public void ShowEndScreen()
    {
        endScreen.SetActive(true);
        startScreen.SetActive(false);
        pauseScreen.SetActive(false);
        playUI.SetActive(false);
    }

    public void ShowPauseScreen()
    {
        pauseScreen.SetActive(true);
        startScreen.SetActive(false);
        endScreen.SetActive(false);
        playUI.SetActive(false);
    }
    
    public void ShowPlayUI()
    {
        playUI.SetActive(true);
        startScreen.SetActive(false);
        endScreen.SetActive(false);
        pauseScreen.SetActive(false);
    }

    public void ShowDialogue()
    {
        playUI.SetActive(true);
        startScreen.SetActive(false);
        endScreen.SetActive(false);
        pauseScreen.SetActive(false);
    }

    private void ShowHealInstruction()
    {
        string message = "Right click to heal using Pneuma";
        StartCoroutine(ShowControlsInstruction(message));
    }

    private IEnumerator ShowControlsInstruction(string message)
    {
        controlsUI.text = message;
        controlsUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(4f);
        controlsUI.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Player.OnHealingGranted += ShowHealInstruction;
        Player.OnDeath += ShowGameLostScreen;
        SchopenhauerAI.OnDeath += ShowGameWonScreen;
    }

    private void ShowGameWonScreen()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        playUI.SetActive(false);
        gameWonScreen.SetActive(true);
        pauseScreen.SetActive(false);
        startScreen.SetActive(false);
        endScreen.SetActive(false);
    }

    private void OnDisable()
    {
        Player.OnHealingGranted -= ShowHealInstruction;
        Player.OnDeath -= ShowGameLostScreen;
        SchopenhauerAI.OnDeath -= ShowGameWonScreen;
    }

    private void ShowGameLostScreen()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        gameLostScreen.SetActive(true);
        gameWonScreen.SetActive(false);
        pauseScreen.SetActive(false);
        startScreen.SetActive(false);
        playUI.SetActive(false);
        endScreen.SetActive(false);
    }
}
