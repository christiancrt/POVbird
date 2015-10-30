using UnityEngine;
using System.Collections.Generic;

public class BirdBehaviour : MonoBehaviour {
	private float _lastTime = 0;
	private float _speed;
	private Vector3 _currentDir;

	private GameObject _target = null;
	public GameObject Target { get { return _target; } set { _target = value; } }
	private GameObject[] _birds;
	private BirdTarget _birdTarget;

	GameObject FindNearestBird () {
		GameObject nearestBird = null;
		float lastDistance = float.PositiveInfinity;

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
		// http://www.lalena.com/AI/Flock/
		// Separation: Steer to avoid crowding birds of the same color.
		// Alignment: Steer towards the average heading of birds of the same color.
		// Cohesion: Steer to move toward the average position of birds of the same color.

		// 1. Find average heading and position of our flock
		// 2. Collect weighted directions for separation, alignment and cohesion
		// 3. Add fellowship: tend towards the bird target that makes the flock follow a given path
		// 4. Combine directions
		// 5. Fly towards new direction

		// TODO Small optimization: instead of using Vector3.magnitude here, square user-defined distance values.

		Vector3 avgHeading = FindAverageHeading ();
		Vector3 avgPosition = FindAveragePosition ();
		GameObject nearestBird = FindNearestBird ();

		// Separation
		Vector3 weightedSeparationDir = Vector3.zero;
		float distanceToNearest = (nearestBird.transform.position - transform.position).magnitude;
		if (distanceToNearest < _birdTarget.MinDistance) {
			// Flee straight away from the nearest bird, but scale that direction by how much our MinDistance was
			// severed.
			weightedSeparationDir = (transform.position - nearestBird.transform.position).normalized
				* distanceToNearest / _birdTarget.MinDistance;
		}

		// Alignment
		float dirDeviation = Vector3.Angle (transform.forward, avgHeading);
		// Fiddle with this.
		Vector3 weightedAlignedDir = avgHeading * dirDeviation / 90f;

		// Cohesion
		// Same algorithm as separation!
		Vector3 weightedCohesionDir = Vector3.zero;
		float distanceFromFlock = (transform.position - avgPosition).magnitude;
		if (distanceFromFlock > _birdTarget.MaxDistance) {
			weightedCohesionDir = (avgPosition - transform.position).normalized
				* distanceFromFlock / _birdTarget.MaxDistance;
		}

		// Fellowship
		// Same algorithm as separation/cohesion!
		Vector3 weightedFellowshipDir = Vector3.zero;
		float distanceFromTarget = (transform.position - _birdTarget.transform.position).magnitude;
		if (distanceFromTarget > _birdTarget.MaxDistance) {
			weightedFellowshipDir = (_birdTarget.transform.position - transform.position).normalized
				* distanceFromTarget / _birdTarget.MaxDistance;
		}

//		Vector3 weightedCompanionCube //... Wait. No.
		_currentDir = (weightedSeparationDir
		               + weightedAlignedDir
		               + weightedCohesionDir
		               + weightedFellowshipDir).normalized;
	}

	void Start () {
		_lastTime = Time.time;
		_birdTarget = _target.GetComponent<BirdTarget> ();
		_currentDir = _birdTarget.CurrentDir;
		_speed = _birdTarget.Speed;
		
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

		transform.position += _currentDir * _speed * Time.fixedDeltaTime;
		transform.LookAt (transform.position + _currentDir);
	}
}
