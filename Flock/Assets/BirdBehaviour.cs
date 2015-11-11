using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class BirdBehaviour : MonoBehaviour {
	private float _lastTime = float.NegativeInfinity;
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
		avgHeading.x /= _birds.Length;
		avgHeading.y /= _birds.Length;
		avgHeading.z /= _birds.Length;
		return avgHeading.normalized;
	}

	Vector3 FindAveragePosition () {
		Vector3 avgPosition = new Vector3 ();
		
		foreach (GameObject bird in _birds) {
			avgPosition += bird.transform.position;
		}
		
		// Calculate mean vector
		avgPosition.x /= _birds.Length;
		avgPosition.y /= _birds.Length;
		avgPosition.z /= _birds.Length;
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

	string Vec3ToPov (Vector3 v) {
		return "<" + v.x + ", " + v.y + ", " + v.z + ">";
	}

	void InitPovFile () {
		if (File.Exists (@"../flock.pov")) {
			File.Delete (@"../flock.pov");
		}

		string clockString = "// clock * recordingTime + startTime\n"
			  + "#declare birdRecClock = clock * " + _birdTarget.RecordingTime + " + " + Time.fixedTime + ";\n";
		System.IO.File.WriteAllText (@"../flock.pov", clockString); // We can assume the file doesn't exist...
	}

	void PrintPovData () {
		string povPosition = Vec3ToPov(transform.position);
		string povDirSpeed = Vec3ToPov(transform.forward * _birdTarget.Speed * Time.fixedDeltaTime);

		// (birdRecClock - startTime) / (endTime - startTime)
		float startTime = _lastTime;
		float endTime = Time.fixedTime;

		string povString = "#if (birdRecClock >= " + startTime + " & birdRecClock < " + endTime + ")\n"
			+ "sphere {\n"
			+ "\t" + povPosition + " + (" + povDirSpeed + " * (birdRecClock - " + startTime + ") / " + (endTime - startTime) + ") .5\n"
			+ "	pigment {\n"
			+ "		rgb <1, 0, 0>\n"
			+ "	}\n"
			+ "}\n"
		    + "#end\n";

		// The file must exist at this point
		using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"../flock.pov", true)) {
			file.WriteLine(povString);
		}
	}

	void Start () {
		_lastTime = Time.fixedTime;
		
		_birds = GameObject.FindGameObjectsWithTag ("Bird");
		if (_birds.Length < 1) {
			throw new UnityException ("No birds found! Noes!");
		}

		InitPovFile ();
	}

	void FixedUpdate () {
		// Make direction changes a little more asynchronous
		var offset = Random.Range (-0.5f * _birdTarget.DirChangeDeltaTime, 0.5f * _birdTarget.DirChangeDeltaTime);
		if (Time.fixedTime - _lastTime > _birdTarget.DirChangeDeltaTime) {// + offset) {
//			_currentDir = _birdTarget.CurrentDir;
			ApplyBirdBrain ();
			if (_lastTime < _birdTarget.RecordingTime)
				PrintPovData ();
			_lastTime = Time.fixedTime;
		}

//		var interpolatedDir = Vector3.Lerp (transform.forward,
//		                                    _currentDir,
//		                                    Time.fixedDeltaTime * Random.Range (0.1f, 10f));
		var interpolatedDir = _currentDir;
		transform.LookAt (transform.position + interpolatedDir);
		transform.position += transform.forward * _birdTarget.Speed * Time.fixedDeltaTime;
	}
}
