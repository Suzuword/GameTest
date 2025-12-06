using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 音乐管理器 - 单例模式
/// 预加载Resources/Audio文件夹下的所有音乐资源
/// 支持循环播放背景音乐
/// </summary>
public class MusicManager : MonoBehaviour
{
    #region 单例模式

    private static MusicManager _instance;
    public static MusicManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MusicManager>();

                if (_instance == null)
                {
                    GameObject musicManagerObject = new GameObject("MusicManager");
                    _instance = musicManagerObject.AddComponent<MusicManager>();
                    DontDestroyOnLoad(musicManagerObject);
                }
            }
            return _instance;
        }
    }

    #endregion

    #region 私有字段

    private Dictionary<string, AudioClip> _musicClips = new Dictionary<string, AudioClip>();
    private AudioSource _musicSource;
    private const string MUSIC_RESOURCES_PATH = "Audio/";

    // 淡入淡出相关
    private Coroutine _fadeCoroutine;
    private float _targetVolume = 1f;

    #endregion

    #region 公有属性

    /// <summary>
    /// 音乐音量 (0-1)
    /// </summary>
    public float MusicVolume
    {
        get => _musicSource.volume;
        set => _musicSource.volume = Mathf.Clamp01(value);
    }

    /// <summary>
    /// 是否启用音乐
    /// </summary>
    public bool IsMusicEnabled { get; set; } = true;

    /// <summary>
    /// 当前正在播放的音乐名称
    /// </summary>
    public string CurrentMusicName { get; private set; }

    /// <summary>
    /// 是否正在播放音乐
    /// </summary>
    public bool IsPlaying => _musicSource.isPlaying;

    /// <summary>
    /// 是否循环播放
    /// </summary>
    public bool Loop
    {
        get => _musicSource.loop;
        set => _musicSource.loop = value;
    }

    /// <summary>
    /// 已加载的音乐数量
    /// </summary>
    public int LoadedMusicCount => _musicClips.Count;

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

        // 初始化音频源
        InitializeMusicSource();

        // 预加载音乐
        PreloadMusicClips();
    }

    private void OnDestroy()
    {
        // 停止淡入淡出协程
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        // 停止音乐
        _musicSource.Stop();

        // 清理资源
        _musicClips.Clear();
    }

    #endregion

    #region 初始化方法

    /// <summary>
    /// 初始化音乐音频源
    /// </summary>
    private void InitializeMusicSource()
    {
        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.playOnAwake = false;
        _musicSource.loop = true; // 音乐默认循环播放
        _musicSource.volume = 0.7f; // 默认音量
    }

    /// <summary>
    /// 预加载所有音乐资源
    /// </summary>
    private void PreloadMusicClips()
    {
        // 加载Resources/Audio文件夹下的所有音乐
        AudioClip[] clips = Resources.LoadAll<AudioClip>(MUSIC_RESOURCES_PATH);

        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning($"在路径 '{MUSIC_RESOURCES_PATH}' 下未找到音乐文件");
            return;
        }

        foreach (AudioClip clip in clips)
        {
            if (!_musicClips.ContainsKey(clip.name))
            {
                _musicClips.Add(clip.name, clip);
                Debug.Log($"已加载音乐: {clip.name}");
            }
            else
            {
                Debug.LogWarning($"音乐名称重复: {clip.name}");
            }
        }

        Debug.Log($"音乐预加载完成，共加载 {_musicClips.Count} 首音乐");
    }

    #endregion

    #region 公有方法 - 基础播放控制

    /// <summary>
    /// 播放音乐（循环播放）
    /// </summary>
    /// <param name="musicName">音乐名称</param>
    /// <param name="volume">音量 (0-1)</param>
    /// <param name="loop">是否循环播放</param>
    /// <param name="restartIfPlaying">如果正在播放同一首音乐，是否重新开始</param>
    /// <returns>是否成功播放</returns>
    public bool PlayMusic(string musicName, float volume = 1f, bool loop = true, bool restartIfPlaying = false)
    {
        if (!IsMusicEnabled) return false;

        // 如果正在播放同一首音乐且不需要重新开始，则直接返回
        if (CurrentMusicName == musicName && _musicSource.isPlaying && !restartIfPlaying)
        {
            return true;
        }

        if (_musicClips.TryGetValue(musicName, out AudioClip clip))
        {
            // 停止淡入淡出效果
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }

            // 设置音乐
            _musicSource.clip = clip;
            _musicSource.volume = Mathf.Clamp01(volume);
            _musicSource.loop = loop;

            // 播放
            _musicSource.Play();

            // 记录当前音乐
            CurrentMusicName = musicName;

            Debug.Log($"开始播放音乐: {musicName}");
            return true;
        }

        Debug.LogWarning($"音乐 '{musicName}' 未找到，请检查是否已加载");
        return false;
    }

    /// <summary>
    /// 停止播放音乐
    /// </summary>
    public void StopMusic()
    {
        _musicSource.Stop();
        CurrentMusicName = null;
        Debug.Log("音乐已停止");
    }

    /// <summary>
    /// 暂停播放音乐
    /// </summary>
    public void PauseMusic()
    {
        _musicSource.Pause();
        Debug.Log("音乐已暂停");
    }

    /// <summary>
    /// 继续播放音乐
    /// </summary>
    public void ResumeMusic()
    {
        if (!IsMusicEnabled) return;

        if (_musicSource.clip != null)
        {
            _musicSource.UnPause();
            Debug.Log("音乐已继续播放");
        }
    }

    /// <summary>
    /// 切换音乐播放状态（播放/暂停）
    /// </summary>
    public void ToggleMusic()
    {
        if (_musicSource.isPlaying)
        {
            PauseMusic();
        }
        else
        {
            ResumeMusic();
        }
    }

    #endregion

    #region 公有方法 - 淡入淡出效果

    /// <summary>
    /// 淡入播放音乐
    /// </summary>
    /// <param name="musicName">音乐名称</param>
    /// <param name="fadeDuration">淡入持续时间（秒）</param>
    /// <param name="targetVolume">目标音量</param>
    /// <param name="loop">是否循环播放</param>
    public void PlayMusicWithFadeIn(string musicName, float fadeDuration = 2f, float targetVolume = 1f, bool loop = true)
    {
        if (!IsMusicEnabled) return;

        // 如果正在播放同一首音乐，则不重复播放
        if (CurrentMusicName == musicName && _musicSource.isPlaying)
        {
            return;
        }

        if (_musicClips.TryGetValue(musicName, out AudioClip clip))
        {
            // 停止之前的淡入淡出效果
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }

            // 设置音乐并开始播放
            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.volume = 0f; // 开始音量设为0
            _musicSource.Play();

            // 记录当前音乐
            CurrentMusicName = musicName;

            // 开始淡入
            _targetVolume = Mathf.Clamp01(targetVolume);
            _fadeCoroutine = StartCoroutine(FadeVolume(0f, _targetVolume, fadeDuration));

            Debug.Log($"淡入播放音乐: {musicName} ({fadeDuration}秒)");
        }
        else
        {
            Debug.LogWarning($"音乐 '{musicName}' 未找到");
        }
    }

    /// <summary>
    /// 淡出停止音乐
    /// </summary>
    /// <param name="fadeDuration">淡出持续时间（秒）</param>
    public void StopMusicWithFadeOut(float fadeDuration = 2f)
    {
        if (!_musicSource.isPlaying) return;

        // 停止之前的淡入淡出效果
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        // 开始淡出
        float startVolume = _musicSource.volume;
        _fadeCoroutine = StartCoroutine(FadeVolume(startVolume, 0f, fadeDuration, stopAfterFade: true));

        Debug.Log($"淡出停止音乐: ({fadeDuration}秒)");
    }

    /// <summary>
    /// 切换音乐并带淡入淡出效果
    /// </summary>
    /// <param name="newMusicName">新音乐名称</param>
    /// <param name="fadeDuration">淡入淡出持续时间（秒）</param>
    /// <param name="targetVolume">目标音量</param>
    /// <param name="loop">是否循环播放</param>
    public void CrossfadeToMusic(string newMusicName, float fadeDuration = 2f, float targetVolume = 1f, bool loop = true)
    {
        if (!IsMusicEnabled) return;

        // 如果正在播放同一首音乐，则不切换
        if (CurrentMusicName == newMusicName && _musicSource.isPlaying)
        {
            return;
        }

        if (_musicClips.TryGetValue(newMusicName, out AudioClip clip))
        {
            // 停止之前的淡入淡出效果
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }

            // 立即切换到新音乐，但音量为0
            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.volume = 0f; // 新音乐音量为0
            _musicSource.Play();

            // 记录当前音乐
            CurrentMusicName = newMusicName;

            // 淡入新音乐
            _targetVolume = Mathf.Clamp01(targetVolume);
            _fadeCoroutine = StartCoroutine(FadeVolume(0f, _targetVolume, fadeDuration));

            Debug.Log($"切换音乐到: {newMusicName} ({fadeDuration}秒淡入淡出)");
        }
        else
        {
            Debug.LogWarning($"音乐 '{newMusicName}' 未找到");
        }
    }

    #endregion

    #region 公有方法 - 资源管理

    /// <summary>
    /// 动态加载单个音乐
    /// </summary>
    public bool LoadMusicClip(string musicName)
    {
        if (_musicClips.ContainsKey(musicName))
        {
            return true; // 已加载
        }

        AudioClip clip = Resources.Load<AudioClip>(MUSIC_RESOURCES_PATH + musicName);
        if (clip != null)
        {
            _musicClips.Add(musicName, clip);
            return true;
        }

        Debug.LogWarning($"无法加载音乐: {musicName}");
        return false;
    }

    /// <summary>
    /// 卸载音乐资源
    /// </summary>
    public bool UnloadMusicClip(string musicName)
    {
        if (!_musicClips.ContainsKey(musicName))
        {
            return false;
        }

        // 如果正在播放该音乐，则停止
        if (CurrentMusicName == musicName && _musicSource.isPlaying)
        {
            StopMusic();
        }

        // 从字典中移除
        _musicClips.Remove(musicName);

        return true;
    }

    /// <summary>
    /// 设置音乐播放进度（0-1）
    /// </summary>
    public void SetPlaybackPosition(float normalizedTime)
    {
        if (_musicSource.clip != null)
        {
            float time = Mathf.Clamp01(normalizedTime) * _musicSource.clip.length;
            _musicSource.time = time;
        }
    }

    /// <summary>
    /// 获取音乐播放进度（0-1）
    /// </summary>
    public float GetPlaybackProgress()
    {
        if (_musicSource.clip != null && _musicSource.clip.length > 0)
        {
            return _musicSource.time / _musicSource.clip.length;
        }
        return 0f;
    }

    /// <summary>
    /// 设置音乐音调
    /// </summary>
    public void SetPitch(float pitch)
    {
        _musicSource.pitch = Mathf.Clamp(pitch, 0.5f, 2f);
    }

    #endregion

    #region 协程辅助方法

    /// <summary>
    /// 音量淡入淡出协程
    /// </summary>
    private System.Collections.IEnumerator FadeVolume(float startVolume, float endVolume, float duration, bool stopAfterFade = false)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            _musicSource.volume = Mathf.Lerp(startVolume, endVolume, t);
            yield return null;
        }

        _musicSource.volume = endVolume;

        if (stopAfterFade && endVolume <= 0f)
        {
            StopMusic();
        }

        _fadeCoroutine = null;
    }

    #endregion

    #region 公有辅助方法

    /// <summary>
    /// 检查音乐是否已加载
    /// </summary>
    public bool HasMusicClip(string musicName)
    {
        return _musicClips.ContainsKey(musicName);
    }

    /// <summary>
    /// 获取所有已加载音乐的名称
    /// </summary>
    public List<string> GetAllLoadedMusicNames()
    {
        return new List<string>(_musicClips.Keys);
    }

    /// <summary>
    /// 获取音乐时长（秒）
    /// </summary>
    public float GetMusicDuration(string musicName)
    {
        if (_musicClips.TryGetValue(musicName, out AudioClip clip))
        {
            return clip.length;
        }
        return 0f;
    }

    /// <summary>
    /// 重置音乐管理器（停止音乐并重置设置）
    /// </summary>
    public void ResetManager()
    {
        StopMusic();
        _musicSource.volume = 0.7f;
        _musicSource.pitch = 1f;
        _musicSource.loop = true;

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = null;
        }
    }

    #endregion
}
