﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

[System.Serializable]
public class MedalsDisplay
{
    public List<TextMeshProUGUI> display;
    public MedalsDisplay()
    {
        display = new List<TextMeshProUGUI>();
    }
}
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
    public Button lockParkourButton;

    [Header("Score by Medal")]
    public TextMeshProUGUI goldScore;
    public TextMeshProUGUI silverScore;
    public TextMeshProUGUI bronzeScore;

    [Header("Difficulties")]
    public Image parkourDifficultyRating;
    public Image aimDifficultyRating;
    public Image temporalDifficultyRating;

    [Header("Required Medals")]
    public Color conditionValid;
    public Color conditionUnvalid;
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
    public MedalsDisplay[] profileMedalObtainedCount = new MedalsDisplay[4];

    [Header("Options")]
    public Slider musiqueSlider;
    public Slider effectSlider;
    public Toggle aberrationToggle;
    public Toggle bobbingToggle;
    public Toggle vignetteToggle;
    public AK.Wwise.RTPC musiqueBusVolume;
    public AK.Wwise.RTPC effectBusVolume;



    // Start is called before the first frame update
    void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Fadder.BeginFadeIn();
        createProfileButton.onClick.AddListener(() => {
            ProfileManager.Instance().CreateProfile(profileNameInput.text);
            LoadProfiles();
            profileForm.SetActive(false);
        });
        dropdownProfiles.onValueChanged.AddListener((int dropdownIndex) => {
            ProfileManager.Instance().SwitchProfile(guidList[dropdownIndex]);
            UpdateProfile(ProfileManager.Instance().profiles[ProfileManager.Instance().currentGUIDProfile]);
        });


        //Options Binding
        // musique
        int musiqueVolume = 100;
        musiqueVolume = PlayerPrefs.GetInt("musique_volume");
        musiqueSlider.value = musiqueVolume;
        musiqueBusVolume.SetGlobalValue(musiqueVolume);
        musiqueSlider.onValueChanged.AddListener((float volume) => {
            PlayerPrefs.SetInt("musique_volume", (int)volume);
            musiqueBusVolume.SetGlobalValue(volume);
        });
        // effect
        int effectVolume = 100;
        effectVolume = PlayerPrefs.GetInt("effect_volume");
        effectSlider.value = effectVolume;
        effectBusVolume.SetGlobalValue(effectVolume);
        effectSlider.onValueChanged.AddListener((float volume) => {
            PlayerPrefs.SetInt("effect_volume", (int)volume);
            effectBusVolume.SetGlobalValue(volume);
        });
        //aberration
        bool aberration = true;
        aberration = PlayerPrefs.GetInt("aberration_chromatique") == 1 ? true : false;
        aberrationToggle.isOn = aberration;
        aberrationToggle.onValueChanged.AddListener((bool state) =>
        {
            PlayerPrefs.SetInt("aberration_chromatique", state ? 1 : 0);
        });
        //Bobbing
        bool bobbing = true;
        bobbing = PlayerPrefs.GetInt("bobbing") == 1 ? true : false;
        bobbingToggle.isOn = bobbing;
        bobbingToggle.onValueChanged.AddListener((bool state) =>
        {
            PlayerPrefs.SetInt("bobbing", state ? 1 : 0);
        });
        //vignette
        bool vignette = true;
        vignette = PlayerPrefs.GetInt("vignette") == 1 ? true : false;
        vignetteToggle.isOn = vignette;
        vignetteToggle.onValueChanged.AddListener((bool state) =>
        {
            PlayerPrefs.SetInt("vignette", state ? 1 : 0);
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
        if(selected == -1)
        {
            if(manager.currentGUIDProfile != "")
            {
                Debug.LogError("profile guid:" + manager.currentGUIDProfile + " not found");
                return;
            }
            else
            {
                Debug.Log("First play");
                musiqueSlider.value = 100;
                effectSlider.value = 100;
                aberrationToggle.isOn = true;
                bobbingToggle.isOn = true;
                vignetteToggle.isOn = true;
                profileForm.SetActive(true);
                return;
            }
        }
    }

    public void UpdateProfile(Profile profile)
    {
        nameValue.text = profile.name;
        for(int i = 0; i < 4; i++)
        {
            for(int j =0; j < profileMedalObtainedCount[i].display.Count; j++) profileMedalObtainedCount[i].display[j].text = profile.medalsObtained[i].ToString();
        }
    }

    public void LoadParkoursAssets()
    {
        Parkour[] parkoursAssets = Resources.LoadAll<Parkour>("ParkoursAssets");
        foreach (Parkour parkourAsset in parkoursAssets)
        {
#if !UNITY_EDITOR
            if (parkourAsset.isDevParkour) continue;
#endif
            GameObject parkourbutton = Instantiate(ParkourUIButtonPrefab, parent);
            parkourbutton.transform.Find("parkourName").GetComponent<TextMeshProUGUI>().text = parkourAsset.displayName;
            parkourbutton.transform.Find("image").GetComponent<Image>().sprite = parkourAsset.displayImage;
            parkourbutton.transform.Find("cadre").GetComponent<Image>().sprite = parkoursAxisSprite[(int)parkourAsset.axis];
            parkourbutton.GetComponent<Button>().onClick.AddListener(() => { ParkourSelected(parkourAsset); });


            if (ProfileManager.Instance().IsCurrentProfileValid())
            {
                Profile profile = ProfileManager.Instance().GetCurrentProfile();
                int i = parkourAsset.required.Length - 1;
                bool requiredMedals = true;
                while (i > 0 && requiredMedals)
                {
                    if (profile.medalsObtained[i] < parkourAsset.required[i])
                        requiredMedals = false;
                    i--;
                }
                if (!requiredMedals)
                {
                    parkourbutton.transform.Find("locked").gameObject.SetActive(true);
                }
            }
        }
    }

    public void ParkourSelected(Parkour parkour)
    {
        Profile profile = new Profile("ERROR");
        ParkourSaveData data = null;
        if (ProfileManager.Instance().IsCurrentProfileValid())
        {
            profile = ProfileManager.Instance().GetCurrentProfile();
            if (profile.parkoursSaveData.ContainsKey(parkour.guid))
            {
                data = profile.parkoursSaveData[parkour.guid];
            }
        }
        levelSelection.SetActive(false);
        LevelView.SetActive(true);
        LoadView(profile, parkour, data);
    }

    private void LoadView(Profile profile, Parkour parkour, ParkourSaveData saveData)
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
        bool RequiredMedals = true;
        goldRequired.text = parkour.required[1].ToString();
        silverRequired.text = parkour.required[2].ToString();
        bronzeRequired.text = parkour.required[3].ToString();
        if (profile.name != "ERROR")
        {

            // Gold
            if (profile.medalsObtained[1] >= parkour.required[1])
                goldRequired.color = conditionValid;
            else
            {
                RequiredMedals = false;
                goldRequired.color = conditionUnvalid;
            }

            // Silver
            if (profile.medalsObtained[2] >= parkour.required[2])
                silverRequired.color = conditionValid;
            else
            {
                RequiredMedals = false;
                silverRequired.color = conditionUnvalid;
            }

            // Bronze
            if (profile.medalsObtained[3] >= parkour.required[3])
                bronzeRequired.color = conditionValid;
            else
            {
                RequiredMedals = false;
                bronzeRequired.color = conditionUnvalid;
            }
        }

        if (RequiredMedals)
        {
            startParkourButton.gameObject.SetActive(true);
            lockParkourButton.gameObject.SetActive(false);
            startParkourButton.onClick.AddListener(() => {
                Loader.LoadWithLoadingScreen(parkour.scene.SceneName);
            });
        }
        else
        {
            lockParkourButton.gameObject.SetActive(true);
            startParkourButton.gameObject.SetActive(false);
        }
    }
}
