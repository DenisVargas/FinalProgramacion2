using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CanvasController : MonoBehaviour {

    static public CanvasController Instance { get; private set; }


    [SerializeField] GameObject MainMenuCanvas= null;

    [Header("Archivements")]
    [SerializeField] GameObject AchievmentCanvas = null;
    [SerializeField] TMP_Text Arch1;
    [SerializeField] TMP_Text Arch2;
    [SerializeField] TMP_Text Arch3;
    [SerializeField] TMP_Text Arch4;
    [SerializeField] TMP_Text Arch5;


    [Header("Menú de Pausa")]
    [SerializeField] GameObject PauseMenu = null;
    [SerializeField] GameObject GameOptions = null;
    [SerializeField] GameObject FixedJoystick = null;
    [SerializeField] GameObject ShootButton = null;
    [SerializeField] GameObject ConsoleComand = null;
    [SerializeField] GameObject LeftButton = null;
    [SerializeField] GameObject RightButton = null;

    // [SerializeField] GameObject InteractButton = null;


    public static bool IsInPause;

	void Start ()
    {
        Instance = this;
       
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ActivatePause();
        }
    }
    //PANTALLA DE MENU
    public void StartGame()
    {
        Game.LoadScene(sceneIndex.Lvl1);
    }
    public void Credits()
    {
        Game.LoadScene(sceneIndex.Credits);
    }

    public void Achievments()
    {
        MainMenuCanvas.SetActive(false);
        AchievmentCanvas.SetActive(true);

        GameProgressTracker.instance.LoadArchivementData();
        var archivements = GameProgressTracker.GetUnlockedArchivements();

        if (archivements.TotalEnemigosSimplesAsesinados > 0 ||archivements.TotalEnemigosRangoAsesinados > 0)
            Arch1.color = Color.green;

        if (archivements.PrimeraMuerte)
            Arch2.color = Color.green;

        if (archivements.EnemigoSimplesAsesinados || archivements.EnemigosRangoAsesinados)
            Arch3.color = Color.green;

        if (archivements.Nivel1Completado)
            Arch4.color = Color.green;

        if (archivements.JuegoCompletado)
            Arch4.color = Color.green;
    }

    public void Return()
    {
        MainMenuCanvas.SetActive(true);
        AchievmentCanvas.SetActive(false);
    }

    ////PANTALLA PERDIDA
    //public void TryAgain()
    //{
            
    //}

    //MENU DE PAUSA
    public void Continue()
    {
        Time.timeScale = 1;
        PauseMenu.SetActive(false);
        //GameOptions.SetActive(true);
        //FixedJoystick.SetActive(true);
       // ShootButton.SetActive(true);
       // LeftButton.SetActive(true);
      //  RightButton.SetActive(true);
        //InteractButton.SetActive(true);
    }

    public void GoToMenu()
    {
        Game.LoadScene(sceneIndex.MainMenu);
    }

    public void X()
    {
        Application.Quit();
    }
    public void ActivatePause()
    {
        Time.timeScale = 0;
        PauseMenu.SetActive(true);
        GameOptions.SetActive(false);
        FixedJoystick.SetActive(false);
        ShootButton.SetActive(false);
        LeftButton.SetActive(false);
        RightButton.SetActive(false);
        //InteractButton.SetActive(false);
    }

    public void ActivateConsole()
    {
        ConsoleComand.SetActive(true);
        PauseMenu.SetActive(false);
    }


}
