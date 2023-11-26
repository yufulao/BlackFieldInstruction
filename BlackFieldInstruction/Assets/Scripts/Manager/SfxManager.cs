using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class SfxManager : Singleton<SfxManager>,IMonoManager
{
    public SfxData sfxData;
    public AudioMixerGroup sfxMixerGroup;

    private Dictionary<string, SfxData.SFXDataEntry> _dataDictionary;
    private Dictionary<SfxData.SFXDataEntry, AudioSource> _sfxItems;
    
    /// <summary>
    /// 初始化Manager，设置SfxItem，为每个sfx生成SfxItem
    /// </summary>
    public void OnInit()
    {
        sfxData = Resources.Load<SfxData>("SfxData");
        sfxMixerGroup = Resources.Load<AudioMixer>("AudioMixer").FindMatchingGroups("sfx")[0];
        
        var root = new GameObject("SfxManager");
        root.transform.SetParent(GameManager.Instance.transform, false);
        
        _dataDictionary = new Dictionary<string, SfxData.SFXDataEntry>();
        _sfxItems = new Dictionary<SfxData.SFXDataEntry, AudioSource>();
        for (int i = 0; i < sfxData.data.Count; i++)
        {
            if (!string.IsNullOrEmpty(sfxData.data[i].name))
            {
                GameObject sfxObjTemp = new GameObject(sfxData.data[i].name);
                sfxObjTemp.transform.SetParent(root.transform); 
                AudioSource sfxObjAudioSource = sfxObjTemp.AddComponent<AudioSource>();
                sfxObjAudioSource.outputAudioMixerGroup = sfxMixerGroup;
                sfxObjAudioSource.playOnAwake = false;
                
                _sfxItems.Add(sfxData.data[i], sfxObjAudioSource);
                _dataDictionary.Add(sfxData.data[i].name, sfxData.data[i]);
            }
        }
    }

    /// <summary>
    /// 播放Sfx
    /// </summary>
    /// <param name="sfxName">sfx名称</param>
    /// <param name="volumeBase">初始音量</param>
    /// <param name="isLoop">是否循环</param>
    public void PlaySfx(string sfxName, float volumeBase=1f, bool isLoop = false)
    {
        if (_dataDictionary.ContainsKey(sfxName))
        {
            SfxData.SFXDataEntry entry = _dataDictionary[sfxName];
            if (entry.m_oneShot)
            {
                _sfxItems[entry].PlayOneShot(entry.maudioClips[UnityEngine.Random.Range(0, entry.maudioClips.Count)], volumeBase);
                return;
            }
            _sfxItems[entry].Stop();
            _sfxItems[entry].clip = entry.maudioClips[UnityEngine.Random.Range(0, entry.maudioClips.Count)];
            _sfxItems[entry].loop = isLoop;

            _sfxItems[entry].Play();
            _sfxItems[entry].volume = volumeBase;
        }
        else
        {
            Debug.LogError("没有这个sfx：" + sfxName);
            return;
        }
    }

    /// <summary>
    /// 停止播放音效
    /// </summary>
    /// <param name="sfxName">音效名称</param>
    public void Stop(string sfxName)
    {
        if (_dataDictionary.ContainsKey(sfxName))
        {
            SfxData.SFXDataEntry entry = _dataDictionary[sfxName];
            _sfxItems[entry].Stop();
        }
        else
        {
            Debug.LogError("没有这个sfx名：" + sfxName);
            return;
        }
    }

    /// <summary>
    /// 播放sfx过程中设置sfx音量
    /// </summary>
    /// <param name="volumeBase">要改变的音量</param>
    public void SetVolumeRuntime(float volumeBase)
    {
        foreach (var sfxItem in _sfxItems)
        {
            sfxItem.Value.volume = volumeBase;
        }
    }

    public void Update()
    {
        
    }

    public void FixedUpdate()
    {
        
    }

    public void LateUpdate()
    {
        
    }

    public void OnClear()
    {
        
    }
}