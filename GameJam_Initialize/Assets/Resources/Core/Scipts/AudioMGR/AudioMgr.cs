using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMgr : MonoBehaviour
{
    // Start is called before the first frame update
    #region 单例模式

    private static AudioMgr _instance;
    public static AudioMgr Instance
    {
        get
        {
            if (_instance == null)
            {
                // 尝试查找已存在的实例
                _instance = FindObjectOfType<AudioMgr>();

                if (_instance == null)
                {
                    // 创建新的GameObject并添加组件
                    GameObject soundManagerObject = new GameObject("AudioMgr");
                    _instance = soundManagerObject.AddComponent<AudioMgr>();
                    DontDestroyOnLoad(soundManagerObject);
                }
            }
            return _instance;
        }
    }

    #endregion

    #region 私有字段

    private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
    private List<AudioSource> _audioSources = new List<AudioSource>();
    private const string AUDIO_RESOURCES_PATH = "Audio/";
    private const int INITIAL_POOL_SIZE = 5;

    #endregion

    #region 公有属性

    /// <summary>
    /// 全局音量控制 (0-1)
    /// </summary>
    public float GlobalVolume { get; set; } = 1f;

    /// <summary>
    /// 是否启用音效
    /// </summary>
    public bool IsSoundEnabled { get; set; } = true;

    /// <summary>
    /// 已加载的音效数量
    /// </summary>
    public int LoadedClipCount => _audioClips.Count;

    #endregion

    #region Unity生命周期

    private void Awake()
    {
        // 确保单例唯一性
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        // 初始化音频源池
        InitializeAudioSourcePool();

        // 预加载音效
        PreloadAudioClips();
    }

    private void OnDestroy()
    {
        // 清理资源
        foreach (var audioSource in _audioSources)
        {
            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }
        _audioClips.Clear();
    }

    #endregion

    #region 初始化方法

    /// <summary>
    /// 初始化音频源对象池
    /// </summary>
    private void InitializeAudioSourcePool()
    {
        for (int i = 0; i < INITIAL_POOL_SIZE; i++)
        {
            CreateNewAudioSource();
        }
    }

    /// <summary>
    /// 预加载所有音效资源
    /// </summary>
    private void PreloadAudioClips()
    {
        // 加载Resources/Audio文件夹下的所有音效
        AudioClip[] clips = Resources.LoadAll<AudioClip>(AUDIO_RESOURCES_PATH);

        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning($"在路径 '{AUDIO_RESOURCES_PATH}' 下未找到音效文件");
            return;
        }

        foreach (AudioClip clip in clips)
        {
            if (!_audioClips.ContainsKey(clip.name))
            {
                _audioClips.Add(clip.name, clip);
                Debug.Log($"已加载音效: {clip.name}");
            }
            else
            {
                Debug.LogWarning($"音效名称重复: {clip.name}");
            }
        }

        Debug.Log($"音效预加载完成，共加载 {_audioClips.Count} 个音效");
    }

    /// <summary>
    /// 创建新的音频源
    /// </summary>
    private AudioSource CreateNewAudioSource()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = GlobalVolume;
        _audioSources.Add(audioSource);
        return audioSource;
    }

    #endregion

    #region 公有方法 - 基础播放功能

    /// <summary>
    /// 播放音效（只播放一次）
    /// </summary>
    /// <param name="clipName">音效名称</param>
    /// <param name="volumeScale">音量缩放 (0-1)</param>
    /// <param name="pitch">音调</param>
    /// <returns>是否成功播放</returns>
    public bool PlaySound(string clipName, float volumeScale = 1f, float pitch = 1f)
    {
        if (!IsSoundEnabled) return false;

        if (_audioClips.TryGetValue(clipName, out AudioClip clip))
        {
            AudioSource audioSource = GetAvailableAudioSource();
            if (audioSource != null)
            {
                audioSource.pitch = pitch;
                audioSource.PlayOneShot(clip, Mathf.Clamp01(volumeScale) * GlobalVolume);
                return true;
            }
        }
        else
        {
            Debug.LogWarning($"音效 '{clipName}' 未找到，请检查是否已加载");
        }

        return false;
    }

    /// <summary>
    /// 播放音效（指定位置）
    /// </summary>
    /// <param name="clipName">音效名称</param>
    /// <param name="position">播放位置</param>
    /// <param name="volumeScale">音量缩放</param>
    /// <returns>是否成功播放</returns>
    public bool PlaySoundAtPoint(string clipName, Vector3 position, float volumeScale = 1f)
    {
        if (!IsSoundEnabled) return false;

        if (_audioClips.TryGetValue(clipName, out AudioClip clip))
        {
            AudioSource.PlayClipAtPoint(clip, position, Mathf.Clamp01(volumeScale) * GlobalVolume);
            return true;
        }

        Debug.LogWarning($"音效 '{clipName}' 未找到，请检查是否已加载");
        return false;
    }

    /// <summary>
    /// 播放音效（循环播放）
    /// </summary>
    /// <param name="clipName">音效名称</param>
    /// <param name="volume">音量</param>
    /// <param name="pitch">音调</param>
    /// <returns>用于控制的AudioSource</returns>
    public AudioSource PlaySoundLoop(string clipName, float volume = 1f, float pitch = 1f)
    {
        if (!IsSoundEnabled) return null;

        if (_audioClips.TryGetValue(clipName, out AudioClip clip))
        {
            AudioSource audioSource = GetAvailableAudioSource();
            if (audioSource != null)
            {
                audioSource.clip = clip;
                audioSource.volume = Mathf.Clamp01(volume) * GlobalVolume;
                audioSource.pitch = pitch;
                audioSource.loop = true;
                audioSource.Play();
                return audioSource;
            }
        }
        else
        {
            Debug.LogWarning($"音效 '{clipName}' 未找到，请检查是否已加载");
        }

        return null;
    }

    #endregion

    #region 公有方法 - 增强功能

    /// <summary>
    /// 停止所有音效
    /// </summary>
    public void StopAllSounds()
    {
        foreach (var audioSource in _audioSources)
        {
            audioSource.Stop();
        }
    }

    /// <summary>
    /// 停止指定音效的播放
    /// </summary>
    /// <param name="clipName">音效名称</param>
    public void StopSound(string clipName)
    {
        foreach (var audioSource in _audioSources)
        {
            if (audioSource.clip != null && audioSource.clip.name == clipName && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    /// <summary>
    /// 检查音效是否正在播放
    /// </summary>
    public bool IsSoundPlaying(string clipName)
    {
        foreach (var audioSource in _audioSources)
        {
            if (audioSource.clip != null && audioSource.clip.name == clipName && audioSource.isPlaying)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 设置全局音量并更新所有音频源
    /// </summary>
    public void SetGlobalVolume(float volume)
    {
        GlobalVolume = Mathf.Clamp01(volume);
        UpdateAllAudioSourceVolumes();
    }

    /// <summary>
    /// 动态加载单个音效
    /// </summary>
    public bool LoadAudioClip(string clipName)
    {
        if (_audioClips.ContainsKey(clipName))
        {
            return true; // 已加载
        }

        AudioClip clip = Resources.Load<AudioClip>(AUDIO_RESOURCES_PATH + clipName);
        if (clip != null)
        {
            _audioClips.Add(clipName, clip);
            return true;
        }

        Debug.LogWarning($"无法加载音效: {clipName}");
        return false;
    }

    /// <summary>
    /// 卸载音效资源
    /// </summary>
    public bool UnloadAudioClip(string clipName)
    {
        if (_audioClips.ContainsKey(clipName))
        {
            // 停止正在播放的该音效
            StopSound(clipName);

            // 从字典中移除
            _audioClips.Remove(clipName);

            // 注意：Resources.Load加载的资源需要使用Resources.UnloadAsset卸载
            // 但这里我们无法获取原始的AudioClip引用，所以由Resources系统管理

            return true;
        }

        return false;
    }

    #endregion

    #region 辅助方法

    /// <summary>
    /// 获取可用的音频源
    /// </summary>
    private AudioSource GetAvailableAudioSource()
    {
        // 首先查找未在播放的音频源
        foreach (var audioSource in _audioSources)
        {
            if (!audioSource.isPlaying)
            {
                ResetAudioSource(audioSource);
                return audioSource;
            }
        }

        // 如果没有可用的，创建一个新的
        return CreateNewAudioSource();
    }

    /// <summary>
    /// 重置音频源设置
    /// </summary>
    private void ResetAudioSource(AudioSource audioSource)
    {
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.loop = false;
        audioSource.volume = GlobalVolume;
        audioSource.pitch = 1f;
    }

    /// <summary>
    /// 更新所有音频源的音量
    /// </summary>
    private void UpdateAllAudioSourceVolumes()
    {
        foreach (var audioSource in _audioSources)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.volume = GlobalVolume;
            }
            // 注意：正在播放的音频源音量不会立即改变，这是设计上的选择
            // 如果需要立即改变，可以在这里添加逻辑
        }
    }

    /// <summary>
    /// 检查音效是否已加载
    /// </summary>
    public bool HasAudioClip(string clipName)
    {
        return _audioClips.ContainsKey(clipName);
    }

    /// <summary>
    /// 获取所有已加载音效的名称
    /// </summary>
    public List<string> GetAllLoadedClipNames()
    {
        return new List<string>(_audioClips.Keys);
    }

    #endregion
}
