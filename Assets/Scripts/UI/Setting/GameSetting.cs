using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSetting : MonoBehaviour
{
    [SerializeField] AudioSource BGM, SE;

    public static Setting setting;

    public class Setting
    {
        public Setting(AudioSource BGM, AudioSource SE)
        {
            this.BGMVolume = BGM.volume;
            this.SEVolume = SE.volume;
            this.BGMMute = BGM.mute;
            this.SEMute = SE.mute;
            KillKingWin = BoardManager.KillKingWin;
        }

        public float BGMVolume, SEVolume;
        public bool BGMMute, SEMute;
        public bool KillKingWin;
    }

    string SettingPath => Application.persistentDataPath + $"/GameSetting.json";

    void Start()
    {
        try
        {
            LoadSetting();
        }
        catch (FileNotFoundException)
        {
            SaveSetting();
        }
    }

    public void SaveSetting()
    {
        setting = new Setting(BGM, SE);
        var json = JsonUtility.ToJson(setting);
        File.WriteAllText(SettingPath, json);
    }

    public void LoadSetting()
    {
        var json = File.ReadAllText(SettingPath);
        setting = JsonUtility.FromJson<Setting>(json);
        BGM.volume = setting.BGMVolume;
        SE.volume = setting.SEVolume;
        BGM.mute = setting.BGMMute;
        SE.mute = setting.SEMute;
        BoardManager.KillKingWin = setting.KillKingWin;
    }
}
