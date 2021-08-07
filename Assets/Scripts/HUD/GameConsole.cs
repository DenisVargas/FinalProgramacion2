using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/*
 * Recorte:
 * //string wea = "algo con espacios";
        //var messages = wea.Split(' ');
        //foreach (var item in messages)
        //{
        //    print("Item:" + item);
        //}
*/
public class GameConsole : MonoBehaviour
{

    [SerializeField] GameObject PauseMenu = null;
    [SerializeField] GameObject ConsoleComand = null;

    public void DesactivateConsole()
    {
        ConsoleComand.SetActive(false);
        PauseMenu.SetActive(true);
    }


    public static GameConsole instance;
    Dictionary<string, Action> Commands = new Dictionary<string, Action>();
    [SerializeField] TMP_Text _consoleText = null;
    [SerializeField] TMP_InputField _inputField = null;


    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(this);

        Commands = new Dictionary<string, Action>();

        //Comandos (5).
        //1. Infinite Bullets.
        string command1Hash = (("DoomGuyIntensifies").ToLowerInvariant());
        Commands.Add(command1Hash, () => 
        {
            Player player = FindObjectOfType<Player>();
            player.DoomGuyMode(true);

            Print("Infinte bulles mode Activated");
        });

        //2. No Enemy mode.
        string command2Hash = (("GGEasy").ToLowerInvariant());
        Commands.Add(command2Hash, () => 
        {
            var enemiesInScene = FindObjectsOfType<Enemy>();
            foreach (var Enemy in enemiesInScene)
                Enemy.gameObject.SetActive(false);

            Print("Stage Enemies Desactivated");
        });

        //3. Bullets Damage++
        string command3Hash = (("PumpedUpKicks").ToLowerInvariant());
        Commands.Add(command3Hash, () => 
        {
            Player player = FindObjectOfType<Player>();
            player.AddExtraDamage(100);

            Print("Damage of all Guns Increased by 100!!");
        });

        //4. Insta Heal.
        string command4Hash = (("SorakaPressedUlt").ToLowerInvariant());
        Commands.Add(command4Hash, () => 
        {
            Player player = FindObjectOfType<Player>();
            player.RestoreAllHealth();

            Print("Health restored to max!!");
        });

        //5. Level1
        string command5Hash = (("DevLoadLevel1").ToLowerInvariant());
        Commands.Add(command5Hash, () => Game.LoadScene(sceneIndex.Lvl1));

        //5. Level2
        string command6Hash = (("DevLoadLevel2").ToLowerInvariant());
        Commands.Add(command6Hash, () => Game.LoadScene(sceneIndex.Lvl2));

        //7. Level3
        string command7Hash = (("IHaveNoTimeForThis").ToLowerInvariant());
        Commands.Add(command7Hash, () => Game.LoadScene(sceneIndex.Lvl3));

        //8. CloseConsole
        string command8Hash = (("Close").ToLower());
        Commands.Add(command8Hash, () => gameObject.SetActive(false));

        //9. ExitGame
        string command9Hash = (("ExitGame").ToLowerInvariant());
        Commands.Add(command9Hash, () => Application.Quit());

        //10. ReduxPlayerHealth by 10.
        string command10Hash = (("DevAuch").ToLowerInvariant());
        Commands.Add(command10Hash, () =>
        {
            Player player = FindObjectOfType<Player>();
            player.Hit(new HitData() { Damage = 10 });

            Print("Dev: Dealing 10 Damage to the Player!!");
        });
        print("Cantidad de comandos: " + Commands.Count);
    }

    public void CheckCommand()
    {
        string lowCaseCommand = (_inputField.text.ToLowerInvariant());

        if (Commands.ContainsKey(lowCaseCommand))
        {
            print("Comando reconocido");
            Commands[lowCaseCommand]();
        }
        else
            print("Comando Erróneo");

        _inputField.text = "";
    }

    public static void Print(string message)
    {
        if (instance)
        {
            instance._consoleText.text += string.Format("\n{0}", message);
        }
    }
}
