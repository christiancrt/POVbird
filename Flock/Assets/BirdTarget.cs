using UnityEngine;
using System.Collections;

public class BirdTarget : MonoBehaviour {
	[SerializeField]
	private int _flockSize = 5;
	[SerializeField]
	private GameObject _birdPrefab;
	[SerializeField] private Vector3[] _path;
	public Vector3[] Path { get { return _path; }}
	private int _targetNode = 0;
	public float Speed = 1.0f;
	public Vector3 CurrentDir;
	private float _sqrPositionEps = 0.2f; // Squared threshold for checking if we reached a node.


	// Flock properties
	public float DirChangeDeltaTime = 1.0f; // Time after which random direction changes should occur
	public float MinDistance = 1f;
	public float MaxDistance = 5f;
	public float MaxHeadingDeviation = 30f; // degrees
	public float MaxSpeedDeviation = 0.5f;
	
	void NextTargetNode () {
		_targetNode = (_targetNode + 1) % _path.Length;
		CurrentDir = (_path[_targetNode] - transform.position).normalized;
	}

	// Use this for initialization
	void Start () {
		if (_path.Length < 2) {
			throw new UnityException ("Not enough path nodes!");
		}

		transform.position = _path[0];
		NextTargetNode ();
		
		for (int i = 0; i < _flockSize; i++) {
			Vector3 position = transform.position;
			position.x += Random.Range (-5f, 5f);
			position.y += Random.Range (-5f, 5f);
			position.z += Random.Range (-5f, 5f);
			GameObject newBird = (GameObject)Instantiate (_birdPrefab, position, transform.rotation);
			newBird.GetComponent<BirdBehaviour> ().Target = gameObject;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if ((_path[_targetNode] - transform.position).sqrMagnitude < _sqrPositionEps) {
			NextTargetNode ();
		}
		transform.position += CurrentDir * Speed * Time.fixedDeltaTime;
	}
}
