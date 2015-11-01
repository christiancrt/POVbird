using UnityEngine;
using System.Collections.Generic;

public class BirdBehaviour : MonoBehaviour {
	private float _lastTime = 0;
	private Vector3 _currentDir;
	private GameObject[] _birds;

	private BirdTarget _birdTarget;
	private GameObject _target = null;
	public GameObject Target {
		get { return _target; } 
		set {
			_target = value;
			_birdTarget = _target.GetComponent<BirdTarget> ();
			_currentDir = _birdTarget.CurrentDir;
		}
	}

	GameObject FindNearestBird () {
		GameObject nearestBird = null;
		float lastDistance = float.PositiveInfinity;

		// Yeah, brute force!
		foreach (GameObject bird in _birds) {
			if (bird != gameObject) {
				float distance = (gameObject.transform.position - bird.transform.position).sqrMagnitude;
		
				if (distance < lastDistance) {
					lastDistance = distance;
					nearestBird = bird;
				}
			}
		}

		return nearestBird;
	}

	Vector3 FindAverageHeading () {
		Vector3 avgHeading = new Vector3 ();

		foreach (GameObject bird in _birds) {
			avgHeading += bird.transform.forward;
		}

		// Calculate mean vector
		avgHeading.x = avgHeading.x / _birds.Length;
		avgHeading.y = avgHeading.y / _birds.Length;
		avgHeading.z = avgHeading.z / _birds.Length;
		return avgHeading.normalized;
	}

	Vector3 FindAveragePosition () {
		Vector3 avgPosition = new Vector3 ();
		
		foreach (GameObject bird in _birds) {
			avgPosition += bird.transform.position;
		}
		
		// Calculate mean vector
		avgPosition.x = avgPosition.x / _birds.Length;
		avgPosition.y = avgPosition.y / _birds.Length;
		avgPosition.z = avgPosition.z / _birds.Length;
		return avgPosition;
	}

	void ApplyBirdBrain () {
		// Rule reference: http://www.lalena.com/AI/Flock/
		// Separation: Steer to avoid crowding birds of the same color.
		// Alignment: Steer towards the average heading of birds of the same color.
		// Cohesion: Steer to move toward the average position of birds of the same color.

		// 1. Find average heading and position of our flock
		// 2. Collect weighted directions for separation, alignment and cohesion
		// 3. Add fellowship: tend towards the bird target that makes the flock follow a given path
		// 4. Combine directions
		// 5. Fly towards new direction

		Vector3 avgHeading = FindAverageHeading ();
		Vector3 avgPosition = FindAveragePosition ();
		GameObject nearestBird = FindNearestBird ();

		// Separation
		Vector3 weightedSeparationDir = Vector3.zero;
		float sqrDistanceToNearest = (nearestBird.transform.position - transform.position).sqrMagnitude;
		if (sqrDistanceToNearest < _birdTarget.SqrMinDistance) {
			// Flee straight away from the nearest bird, but scale that direction by how much our MinDistance was
			// severed.
			weightedSeparationDir = (transform.position - nearestBird.transform.position).normalized
				* sqrDistanceToNearest / _birdTarget.SqrMinDistance;
		}

		// Alignment
		float dirDeviation = Vector3.Angle (transform.forward, avgHeading);
		Vector3 weightedAlignedDir = avgHeading * dirDeviation / 90f; // TODO Fiddle with this

		// Cohesion
		// Same algorithm as separation!
		Vector3 weightedCohesionDir = Vector3.zero;
		float sqrDistanceFromFlock = (transform.position - avgPosition).sqrMagnitude;
		if (sqrDistanceFromFlock > _birdTarget.SqrMaxDistance) {
			weightedCohesionDir = (avgPosition - transform.position).normalized
				* sqrDistanceFromFlock / _birdTarget.SqrMaxDistance;
		}

		// Fellowship
		// Same algorithm as separation/cohesion!
		Vector3 weightedFellowshipDir = Vector3.zero;
		float sqrDistanceFromTarget = (transform.position - _birdTarget.transform.position).sqrMagnitude;
		if (sqrDistanceFromTarget > _birdTarget.SqrMaxDistance) {
			weightedFellowshipDir = (_birdTarget.transform.position - transform.position).normalized
				* sqrDistanceFromTarget / _birdTarget.SqrMaxDistance;
		}

//		Vector3 weightedCompanionCube //... Wait. No.
		_currentDir = (weightedSeparationDir
		               + weightedAlignedDir
		               + weightedCohesionDir
		               + weightedFellowshipDir).normalized;
	}

	void Start () {
		_lastTime = Time.time;
		
		_birds = GameObject.FindGameObjectsWithTag ("Bird");
		if (_birds.Length < 1) {
			throw new UnityException ("No birds found! Noes!");
		}
	}

	void FixedUpdate () {
		// Make direction changes a little more asynchronous
		var offset = Random.Range (-0.5f * _birdTarget.DirChangeDeltaTime, 0.5f * _birdTarget.DirChangeDeltaTime);
		if (Time.time - _lastTime > _birdTarget.DirChangeDeltaTime + offset) {
			_currentDir = _birdTarget.CurrentDir;
			ApplyBirdBrain ();
			_lastTime = Time.time;
		}

		var interpolatedDir = Vector3.Lerp (transform.forward,
		                                    _currentDir,
		                                    Time.fixedDeltaTime * Random.Range (0.1f, 10f));
		transform.LookAt (transform.position + interpolatedDir);
		transform.position += transform.forward * _birdTarget.Speed * Time.fixedDeltaTime;
	}
}
