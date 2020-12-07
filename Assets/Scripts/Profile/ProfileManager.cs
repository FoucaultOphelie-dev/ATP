using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Text;

public struct Profile
{
    public string guid;
    public string GetGuid() { return guid; }
    public string name;
    public int[] medalsObtained;
    public int[] parkourFinishByType;
    public Dictionary<string, ParkourSaveData> parkoursSaveData;
    public Profile(string profileName)
    {
        name = profileName;
        medalsObtained = new int[4];
        parkourFinishByType = new int[4];
        parkoursSaveData = new Dictionary<string, ParkourSaveData>();
        guid = Guid.NewGuid().ToString("D");
    }
}
public class ProfileManager : MonoBehaviour
{
    private static ProfileManager instance;
    public static ProfileManager Instance()
    {
        if (instance)
        {
            return instance;
        }
        else
        {
            ProfileManager manager = GameObject.FindObjectOfType<ProfileManager>();
            if (!manager)
            {
                GameObject gameObjectManager = new GameObject("Profile Manager");
                manager = gameObjectManager.AddComponent<ProfileManager>();
            }
            instance = manager;
            return instance;
        }
    }
#if UNITY_EDITOR
    [ReadOnly]
    public string currentGUIDProfile;
#else
    public string currentGUIDProfile;
#endif
    string path;
    public Dictionary<string, Profile> profiles = new Dictionary<string, Profile>();
    public void Awake()
    {
        if (instance)
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        path = Path.Combine(Application.persistentDataPath, "Profiles");
        LoadProfiles();
        currentGUIDProfile = PlayerPrefs.GetString("lastProfile");
        Debug.Log(currentGUIDProfile);
        if (!profiles.ContainsKey(currentGUIDProfile))
        {
            currentGUIDProfile = "";
        }
    }
    public void CreateProfile(string name)
    {
        Profile newProfile = new Profile(name);
        profiles.Add(newProfile.GetGuid(), newProfile);
        SaveProfile(newProfile.GetGuid());

        if (currentGUIDProfile == "")
        {
            currentGUIDProfile = newProfile.GetGuid();
            PlayerPrefs.SetString("lastProfile", currentGUIDProfile);
        }
    }
    public void LoadProfiles()
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            return;
        }
        foreach (string file in Directory.GetFiles(path))
        {
            string filepath = Path.Combine(path, file);
            string jsonData = File.ReadAllText(filepath);
            if (jsonData != "")
            {
                Profile profile = JsonConvert.DeserializeObject<Profile>(jsonData);
                profiles.Add(profile.GetGuid(), profile);
            }
        }
    }

    public void SaveProfile(string guid)
    {
        string profilePath = Path.Combine(path, guid + ".json");
        string jsonData = JsonConvert.SerializeObject(profiles[guid]);
        File.WriteAllText(profilePath, jsonData);
    }

    public void SwitchProfile(string guid)
    {
        SaveProfile(currentGUIDProfile);
        currentGUIDProfile = guid;
        PlayerPrefs.SetString("lastProfile", currentGUIDProfile);
    }

    public Profile GetCurrentProfile()
    {
        return profiles[currentGUIDProfile];
    }

    public void SaveCurrentProfile()
    {
        SaveProfile(currentGUIDProfile);
    }

    public bool IsCurrentProfileValid()
    {
        return profiles.ContainsKey(currentGUIDProfile);
    }
}