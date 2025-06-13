using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : MonoBehaviour
{
	public GameObject LinePrefab;
	int numberA, numberB;
	GameObject StarObject;
	GameObject NextStarObject;
	List<Main.Line> line;
	
	void Start ()
	{
		numberA = int.Parse (gameObject.name);
		numberB = 1 - numberA;
	}
	
	void Update ()
	{
		if (Main.character [numberA].level > 0 && Main.turn == numberA) {
			StarObject = Main.character [numberA].StarObject;
			NextStarObject = Main.character [numberA].NextStarObject;
			line = Main.line [numberA];
			if (Main.step == 0) {
				if (line.Count == 0) {
					NextStarObject = Main.star [FarStar ()];
					if (Main.character [numberA].level < 2) {
						int r = Random.Range (0, Main.MaxStar);
						while (Move.CheckStar (r, numberA) == false) {
							r = Random.Range (0, Main.MaxStar);
						}
						NextStarObject = Main.star [r];
					}
				} else {
					int score = CountScore (null, numberA);
					if (Main.character [numberA].level == 3) {
						float distance = 0;
						for (int n = 0; n < Main.MaxStar; n++) {
							if (Move.CheckStar (n, numberA)) {
								int total = CountScore (n, numberA);
								if (total > score || (NextStarObject != StarObject && total >= score && Vector3.Distance (gameObject.transform.position, Main.star [n].transform.position) > distance)) {
									score = total;
									NextStarObject = Main.star [n];
									distance = Vector3.Distance (gameObject.transform.position, Main.star [n].transform.position);
								}
							}
						}
					}
					if (NextStarObject == StarObject) {
						NextStarObject = Main.star [FarStar ()];
						if (Main.character [numberA].level == 2) {
							score = 0;
							for (int n = 0; n < Main.MaxStar; n++) {
								if (Move.CheckStar (n, numberA)) {
									if (CountScore (n, numberA) > score) {
										NextStarObject = Main.star [n];
										score = CountScore (n, numberA);
									}
								}
							}
						}
						if (Main.character [numberA].level == 1) {
							score = 100;
							for (int n = 0; n < Main.MaxStar; n++) {
								if (Move.CheckStar (n, numberA)) {
									if (CountScore (n, numberA) < score) {
										NextStarObject = Main.star [n];
										score = CountScore (n, numberA);
									}
								}
							}
						}
						int intersectionPoint = 100;
						float distance = 0;
						for (int n = 0; n < Main.MaxStar; n++) {
							if (Move.CheckStar (n, numberA)) {
								for (int m = 0; m < line.Count - 1; m++) {
									if (Move.IntersectionPoint (gameObject.transform.position, Main.star [n].transform.position, line [m].lineRenderer.GetPosition (0), line [m].lineRenderer.GetPosition (1)) != null) {
										if (Main.character [numberA].level != 3 || (m < intersectionPoint || (m == intersectionPoint && Vector3.Distance (gameObject.transform.position, Main.star [n].transform.position) > distance))) {
											if (Main.character [numberA].level != 2 || CountScore (m, numberA) > 3) {
												if (Main.character [numberA].level != 1 || (CountScore (m, numberA) < 3 && CountScore(m,numberA) != 0)) {
													intersectionPoint = m;
													NextStarObject = Main.star [n];
													distance = Vector3.Distance (gameObject.transform.position, Main.star [n].transform.position);
												}
											}
										}
									}
								}
							}
						}
					}
				}
				gameObject.transform.rotation = Quaternion.LookRotation (Vector3.forward, NextStarObject.transform.position - gameObject.transform.position);
				gameObject.transform.Rotate (new Vector3 (0, 0, -90));
				line.Add (new Main.Line (Instantiate (LinePrefab).GetComponent<LineRenderer> (), StarObject.transform.position, NextStarObject.transform.position, numberA + 1));
				line [line.Count - 1].lineRenderer.gameObject.transform.parent = gameObject.transform;
				Main.step++;
			}
			Main.character [numberA].StarObject = StarObject;
			Main.character [numberA].NextStarObject = NextStarObject;
			Main.line [numberA] = line;
		}
	}

	bool IsInside (Vector3 point, List<Vector3> loop)
	{
		int count = 0;
		for (int n = 0; n < loop.Count - 1; n++) {
			Vector3 LinePosition = loop [0] + ((loop [1] - loop [0]) / 2);
			LinePosition = point + ((LinePosition - point) * 1000);
			if (Move.IntersectionPoint (point, LinePosition, loop [n], loop [n + 1]) != null) {
				count++;
			}
		}
		if (count % 2 == 0) {
			return false;
		} else {
			return true;
		}
	}

	int CountScore (int? n, int number)
	{
		line = Main.line [number];
		List<Vector3> loop = new List<Vector3> ();
		loop.Add (line [0].lineRenderer.GetPosition (0));
		for (int m = 0; m < line.Count; m++) {
			loop.Add (line [m].lineRenderer.GetPosition (1));
		}
		if (n != null) {
			loop.Add (Main.star [n ?? 0].transform.position);
		}
		int count = 0;
		for (int m = 0; m < loop.Count - 3; m++) {
			if (Move.IntersectionPoint (loop [m], loop [m + 1], loop [loop.Count - 1], loop [loop.Count - 2]) != null)
				count++;
		}
		int score = 0;
		if (count == 0) {
			loop.Add (loop [0]);
			for (int m = 0; m < Main.MaxStar; m++) {
				if (IsInside (Main.star [m].transform.position, loop) != IsInside (Main.character [1 - number].gameObject.transform.position, loop)) {
					if (Main.GetColor (Main.star [m]) == 0)
						score += 1;
					if (Main.GetColor (Main.star [m]) == 1 - number + 1)
						score += 2;
				}
			}
		} else {
			for (int l = loop.Count - 4; l >= 0; l--) {
				Vector3 intersectionPoint;
				if (Move.IntersectionPoint (loop [loop.Count - 1], loop [loop.Count - 2], loop [l], loop [l + 1]) != null) {
					intersectionPoint = Move.IntersectionPoint (loop [loop.Count - 1], loop [loop.Count - 2], loop [l], loop [l + 1]) ?? Vector3.zero;
					List<Vector3> loop1 = new List<Vector3> ();
					loop1.Add (intersectionPoint);
					loop1.Add (loop [l + 1]);
					loop [l + 1] = intersectionPoint;
					for (int m = l + 2; m < loop.Count - 1; m += 0) {
						loop1.Add (loop [m]);
						loop.RemoveAt (m);
					}
					loop1.Add (intersectionPoint);
					for (int m = 0; m < Main.MaxStar; m++) {
						if (IsInside (Main.star [m].transform.position, loop1) != IsInside (Main.character [1 - number].gameObject.transform.position, loop1)) {
							if (Main.GetColor (Main.star [m]) == 0)
								score += 1;
							if (Main.GetColor (Main.star [m]) == 1 - number + 1)
								score += 2;
						}
					}
				}
			}
		}

		if (Main.character [number].level == 3) {
			Vector3 pos = transform.position;
			if (n != null) {
				transform.position = Main.star [n ?? 0].transform.position;
			}
			int maxScore = 0;
			for (int m = 0; m < Main.MaxStar; m++) {
				if (Move.CheckStar (m, 1 - number)) {
					maxScore = Mathf.Max(CountScore (m,1 - number),maxScore);
				}
			}
			transform.position = pos;
			score -= maxScore;
		}

		line = Main.line [number];
		return score;
	}

	int FarStar ()
	{
		float distance = 0;
		int farStar = 0;
		for (int n = 0; n < Main.MaxStar; n++) {
			if (Move.CheckStar (n, numberA)) {
				if (Vector3.Distance (gameObject.transform.position, Main.star [n].transform.position) > distance) {
					farStar = n;
					distance = Vector3.Distance (gameObject.transform.position, Main.star [n].transform.position);
				}
			}
		}
		return farStar;
	}
}
