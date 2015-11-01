using UnityEngine;
using System.Collections;

public class BirdTarget : MonoBehaviour {
	[SerializeField] private int _flockSize = 5;
	[SerializeField] private GameObject _birdPrefab;
	[SerializeField] private Vector3[] _path;
	[SerializeField] private float _sqrPositionEps = 0.2f; // Squared threshold for checking if we reached a node.
	private int _targetNode = 0;

	public Vector3[] Path { get { return _path; }}
	public float Speed = 1.0f;

	[SerializeField] private Vector3 _currentDir;
	public Vector3 CurrentDir {
		get { return _currentDir; }
		set {_currentDir = value; }
	}
	
	// Flock properties
	public float DirChangeDeltaTime = 1.0f; // Time after which random direction changes should occur

	[SerializeField] private float _SqrMinDistance = 1.0f;
	public float SqrMinDistance {
		get { return _SqrMinDistance; }
	}
	public float MinDistance {
		get { return Mathf.Sqrt (_SqrMinDistance); }
		set { _SqrMinDistance = value * value; }
	}

	[SerializeField] private float _SqrMaxDistance = 5.0f;
	public float SqrMaxDistance {
		get {return _SqrMaxDistance; }
	}
	public float MaxDistance {
		get { return Mathf.Sqrt (_SqrMaxDistance); }
		set { _SqrMaxDistance = value * value; }
	}

	private void TargetNextNode () {
		_targetNode = (_targetNode + 1) % _path.Length;
		CurrentDir = (_path[_targetNode] - transform.position).normalized;
	}

	private void CreateFlock () {
		for (int i = 0; i < _flockSize; i++) {
			Vector3 position = transform.position;
			position.x += Random.Range (-5f, 5f);
			position.y += Random.Range (-5f, 5f);
			position.z += Random.Range (-5f, 5f);
			GameObject newBird = (GameObject)Instantiate (_birdPrefab, position, transform.rotation);
			newBird.GetComponent<BirdBehaviour> ().Target = gameObject;
		}
	}

	void Start () {
		if (_path.Length < 2) {
			throw new UnityException ("Not enough path nodes!");
		}

		transform.position = _path[0];
		TargetNextNode ();
		CreateFlock ();
	}

	void FixedUpdate () {
		if ((_path[_targetNode] - transform.position).sqrMagnitude < _sqrPositionEps) {
			TargetNextNode ();
		}
		transform.position += CurrentDir * Speed * Time.fixedDeltaTime;
	}
}
