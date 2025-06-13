using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public GameObject LinePrefab;
    int numberA,numberB;
    GameObject StarObject;
    GameObject NextStarObject;
    GameObject NewLine;
    List<Main.Line> line;
    void Start () {
        numberA = int.Parse(gameObject.name);
        numberB = 1 - numberA;
        NewLine = gameObject.transform.Find("NewLine").gameObject;
		NewLine.GetComponent<LineRenderer>().SetColors(Main.lineColor[numberA + 1],Main.lineColor[numberA + 1]);
    }
	
	void Update () {
		if (Main.character[numberA].level == 0 && Main.turn == numberA && Time.timeScale == 1) {
            StarObject = Main.character[numberA].StarObject;
            NextStarObject = Main.character[numberA].NextStarObject;
            line = Main.line[numberA];
            if (Main.step == 0) {
                NewLine.transform.position = gameObject.transform.position;
                Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                position.z = 0;
                NewLine.transform.LookAt(position);
                if(NextStarObject != null)NextStarObject.transform.localScale = new Vector3(0.18f,0.18f,0.18f);
                NextStarObject = null;
                while(Mathf.Abs(NewLine.transform.position.x) < 10 && Mathf.Abs(NewLine.transform.position.y) < 10)
                {
                    NewLine.transform.position += NewLine.transform.forward * 0.1f;
                    for (int n = 0; n < Main.MaxStar;n++)
                    {
                        if (Vector2.Distance(NewLine.transform.position,Main.star[n].transform.position) < 0.2f && gameObject.transform.position != Main.star[n].transform.position)
                        {
                            if (Main.GetColor(Main.star[n]) == 0) {
                                NextStarObject = Main.star[n];
                                NextStarObject.transform.localScale = new Vector3(0.22f, 0.22f, 0.22f);
                                NewLine.transform.position = NextStarObject.transform.position;
                            }
                            goto draw;
                        }
                    }
                }
                draw:
                NewLine.GetComponent<LineRenderer>().SetPositions(new Vector3[] {gameObject.transform.position,NewLine.transform.position});
				if (Input.GetMouseButtonDown(0) && Main.pointerEnter == false && NextStarObject != null)
                {
					NextStarObject.transform.localScale = new Vector3(0.18f, 0.18f, 0.18f);
                    gameObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, NextStarObject.transform.position - gameObject.transform.position);
					gameObject.transform.Rotate (new Vector3(0,0,-90));
					NewLine.GetComponent<LineRenderer> ().positionCount = 0;
					NewLine.GetComponent<LineRenderer> ().positionCount = 2;
                    line.Add(new Main.Line(Instantiate(LinePrefab).GetComponent<LineRenderer>(),StarObject.transform.position,NextStarObject.transform.position,numberA + 1));
                    line[line.Count - 1].lineRenderer.gameObject.transform.parent = gameObject.transform;
                    Main.step++;
                }
			}
            Main.character[numberA].StarObject = StarObject;
            Main.character[numberA].NextStarObject = NextStarObject;
            Main.line[numberA] = line;
        }
	}
}
