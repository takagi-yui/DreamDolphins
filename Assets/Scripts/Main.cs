using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour
{
	public const int MaxStar = 40;
	public GameObject StarPrefab;
    public GameObject Dolphin0;
	public GameObject Dolphin1;
    public Sprite[] StarSprite;
	public static Sprite[] StarColor = new Sprite[3];
	public Color[] ParticleColor;
	public static Color[] particleColor = new Color[3];
    public Color[] LineColor;
    public static Color[] lineColor = new Color[3];
	public string[] Message;
	public GameObject howToPlay;
    public LayerMask StarLayer;
	public class Character{
        public GameObject gameObject;
        public GameObject StarObject;
        public GameObject NextStarObject;
        public int level;
        public Character(GameObject gameObject,GameObject StarObject,int number,int level)
        {
            this.gameObject = gameObject;
            this.StarObject = StarObject;
            this.gameObject.name = number.ToString();
            this.level = level;
        }
	}
    public class Line
    {
        public LineRenderer lineRenderer;
        public Line(LineRenderer lineRenderer,Vector3 position0, Vector3 position1, int color)
        {
            this.lineRenderer = lineRenderer;
            this.lineRenderer.SetPosition(0, position0);
            this.lineRenderer.SetPosition(1, position1);
			this.lineRenderer.SetColors(lineColor[color],lineColor[color]);
        }
    }
    public static Character[] character = new Character[2];
    public static List<GameObject> star;
    public static List<Line>[] line = new List<Line>[2];
	public static int level;
    public static int turn;
	public static int step;
	public static bool pointerEnter;
	int count;
	int messageNumber;
	void Awake(){
		StarSprite.CopyTo (StarColor,0);
		ParticleColor.CopyTo (particleColor,0);
        LineColor.CopyTo(lineColor, 0);
		turn = 0;
		step = 0;
		pointerEnter = false;
		messageNumber = 0;
		GameObject.Find ("Message").GetComponent<Text> ().text = Message [0];
		if (level >= 2)
			howToPlay.SetActive (false);
	}
	
	void Start ()
	{
        Start:
		star = new List<GameObject> ();
		line[0] = new List<Line> ();
        line[1] = new List<Line>();
        star.Add ((GameObject)Instantiate (StarPrefab, new Vector3 (-1.0f, -1.0f, 0), Quaternion.Euler (0, 0, Random.Range (0, 360))));
		star.Add ((GameObject)Instantiate (StarPrefab, new Vector3 (1.0f, -1.0f, 0), Quaternion.Euler (0, 0, Random.Range (0, 360))));
        SetColor(star[0],1);
        SetColor(star[1],2);
        for (int n = 2; n < MaxStar; n++) {
			count = 0;
			Retry:
			star.Add ((GameObject)Instantiate (StarPrefab, new Vector3 (Random.Range (-6.00f, 6.00f), Random.Range (-4.40f, 2.50f), 0), Quaternion.Euler (0, 0, Random.Range (0, 360))));
			for (int m = 0; m < n; m++) {
				if (Vector2.Distance(star[n].transform.position,star[m].transform.position) < 1.0f) {
					Destroy (star [n]);
					star.RemoveAt (n);
					count++;
                    if (count == 5000)
                    {
                        while(star.Count > 0)
                        {
                            Destroy(star[0]);
                            star.RemoveAt(0);
                        }
                        goto Start;
                    }
					goto Retry;
				}
			}
        }
        character[0] = (new Character(Instantiate(Dolphin0, new Vector3(-1.0f, -1.0f, 0), Quaternion.identity),star[0],0,0));
		character[1] = (new Character(Instantiate(Dolphin1, new Vector3(1.0f, -1.0f, 0), Quaternion.identity), star[1],1,level));
    }
    
	void Update ()
	{
		SetAspect ();
    }
	public static void SetAspect(){
		float width, height;
		if(Screen.width * 3 < Screen.height * 4){
			width = 1;
			height = Screen.width * (3f / 4f) / Screen.height;
			GameObject.Find("Main Camera").GetComponent<Camera> ().rect = new Rect (0,(1 - height) / 2, 1, height);
		}else{
			height = 1;
			width = Screen.height * (4f / 3f) / Screen.width;
			GameObject.Find("Main Camera").GetComponent<Camera> ().rect = new Rect ((1 - width) / 2,0, width, 1);
		}
	}
	public static int GetColor(GameObject obj){
		return int.Parse (obj.GetComponent<SpriteRenderer>().sprite.name);
	}
    public static void SetColor(GameObject obj,int SpriteNumber){
        obj.GetComponent<SpriteRenderer>().sprite = Main.StarColor[SpriteNumber];
		obj.GetComponent<ParticleSystem> ().startColor = Main.particleColor[SpriteNumber];
    }
	public void PointerEnter(){
		pointerEnter = true;
	}
	public void PointerExit(){
		pointerEnter = false;
	}
	public void Menu(){
		GetComponent<AudioSource> ().Play();
		Time.timeScale = 0;
		GameObject.Find ("Menu").GetComponent<Canvas> ().enabled = true;
	}
	public void HowToPlay(){
		GetComponent<AudioSource> ().Play();
		howToPlay.SetActive (!howToPlay.activeInHierarchy);
	}
	public void NextMessage(){
			messageNumber++;
			if (messageNumber == Message.Length)
				messageNumber = 0;
			GameObject.Find ("Message").GetComponent<Text> ().text = Message [messageNumber];
		GameObject.Find ("page").GetComponent<Text> ().text = (messageNumber + 1) + "/" + Message.Length;
	}
	public void Continue(){
		GetComponent<AudioSource> ().Play();
		Time.timeScale = 1;
		GameObject.Find ("Menu").GetComponent<Canvas> ().enabled = false;
	}
	public void  Back(){
		StartCoroutine ("back");
	}
	public IEnumerator back(){
		GetComponent<AudioSource> ().Play();
		while (GetComponent<AudioSource> ().isPlaying) {
			yield return null;
		}
		Time.timeScale = 1;
		SceneManager.LoadScene(0);
	}
}
