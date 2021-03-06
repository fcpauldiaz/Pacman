﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class pacman : MonoBehaviour {

	public GameObject pacm;
	public bool firstPersonView = false;
	public float moveSpeed = 10000f;
	public float rotationSpeed = 2.0f;
	public float horizontalSpeed = 50F;
	public float verticalSpeed = 2.0F;
	public GameObject cameraPlayer;
	public GameObject cameraMenu;
	public Button newGame;
	public GameObject menu;
	public GameObject win;
	public GameObject loose;
	public Button startButton;
	public Button newGameButton;
	public GameObject[] ghosts;
	public static bool finish = false;
	public bool gamePaused = true;
	public bool inversePlay = false;
	public static bool resetGame = false;
	public static bool winner = false;
	public static bool looser = false;
	public int score;
	public float targetTime = 0.2f;
	public float rBase = 180f;
	public static float stepFollow = 50f;
	public static float stepAway = 1.2f;
	public float staticYPacm;
	public AudioSource[] allAudio;
	public float speed = 1.0f;

	// Use this for initialization
	void Start () {
		gamePaused = true;
		pacm = GameObject.Find ("pacman");
		cameraPlayer = GameObject.Find ("Camera");
		cameraMenu = GameObject.Find ("CameraMenu");
		menu = GameObject.Find ("MainMenu");
		ghosts = GameObject.FindGameObjectsWithTag ("ghost");
		cameraPlayer.SetActive (false);
		startButton = GameObject.Find("Continue").GetComponent<Button>();
		newGameButton = GameObject.Find ("Newgame").GetComponent<Button> ();
		allAudio  = GetComponents<AudioSource>();
		startButton.onClick.AddListener(startOnClick);
		newGameButton.onClick.AddListener (newGameClick);
		win = GameObject.Find ("WinnerPannel");
		win.SetActive (winner);
		loose = GameObject.Find ("LooserPanel");
		loose.SetActive (looser);
		looser = false;
		winner = false;
		if (finish == true) {
			stepAway += 2f;
			stepFollow += 30f; 

		}

		menu = GameObject.Find ("MainMenu");
		if (resetGame == true) {
			startOnClick ();
		}
		resetGame = false;
		staticYPacm = pacm.transform.position.y;

	}
		
	
	// Update is called once per frame
	void Update () {

		// PS4 Rectangle button
		if (Input.GetKeyDown ("joystick button 0")){
			firstPersonView = !firstPersonView;
			if (firstPersonView == true) {
				
				//Camera.main.transform.Translate(Vector3.forward * 25f);  
				Vector3 fwrd = new Vector3 (-0.01990253f, 0.2234014f, 0.1f);
				Camera.main.transform.localPosition = fwrd;

			} else {
				//Camera.main.transform.Translate(Vector3.back * 25f);  
				Vector3 bkw = new Vector3 (1.583f, 0.6799f, 0.1f);
				Camera.main.transform.localPosition = bkw;

			}


		}
		if (gamePaused == false) {

			foreach (GameObject gh in ghosts)
			{
				// pacman runs away from ghosts
				if (inversePlay == false) {
					float step = (stepFollow + Random.Range(-20.0f, 30.0f)) * Time.deltaTime;
					//gh.transform.Rotate(0, 0 , 180 + Random.Range(-30.0f, 30.0f));
					gh.transform.position = Vector3.MoveTowards(gh.transform.position, pacm.transform.position, step);
				}
				// ghosts run away from pacman
				else {
					gh.transform.LookAt (pacm.transform.position);


					if (gh.transform.position.x >= 520f || gh.transform.position.x <= -368f
					    || gh.transform.position.z >= 205f || gh.transform.position.z <= -290f) {
						rBase = 90f;
					} else if (gh.transform.position.x <= 520f || gh.transform.position.x >= -350f
					            || gh.transform.position.z <= 190f || gh.transform.position.z >= -263f) { 
						rBase = 180f;
					}
					else {
						//rBase = 180f;
					}

					gh.transform.Rotate(0, rBase + Random.Range(-40.0f, 40.0f) , 0); 
					gh.transform.Translate(new Vector3(0, 0, stepAway));
				}

				Vector3 temp2 = gh.transform.position; // copy to an auxiliary variable...
				Quaternion temp22 = gh.transform.rotation;
				Vector3 temp3 = pacm.transform.position;
				temp2.y = 97f;
				temp22.x = 0f;
				temp22.z = 0f;
				temp3.y = 97f;
				gh.transform.position = temp2;
				gh.transform.rotation = temp22;
				pacm.transform.position = temp3;



			}

			//PS4 OPTIONS BUTTON
			if (Input.GetKeyDown ("joystick button 9")) {
				gamePaused = !gamePaused;
				if (gamePaused == true) {
					cameraMenu.SetActive (true);
					menu.SetActive (true);
					cameraPlayer.SetActive (false);
				} else {
					cameraMenu.SetActive (false);
					menu.SetActive (false);
					cameraPlayer.SetActive (true);
				}


			}
			// PS4 MAIN JOYSTICKS
			float h = horizontalSpeed * Input.GetAxis("PS4_RIGHTANALOG_HORIZONTAL");
			float v = rotationSpeed * Input.GetAxis ("PS4_RIGHTANALOG_VERTICAL");
			pacm.transform.Rotate(0, h, 0);

			float translation = Input.GetAxis("Vertical") * moveSpeed;
			float translationX = Input.GetAxis("Horizontal") * moveSpeed;
			translation *= Time.deltaTime;
			translationX *= Time.deltaTime;
			pacm.transform.Translate(-translation, 0, 0);
			pacm.transform.Translate(0, 0, translationX);
			//camera rotate but not the player.
			cameraPlayer.transform.Rotate (v, 0, 0);

			if (inversePlay == true) {
				targetTime -= Time.deltaTime;
				if (targetTime <= 0.0f) {
					timerEnded();
				}

			}

			if (score >= 30) {
				//player wins or finish this level.
				if (finish == true) {
					allAudio [1].Stop ();
					gamePaused = true;
					cameraMenu.SetActive (true);
					menu.SetActive (true);
					cameraPlayer.SetActive (false);

					winner = true;
					finish = false;
					stepAway = 1.2f;
					stepFollow = 50f; 
					SceneManager.UnloadScene ("Level2");
					SceneManager.LoadScene ("Level1");
				} else {
					
					resetGame = true;
					SceneManager.UnloadScene ("Level1");
					SceneManager.LoadScene ("Level2");
					finish = true;
				}
			}
		}


	}

	//remove menu and start game
	public void startOnClick() {
		menu.SetActive (false);
		cameraMenu.SetActive(false);
		cameraPlayer.SetActive (true);
		gamePaused = false;
		allAudio [0].Stop ();
		allAudio [1].Play ();
		Camera mapCamera = GameObject.Find("MapCamera").GetComponent<Camera>();
		mapCamera.enabled = true;

	}

	public void OnCollisionEnter (Collision col) {
		
		if (col.gameObject.name.Contains ("ghost")) {
			//lost game or one heart
			if (inversePlay == false) {
				allAudio [1].Stop ();
				allAudio [2].Play ();
				gamePaused = true;
				cameraMenu.SetActive (true);
				menu.SetActive (true);
				cameraPlayer.SetActive (false);
				looser = true;
				finish = false;
				stepAway = 1.2f;
				stepFollow = 50f; 
				SceneManager.LoadScene("Level1");
			} else {
				allAudio [3].Play ();
				//earn points for eating a ghost
				score = score + 10;
				//reset ghost position to some standard
				Vector3 temp = col.gameObject.transform.position; // copy to an auxiliary variable...
				temp.x = 82f;
				temp.y = -15f;
				temp.z = -42.6f;
				col.gameObject.transform.position = temp;
			}

		}
		// Physics.IgnoreCollision(col.gameObject.GetComponent<SphereCollider>(), GetComponent<SphereCollider>());

	}

	public void OnTriggerEnter(Collider col) {
		

		if (col.gameObject.name.Contains("apple")) {
			allAudio [5].Play ();
			score = score + 1;
			Destroy(col.gameObject);
		}
		if (col.gameObject.name.Contains("cherry")) {
			
			inversePlay = true;
			allAudio [1].Stop ();
			allAudio [4].Play ();
			Destroy(col.gameObject);
		}

	}

	public void OnGUI(){
		if (gamePaused == false) {
			GUI.Box(new Rect(Screen.width/2-300,Screen.height/2-150,100,25), "Score " + score);
		}
	}

	public void timerEnded() {
		inversePlay = false;
		allAudio [4].Stop ();
		allAudio [1].Play ();
		targetTime = 10f;

	}

	public void newGameClick() {
		resetGame = true;
		stepAway = 1.2f;
		stepFollow = 50f; 
		SceneManager.LoadScene("Level1");


	}
		


}
