using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("Level Selector")]
    public GameObject ParkourUIButtonPrefab;
    public Transform parent;

    public GameObject mainMenu;
    public GameObject levelSelection;
    public GameObject LevelView;

    public Sprite[] medalsSprites;
    public Sprite[] parkoursAxisSprite;

    [Header("Level View")]
    [Header("General")]
    public TextMeshProUGUI parkourName;
    public TextMeshProUGUI bestScoreText;
    public Image bestMedalSprite;
    public Button startParkourButton;

    [Header("Score by Medal")]
    public TextMeshProUGUI goldScore;
    public TextMeshProUGUI silverScore;
    public TextMeshProUGUI bronzeScore;

    [Header("Difficulties")]
    public Image parkourDifficultyRating;
    public Image aimDifficultyRating;
    public Image temporalDifficultyRating;

    [Header("Required Medals")]
    public TextMeshProUGUI goldRequired;
    public TextMeshProUGUI silverRequired;
    public TextMeshProUGUI bronzeRequired;

    // Start is called before the first frame update
    void Awake()
    {
        LoadParkoursAssets();
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    public void LoadParkoursAssets()
    {
        Parkour[] parkoursAssets = Resources.LoadAll<Parkour>("ParkoursAssets");
        foreach (Parkour parkourAsset in parkoursAssets)
        {
            GameObject parkourbutton = Instantiate(ParkourUIButtonPrefab, parent);
            parkourbutton.transform.Find("parkourName").GetComponent<TextMeshProUGUI>().text = parkourAsset.displayName;
            parkourbutton.transform.Find("cadre").GetComponent<Image>().sprite = parkoursAxisSprite[(int)parkourAsset.axis];
            parkourbutton.GetComponent<Button>().onClick.AddListener(() => { ParkourSelected(parkourAsset); });
        }
    }

    public void ParkourSelected(Parkour parkour)
    {
        ParkourSaveData data = ParkourManager.loadData(parkour);
        levelSelection.SetActive(false);
        LevelView.SetActive(true);
        LoadView(parkour, data);
    }

    private void LoadView(Parkour parkour, ParkourSaveData saveData)
    {
        //general
        parkourName.text = parkour.displayName;
        bestScoreText.text = saveData != null ? saveData.bestScore.ToString() : "";
        bestMedalSprite.sprite = saveData != null ? medalsSprites[saveData.bestMedalObtained] : null;
        if(saveData == null) bestMedalSprite.color = Color.clear;
        //MedalsScores
        goldScore.text = parkour.medals[1].score.ToString();
        silverScore.text = parkour.medals[2].score.ToString();
        bronzeScore.text = parkour.medals[3].score.ToString();
        //difficulties
        float ratio = 1f / 5f;
        parkourDifficultyRating.fillAmount = ratio * parkour.parkourDifficulty;
        aimDifficultyRating.fillAmount = ratio * parkour.aimDifficulty;
        temporalDifficultyRating.fillAmount = ratio * parkour.timePowerDifficulty;
        //Required
        goldRequired.text = parkour.required[1].ToString();
        silverRequired.text = parkour.required[2].ToString();
        bronzeRequired.text = parkour.required[3].ToString();

        startParkourButton.onClick.AddListener(() => { Loader.LoadWithLoadingScreen(parkour.scene.SceneName); });
    }
}
