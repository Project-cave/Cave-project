using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public List<SkillData> skills = new List<SkillData>();
    public int gold;
    public int level;

    private SkillData[] allSkills;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }
        else
        {
            Destroy(gameObject);
        }          
        allSkills = Resources.LoadAll<SkillData>("Data/So/Skills");
    }

    public void SetSkills(List<SkillData> skills) => this.skills = skills;

    public void Save()
    {
        PlayerSaveData data = new PlayerSaveData();
        data.gold = gold;
        data.level = level;

        foreach (var skill in skills)
            data.skillNames.Add(skill != null ? skill.name : "");
        
        string json = JsonUtility.ToJson(data, true);
        System.IO.File.WriteAllText(SavePath(), json);
    }

    public void Load()
    {
        string path = SavePath();
        if (!System.IO.File.Exists(path))
        {
            InitDefault();
            return;
        }

        string json = System.IO.File.ReadAllText(path);
        PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

        gold = data.gold;
        level = data.level;

        skills.Clear();

        foreach(var name in data.skillNames)
        {
            if (string.IsNullOrEmpty(name)) skills.Add(null);

            var skill = System.Array.Find(allSkills, s=>s.name == name);
            if (skill != null) skills.Add(skill);
        }
    }

    private void InitDefault()
    {
        for (int i = 0; i < 5; i++)
        {
            skills.Add(null);
        }
    }

    private string SavePath()
    {
        Debug.Log(Application.persistentDataPath);
        return Application.persistentDataPath + "/playerData.json";      
    }
}
