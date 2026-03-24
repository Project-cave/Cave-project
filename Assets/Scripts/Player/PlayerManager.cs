using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public List<SkillSo> skills = new List<SkillSo>();
    //public List<ItemSo> items = new List<ItemSo>();
    public int gold;
    public int level;

    private SkillSo[] allSkills;
    //private ItemSo[] allItems;

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
        allSkills = Resources.LoadAll<SkillSo>("Skills");
    }

    public void AddSkill(SkillSo skill) => skills.Add(skill);

    public void Save()
    {
        PlayerSaveData data = new PlayerSaveData();
        data.gold = gold;
        data.level = level;

        foreach (var skill in skills)
            data.skillNames.Add(skill.name);
        
        string json = JsonUtility.ToJson(data, true);
        System.IO.File.WriteAllText(SavePath(), json);
    }

    public void Load()
    {
        string path = SavePath();
        if (!System.IO.File.Exists(path)) return;

        string json = System.IO.File.ReadAllText(path);
        PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

        gold = data.gold;
        level = data.level;

        skills.Clear();

        foreach(var name in data.skillNames)
        {
            var skill = System.Array.Find(allSkills, s=>s.name == name);
            if (skill != null) skills.Add(skill);
        }
    }

    private string SavePath()
    {
        Debug.Log(Application.persistentDataPath);
        return Application.persistentDataPath + "/playerData.json";
        
    }
}
