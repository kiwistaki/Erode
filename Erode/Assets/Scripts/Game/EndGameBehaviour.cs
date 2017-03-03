using UnityEngine;

public class EndGameBehaviour : MonoBehaviour {

    private GameManager _gameManager;

	// Use this for initialization
	void Start () {
        _gameManager = GameObject.Find("MainCamera").GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            _gameManager.GameOverRequest();
        }
    }
}
