using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public struct Medal
{
    public enum MedalType
    {
        Author,
        Gold,
        Silver,
        Bronze
    }
    public static string MedalTypeToString(MedalType type)
    {
        switch (type)
        {
            case MedalType.Author:
                return "Author";
            case MedalType.Gold:
                return "Gold";
            case MedalType.Silver:
                return "Silver";
            case MedalType.Bronze:
                return "Bronze";
        }
        return null;
    }
    public MedalType type;
    public int score;
}

[CreateAssetMenu(fileName = "newParkour", menuName = "New Parkour")]
public class Parkour : ScriptableObject
{
    [Tooltip("Nom affiché au joueur")]
    public string displayName;

    [Tooltip("Difficulté Affiché")]
    [Range(0,5)]
    public int difficulty;

    [Tooltip("Score initialement défini pour effectué le parcours\n" +
        "ce score diminuera d'une certaines quantité selon le temps que mets le joueur a finir le parkour")]
    public int startingScore;

    [Tooltip("Récompence du parkour\n" +
        "lorsque toute les médailes sont obtenus (or, argent, bronze)")]
    public int reward;

    [Tooltip("Liste des médailes disponible")]
    public Medal[] medals =
    {
        new Medal {type = Medal.MedalType.Author, score = 0},
        new Medal {type = Medal.MedalType.Gold,   score = 0},
        new Medal {type = Medal.MedalType.Silver, score = 0},
        new Medal {type = Medal.MedalType.Bronze, score = 0},
    };

    [SerializeField]
    public SceneField scene;
    public Parkour nextParkour;

    public bool isTutorial;

    public List<float> timerByCheckpoint = new List<float>();
}
#if UNITY_EDITOR
[CustomEditor(typeof(Parkour))]
public class ParkourEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Parkour parkour = (Parkour)target;
        base.OnInspectorGUI();

        for (int j = 0; j < 4; j++)
        {
            if (j < parkour.medals.Length)
            {
                if(parkour.medals[j].type != (Medal.MedalType)j)
                {
                    EditorGUILayout.HelpBox("Medal at index " + j + " should be of type " + Medal.MedalTypeToString((Medal.MedalType)j), MessageType.Error);
                }
            }
            else
            {
                EditorGUILayout.HelpBox(Medal.MedalTypeToString((Medal.MedalType)j) + " Medal is missing", MessageType.Error);
            }
        }

        int min = parkour.startingScore;
        int i = 0;
        while(i < parkour.medals.Length)
        {
            if (parkour.medals[i].score > min)
            {
                EditorGUILayout.HelpBox(Medal.MedalTypeToString(parkour.medals[i].type) + " Medal score should not be higher than previous medal", MessageType.Error);
                break;
            }
            min = parkour.medals[i].score;
            i++;
        }
    }
}
#endif
