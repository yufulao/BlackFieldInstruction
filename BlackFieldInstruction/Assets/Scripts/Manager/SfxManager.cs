using System.Collections;
using System.Collections.Generic;
using Rabi;
using UnityEngine;
using UnityEngine.Audio;


public class SfxManager : BaseSingleTon<SfxManager>,IMonoManager
{
    private CfgSfx _cfgSfx;
    private AudioMixerGroup _sfxMixerGroup;

    private Dictionary<string, RowCfgSfx> _dataDictionary;
    private Dictionary<RowCfgSfx, AudioSource> _sfxItems;
    
    /// <summary>
    /// 初始化Manager，设置SfxItem，为每个sfx生成SfxItem
    /// </summary>
    public void OnInit()
    {
        _cfgSfx=ConfigManager.Instance.cfgSfx;
        _sfxMixerGroup = AssetManager.Instance.LoadAsset<AudioMixer>("AudioMixer").FindMatchingGroups("sfx")[0];
        
        var root = new GameObject("SfxManager");
        root.transform.SetParent(GameManager.Instance.transform, false);
        
        _dataDictionary = new Dictionary<string, RowCfgSfx>();
        _sfxItems = new Dictionary<RowCfgSfx, AudioSource>();
        for (int i = 0; i < _cfgSfx.AllConfigs.Count; i++)
        {
            if (!string.IsNullOrEmpty(_cfgSfx.AllConfigs[i].key))
            {
                GameObject sfxObjTemp = new GameObject(_cfgSfx.AllConfigs[i].key);
                sfxObjTemp.transform.SetParent(root.transform); 
                AudioSource sfxObjAudioSource = sfxObjTemp.AddComponent<AudioSource>();
                sfxObjAudioSource.outputAudioMixerGroup = _sfxMixerGroup;
                sfxObjAudioSource.playOnAwake = false;
                
                _sfxItems.Add(_cfgSfx.AllConfigs[i], sfxObjAudioSource);
                _dataDictionary.Add(_cfgSfx.AllConfigs[i].key, _cfgSfx.AllConfigs[i]);
            }
        }
    }

    /// <summary>
    /// 播放Sfx
    /// </summary>
    /// <param name="sfxName">sfx名称</param>
    /// <param name="volumeBase">初始音量</param>
    /// <param name="isLoop">是否循环</param>
    public IEnumerator PlaySfx(string sfxName, float volumeBase=1f, bool isLoop = false)
    {
        if (_dataDictionary.ContainsKey(sfxName))
        {
            RowCfgSfx rowCfgSfx = _dataDictionary[sfxName];

            yield return AssetManager.Instance.LoadAssetAsync<AudioClip>(
                rowCfgSfx.audioClipPaths[UnityEngine.Random.Range(0, rowCfgSfx.audioClipPaths.Count)],
                (clip) =>
                {
                    PlaySfxAsync(rowCfgSfx, volumeBase, isLoop, clip);
                });
            yield break;
        }
        else
        {
            Debug.LogError("没有这个sfx：" + sfxName);
            yield break;
        }
    }
    
    /// <summary>
    /// 异步获取到audioClip后播放
    /// </summary>
    /// <param name="rowCfgSfx"></param>
    /// /// <param name="volumeBase"></param>
    /// /// <param name="isLoop"></param>
    /// <param name="clip"></param>
    private void PlaySfxAsync(RowCfgSfx rowCfgSfx,float volumeBase,bool isLoop,AudioClip clip)
    {
        if (rowCfgSfx.oneShot)
        {
            _sfxItems[rowCfgSfx].PlayOneShot(clip, volumeBase);
        }
        else
        {
            _sfxItems[rowCfgSfx].Stop();
            _sfxItems[rowCfgSfx].clip = clip;
            _sfxItems[rowCfgSfx].loop = isLoop;
            _sfxItems[rowCfgSfx].volume = volumeBase;
            _sfxItems[rowCfgSfx].Play();
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
            RowCfgSfx rowCfgSfx = _dataDictionary[sfxName];
            _sfxItems[rowCfgSfx].Stop();
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