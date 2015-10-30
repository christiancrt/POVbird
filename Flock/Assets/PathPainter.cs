using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BirdTarget))]
public class PathPainter : MonoBehaviour {
	private BirdTarget _birdTarget;

	// Use this for initialization
	void Start () {
		_birdTarget = GetComponent<BirdTarget> ();
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < _birdTarget.Path.Length; i++) {
			Debug.DrawLine (_birdTarget.Path[i],
			                _birdTarget.Path[(i + 1) % _birdTarget.Path.Length],
			                Color.black);
		}
	}
}
