using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class GameProgressTracker : MonoBehaviour
{
    public static GameProgressTracker instance;

    public const string DataPath = "Archievements";
    public ArchivementData unlockedArchivements;

    private void Awake()
    {
        if (!instance) instance = this;
        else Destroy(this);

        LoadArchivementData();
    }

    public void LoadArchivementData()
    {
        unlockedArchivements = Resources.Load<ArchivementData>("Archievements");
    }

    public static ArchivementData GetUnlockedArchivements()
    {
        return Resources.Load<ArchivementData>("Archievements");
    }

    public static void NotifyCompletedLevel(sceneIndex level)
    {
        if (instance.unlockedArchivements == null)
            instance.LoadArchivementData();

        switch (level)
        {
            case sceneIndex.Lvl1:
                instance.unlockedArchivements.Nivel1Completado = true;
                Analytics.CustomEvent("Level1Finalized");
                break;
            case sceneIndex.Lvl2:
                instance.unlockedArchivements.Nivel2Completado = true;
                Analytics.CustomEvent("Level2Finalized");
                break;
            case sceneIndex.Lvl3:
                instance.unlockedArchivements.JuegoCompletado = true;
                Analytics.CustomEvent("GameCompleted");
                break;
            default:
                break;
        }
    }

    public static void NofitySimpleEnemyKilled()
    {
        if (instance.unlockedArchivements == null)
            instance.LoadArchivementData();

        instance.unlockedArchivements.TotalEnemigosSimplesAsesinados++;

        if (instance.unlockedArchivements.TotalEnemigosSimplesAsesinados == 5)
            instance.unlockedArchivements.EnemigoSimplesAsesinados = true;

        //LLamamos a los eventos de Analitics.
        AnalyticsEvent.Custom("SimpleEnemyiKilled", new Dictionary<string, object>
        {
            { "Total killed", instance.unlockedArchivements.EnemigoSimplesAsesinados }
        });
    }

    public static void NotifyRangeEnemyKilled()
    {
        if (instance.unlockedArchivements == null)
            instance.LoadArchivementData();

        instance.unlockedArchivements.TotalEnemigosRangoAsesinados++;

        if (instance.unlockedArchivements.TotalEnemigosRangoAsesinados == 5)
            instance.unlockedArchivements.EnemigosRangoAsesinados = true;

        //LLamamos a Analitics again.
        AnalyticsEvent.Custom("RangeEnemyKilled", new Dictionary<string, object>
        {
            { "Total killed", instance.unlockedArchivements.EnemigosRangoAsesinados }
        });
    }

    public static void NotifyPlayerDied()
    {
        if (instance.unlockedArchivements == null)
            instance.LoadArchivementData();

        if (!instance.unlockedArchivements.PrimeraMuerte)
        {
            instance.unlockedArchivements.PrimeraMuerte = true;
            
            //Notificamos a Analitics.
            Analytics.CustomEvent("PlayerFirstDeath");
        }
    }

    public static void BossEnemyKilled()
    {
        if (instance.unlockedArchivements == null)
            instance.LoadArchivementData();

        if (!instance.unlockedArchivements.BossAsesinado)
        {
            instance.unlockedArchivements.BossAsesinado = true;
            //Notificación.
            Analytics.CustomEvent("BossKilled");
        }

        instance.unlockedArchivements.TotalBossesEliminados++;
    }
}
