using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Move : MonoBehaviour {
    const float speed = 5;
    public GameObject LinePrefab;
    int numberA,numberB;
    GameObject StarObject;
    GameObject NextStarObject;
    List<Main.Line> line;
    void Start () {
        numberA = int.Parse(gameObject.name);
        numberB = 1 - numberA;
        StarObject = Main.character[numberA].StarObject;
        NextStarObject = Main.character[numberA].NextStarObject;
		if (numberA == 0) {
			gameObject.transform.eulerAngles = new Vector3 (0, 0, 180);
			GetComponent<Animator> ().Play ("Dolphin Animation" + numberA,-1,0.5f);
		}
		GetComponent<Animator> ().speed = 0.7f;
    }
	
	void Update () {
        if (Main.turn == numberA)
        {
            StarObject = Main.character[numberA].StarObject;
            NextStarObject = Main.character[numberA].NextStarObject;
            line = Main.line[numberA];
            if (Main.step == 1)
            {
                gameObject.transform.position -= gameObject.transform.right * speed * Time.deltaTime;
				if (Vector3.Distance(gameObject.transform.position, StarObject.transform.position)>Vector3.Distance(NextStarObject.transform.position,StarObject.transform.position))
                {
                    StarObject = NextStarObject;
                    gameObject.transform.position = StarObject.transform.position;
					if (gameObject.transform.eulerAngles.z > 270 || gameObject.transform.eulerAngles.z < 90) {
						gameObject.transform.eulerAngles = new Vector3 (0,0,0);
					} else {
						gameObject.transform.eulerAngles = new Vector3 (0,0,180);
					}
                    Main.SetColor(StarObject, numberA + 1);
					Main.step++;
                }
            }
            if(Main.step == 2)
            {
                Vector3 position0 = line[line.Count - 1].lineRenderer.GetPosition(0);
                for (int n = line.Count - 3; n >= 0; n--)
                {
                    Vector3 intersectionPoint;
                    if (IntersectionPoint(line[line.Count - 1].lineRenderer.GetPosition(0), line[line.Count - 1].lineRenderer.GetPosition(1), line[n].lineRenderer.GetPosition(0), line[n].lineRenderer.GetPosition(1)) != null)
                    {
                        intersectionPoint = IntersectionPoint(line[line.Count - 1].lineRenderer.GetPosition(0), line[line.Count - 1].lineRenderer.GetPosition(1), line[n].lineRenderer.GetPosition(0), line[n].lineRenderer.GetPosition(1)) ?? Vector3.zero;
                        List<Main.Line> loop = new List<Main.Line>();
                        loop.Add(new Main.Line(Instantiate(LinePrefab).GetComponent<LineRenderer>(), intersectionPoint, line[n].lineRenderer.GetPosition(1), 0));
                        line[n].lineRenderer.SetPosition(1, intersectionPoint);
                        for(int m = n + 1; m < line.Count - 1;m += 0){
                            loop.Add(new Main.Line(Instantiate(LinePrefab).GetComponent<LineRenderer>(), line[m].lineRenderer.GetPosition(0), line[m].lineRenderer.GetPosition(1), 0));
                            Destroy(line[m].lineRenderer.gameObject);
                            line.RemoveAt(m);
                        }
                        loop.Add(new Main.Line(Instantiate(LinePrefab).GetComponent<LineRenderer>(), loop[loop.Count - 1].lineRenderer.GetPosition(1), loop[0].lineRenderer.GetPosition(0), 0));
                        if(Vector2.Distance(line[line.Count - 1].lineRenderer.GetPosition(1),intersectionPoint) < Vector2.Distance(line[line.Count - 1].lineRenderer.GetPosition(1), position0)) position0 = intersectionPoint;
                        StartCoroutine("ChangeColor", loop);
                    }
                }
                line[line.Count - 1].lineRenderer.SetPosition(0, position0);
                if (line.Count > 1)
                {
                    if (line[line.Count - 2].lineRenderer.GetPosition(1) != position0) StartCoroutine("Eraselines", line);
                }
                int count = 0;
                for (int n = 0; n < Main.MaxStar; n++) {
					if (CheckStar (n,numberB) == true)
						count++;
                }
                if (count == 0)
                {
                    Main.step++;
                    StartCoroutine("Finish");
                }
                else
                {
                    Main.turn = numberB;
                    Main.step = 0;
                }
            }
            Main.character[numberA].StarObject = StarObject;
            Main.character[numberA].NextStarObject = NextStarObject;
            Main.line[numberA] = line;
        }
		if(gameObject.transform.eulerAngles.z > 270 || gameObject.transform.eulerAngles.z< 90){
			gameObject.transform.localScale = new Vector3 (0.1f,0.1f,1);
		}else{
			gameObject.transform.localScale = new Vector3 (0.1f,-0.1f,1);;
		}
    }
    public static Vector3? IntersectionPoint(Vector3 point1,Vector3 point2,Vector3 point3,Vector3 point4)
    {
        float a1 = 0, b1 = 0, a2 = 0, b2 = 0;
        float x = 100;
        float y = 100;
        if (point2.x != point1.x)
        {
            a1 = (point2.y - point1.y) / (point2.x - point1.x);
            b1 = point1.y - (a1 * point1.x);
        }
        else
        {
            x = point1.x;
        }
        if (point4.x - point3.x != 0)
        {
            a2 = (point4.y - point3.y) / (point4.x - point3.x);
            b2 = point3.y - (a2 * point3.x);
        }
        else
        {
            if (x != 100) return null;
            x = point3.x;
        }
        if (a1 == a2) return null;
        if (x == 100) x = (b2 - b1) / (a1 - a2);
        y = a1 * x + b1;
        if ((x < point1.x != x < point2.x) && (x < point3.x != x < point4.x) && (y < point1.y != y < point2.y) && (y < point3.y != y < point4.y)) {
            return new Vector3(x, y, 0);
        }
        else
        {
            return null;
        }
    }
    bool IsInside(Vector3 point,List<Main.Line> loop)
    {
        Retry:
        int count = 0;
        for(int n = 0;n < loop.Count; n++)
        {
            Vector3 LinePosition = loop[0].lineRenderer.GetPosition(0) + ((loop[0].lineRenderer.GetPosition(1) - loop[0].lineRenderer.GetPosition(0)) / 2);
            LinePosition = point + ((LinePosition - point) * 1000);
            if (IntersectionPoint(point, LinePosition, loop[n].lineRenderer.GetPosition(0), loop[n].lineRenderer.GetPosition(1)) != null)
            {
                count++;
            }
            else if (n == 0)
            {
                loop.Add(loop[0]);
                loop.RemoveAt(0);
                goto Retry;
            }
        }
        if(count % 2 == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
	public static bool CheckStar(int n,int number){
		GameObject obj = new GameObject();
		if (Main.GetColor(Main.star[n]) == 0)
		{
			obj.transform.position = Main.character[number].gameObject.transform.position;
			obj.transform.LookAt(Main.star[n].transform.position);
			while (Vector2.Distance(obj.transform.position, Main.star[n].transform.position) >= 0.2f)
			{
				obj.transform.position += obj.transform.forward * 0.1f;
				for (int m = 0; m < Main.MaxStar; m++)
				{
					if (m != n && Main.star[m] != Main.character[number].StarObject && Vector2.Distance(obj.transform.position, Main.star[m].transform.position) < 0.2f)
					{
						goto Hit;
					}
				}
			}
			Destroy(obj);
			return true;
			Hit:;
		}
		Destroy(obj);
		return false;
	}
    IEnumerator ChangeColor(List<Main.Line> loop)
    {
        for(int n = 0; n < Main.MaxStar; n++)
        {
            if(IsInside(Main.star[n].transform.position,loop) != IsInside(Main.character[numberB].gameObject.transform.position, loop))
            {
                Main.SetColor(Main.star[n],numberA + 1);
            }
        }
        yield return new WaitForSeconds(0.2f);
        while(loop.Count > 0)
        {
            Destroy(loop[0].lineRenderer.gameObject);
            loop.RemoveAt(0);
        }
    }
    IEnumerator Eraselines(List<Main.Line> line)
    {
        for (int n = 0; n < line.Count - 1; n++)
        {
			line[n].lineRenderer.SetColors(Main.lineColor[0],Main.lineColor[0]);
        }
            yield return new WaitForSeconds(0.2f);
        while(line.Count > 1)
        {
            Destroy(line[0].lineRenderer.gameObject);
            line.RemoveAt(0);
        }
    }
    IEnumerator Finish()
    {
        GameObject.Find("Canvas1").GetComponent<Canvas>().enabled = true;
        yield return new WaitForSeconds(2f);
        GameObject.Find("Finish").GetComponent<Text>().enabled = false;
		GameObject.Find ("Canvas0").GetComponent<Canvas> ().enabled = false;
        int score0 = 0;
        int score1 = 0;
        for(int n = 0; n < Main.MaxStar; n++)
        {
            if(Main.GetColor(Main.star[n]) == 1)
            {
                score0++;
            }
            if (Main.GetColor(Main.star[n]) == 2)
            {
                score1++;
            }
        }
        GameObject.Find("score0").GetComponent<Text>().enabled = true;
        GameObject.Find("score1").GetComponent<Text>().enabled = true;
		for (int n = 0; n < Mathf.Max(score0,score1); n++)
        {
			if (GameObject.Find ("score0").GetComponent<Text> ().text != score0.ToString ())
				GameObject.Find ("score0").GetComponent<Text> ().text = "" + (int.Parse (GameObject.Find ("score0").GetComponent<Text> ().text) + 1);
			if(GameObject.Find("score1").GetComponent<Text>().text != score1.ToString())
				GameObject.Find ("score1").GetComponent<Text> ().text = "" + (int.Parse (GameObject.Find ("score1").GetComponent<Text> ().text) + 1);
				
            yield return new WaitForSeconds(0.05f);
        }
		yield return new WaitForSeconds(3f);
		if (PlayerPrefs.GetInt ("level") == Main.level)
			PlayerPrefs.SetInt ("level",Main.level + 1);
		if (score0 > PlayerPrefs.GetInt ("HighScore"+ Main.level))
			PlayerPrefs.SetInt ("HighScore" + Main.level, score0);
		PlayerPrefs.Save ();
        SceneManager.LoadScene(0);
    }
}
