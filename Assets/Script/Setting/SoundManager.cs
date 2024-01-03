using UnityEngine;
using System.Collections;
using CMNCMD;
public class SoundManager : MonoBehaviour {
	public static  AudioSource 	login_music;
	public static  AudioSource 	mainscene_music;
	public static  AudioSource 	battle_music;
	public static  AudioSource 	click_music;
	public static  BG_MUSIC 	palying_music;
	// Use this for initialization
	void Start () {
			//AudioSource clickaudio = (AudioSource)((GameObject)Instantiate(click_sound)).GetComponent(typeof(AudioSource));
			//bgaudio.Play();
			//clickaudio.volume = 0.2f;
		login_music =	(AudioSource)GameObject.Find("LoginMusic").GetComponent(typeof(AudioSource));
		mainscene_music =	(AudioSource)GameObject.Find("MainSceneMusic").GetComponent(typeof(AudioSource));
		battle_music =	(AudioSource)GameObject.Find("BattleMusic").GetComponent(typeof(AudioSource));
		click_music =	(AudioSource)GameObject.Find("ClickMusic").GetComponent(typeof(AudioSource));
		DontDestroyOnLoad(gameObject);
		if( PlayerPrefs.GetInt("setting_first")>0 ) {
			// not the first time, chanage the vol to last setting
			float vol = PlayerPrefs.GetFloat("setting_music_vol");
			ChangeBgMusicVolume(vol);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public static void SetSettingDefaultValue()
	{
		//AudioSource login_music =	(AudioSource)GameObject.Find("LoginMusic").GetComponent(typeof(AudioSource));
		//AudioSource mainscene_music =	(AudioSource)GameObject.Find("MainSceneMusic").GetComponent(typeof(AudioSource));
		//AudioSource battle_music =	(AudioSource)GameObject.Find("BattleMusic").GetComponent(typeof(AudioSource));
		//GameObject.Find("SettingManager").SendMessage("SetBgSoundValue",bg_music.volume);
		//GameObject.Find("SettingManager").SendMessage("SetClickSoundValue",click_sound.volume);
	}
	public static void ChangeBgMusicVolume(float vol)
	{
		mainscene_music.volume = vol;
		battle_music.volume = vol;
		login_music.volume = vol;
	}
	public static void PlayLoginMusic()
	{
		palying_music = BG_MUSIC.LOGIN_MUSIC;
		login_music.Play();
	}
	public static void StopLoginMusic()
	{
		palying_music = BG_MUSIC.NONE;
		login_music.Stop();
	}
	public static void PlayMainSceneSound()
	{
		palying_music = BG_MUSIC.MAINSCENE_MUSIC;
		mainscene_music.Play();
	}
	public static void StopMainSceneSound()
	{
		palying_music = BG_MUSIC.NONE;
		mainscene_music.Stop();
	}
	public static void PlayBattleSound()
	{
		palying_music = BG_MUSIC.BATTLE_MUSIC;
		battle_music.Play();
	}
	public static void StopBattleSound()
	{
		palying_music = BG_MUSIC.NONE;
		battle_music.Stop();
	}
	public static void PlayClickSound()
	{
		click_music.Play();
	}
	public static void SetSoundVolume(float volume)
	{
		click_music.volume = volume;
		//selectchar_sound.volume = volume;
	}
	public static void  SetBgMusicVolume(float volume)
	{
		login_music.volume = volume;
		mainscene_music.volume = volume;
		battle_music.volume = volume;
	}
}
