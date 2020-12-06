using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

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
    public Image parkourImage;
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

    [Header("Profiles")]
    public GameObject profileForm;
    public TMP_InputField profileNameInput;
    public Button createProfileButton;
    public TMP_Dropdown dropdownProfiles;
    private List<string> guidList = new List<string>();
    public TextMeshProUGUI nameValue;
    public TextMeshProUGUI[] profileMedalObtainedCount = new TextMeshProUGUI[4];

    // Start is called before the first frame update
    void Awake()
    {
        createProfileButton.onClick.AddListener(() => {
            ProfileManager.Instance().CreateProfile(profileNameInput.text);
            LoadProfiles();
            profileForm.SetActive(false);
        });
        dropdownProfiles.onValueChanged.AddListener((int dropdownIndex) => {
            ProfileManager.Instance().SwitchProfile(guidList[dropdownIndex]);
            UpdateProfile(ProfileManager.Instance().profiles[ProfileManager.Instance().currentGUIDProfile]);
        });
        LoadProfiles();
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

    public void LoadProfiles()
    {
        guidList = new List<string>();
        dropdownProfiles.ClearOptions();
        ProfileManager manager = ProfileManager.Instance();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        int selected = -1;
        if(manager.currentGUIDProfile != "")
        {
            foreach (KeyValuePair<string, Profile> profile in manager.profiles)
            {
                options.Add(new TMP_Dropdown.OptionData(profile.Value.name));
                guidList.Add(profile.Key);
                if (profile.Key == manager.currentGUIDProfile)
                {
                    selected = options.Count - 1;
                    UpdateProfile(profile.Value);
                }
            }
            dropdownProfiles.AddOptions(options);
            dropdownProfiles.value = selected;
            if(dropdownProfiles.value == -1)
            {
                Debug.LogError("profile guid:" + manager.currentGUIDProfile + " not found");
                return;
            }
        }
        else
        {
            Debug.Log("First play");
            profileForm.SetActive(true);
            return;
        }
    }

    public void UpdateProfile(Profile profile)
    {
        nameValue.text = profile.name;
        for(int i = 0; i < 4; i++)
        {
            profileMedalObtainedCount[i].text = profile.medalsObtained[i].ToString();
        }
    }

    public void LoadParkoursAssets()
    {
        Parkour[] parkoursAssets = Resources.LoadAll<Parkour>("ParkoursAssets");
        foreach (Parkour parkourAsset in parkoursAssets)
        {
            GameObject parkourbutton = Instantiate(ParkourUIButtonPrefab, parent);
            parkourbutton.transform.Find("parkourName").GetComponent<TextMeshProUGUI>().text = parkourAsset.displayName;
            parkourbutton.transform.Find("image").GetComponent<Image>().sprite = parkourAsset.displayImage;
            parkourbutton.transform.Find("cadre").GetComponent<Image>().sprite = parkoursAxisSprite[(int)parkourAsset.axis];
            parkourbutton.GetComponent<Button>().onClick.AddListener(() => { ParkourSelected(parkourAsset); });
        }
    }

    public void ParkourSelected(Parkour parkour)
    {
        ParkourSaveData data = null;
        if (ProfileManager.Instance().IsCurrentProfileValid())
        {
            Profile profile = ProfileManager.Instance().GetCurrentProfile();
            if (profile.parkoursSaveData.ContainsKey(parkour.guid))
            {
                data = profile.parkoursSaveData[parkour.guid];
            }
        }
        levelSelection.SetActive(false);
        LevelView.SetActive(true);
        LoadView(parkour, data);
    }

    private void LoadView(Parkour parkour, ParkourSaveData saveData)
    {
        //general
        parkourName.text = parkour.displayName;
        parkourImage.sprite = parkour.displayImage;
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
