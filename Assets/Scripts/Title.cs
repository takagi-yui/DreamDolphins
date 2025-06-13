using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour {
	void Awake(){
		if (!PlayerPrefs.HasKey ("level")) {
			PlayerPrefs.SetInt ("level",1);
		}
		if(PlayerPrefs.GetInt("level") >= 2)Destroy(GameObject.Find("Panel2"));
		if(PlayerPrefs.GetInt("level") >= 3)Destroy(GameObject.Find("Panel3"));
		if (PlayerPrefs.HasKey ("HighScore1")) {
			GameObject.Find ("HighScore1").GetComponent<Text> ().text = "High Score  " + PlayerPrefs.GetInt ("HighScore1");
		}
		if (PlayerPrefs.HasKey ("HighScore2")) {
			GameObject.Find ("HighScore2").GetComponent<Text> ().text = "High Score  " + PlayerPrefs.GetInt ("HighScore2");
		}
		if (PlayerPrefs.HasKey ("HighScore3")) {
			GameObject.Find ("HighScore3").GetComponent<Text> ().text = "High Score  " + PlayerPrefs.GetInt ("HighScore3");
		}
	}
	void Start () {
		
	}
	void Update () {
		Main.SetAspect ();
	}
	public void ChangeCanvas(int number){
		for (int n = 0; n < 3; n++) {
			GameObject.Find ("Canvas" + n).GetComponent<Canvas> ().enabled = false;
		}
		GetComponent<AudioSource> ().Play();
		GameObject.Find ("Canvas" + number).GetComponent<Canvas> ().enabled = true;
	}
	public void LoadScene(int level)
    {
		Main.level = level;
		StartCoroutine ("Play");
    }
	public void Exit(){
		GetComponent<AudioSource> ().Play();
		Application.Quit ();
	}
	public IEnumerator Play(){
		GetComponent<AudioSource> ().Play();
		while (GetComponent<AudioSource> ().isPlaying) {
			yield return null;
		}
		SceneManager.LoadScene(1);
	}
}
