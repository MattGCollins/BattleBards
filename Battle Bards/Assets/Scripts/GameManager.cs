using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Tutorial2D
{
	using System.Collections.Generic;       //Allows us to use Lists. 

	public class GameManager : MonoBehaviour
	{

		public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
		private BoardManager boardScript;                       //Store a reference to our BoardManager which will set up the level.
		private int level = 3;                                  //Current level number, expressed in game as "Day 1".
        public Vector3 playerPos;

		//Awake is always called before any Start functions
		void Awake()
		{
			//Check if instance already exists
			if (instance == null)

				//if not, set instance to this
				instance = this;

			//If instance already exists and it's not this:
			else if (instance != this)

				//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
				Destroy(gameObject);    

			//Sets this to not be destroyed when reloading scene
			DontDestroyOnLoad(gameObject);

			//Get a component reference to the attached BoardManager script
			boardScript = GetComponent<BoardManager>();

			//Call the InitGame function to initialize the first level 
			InitOverworld();
		}

        void OnLevelWasLoaded(int index)
        {
            Scene scene = SceneManager.GetActiveScene();
            //Call InitGame to initialize our level.
            if (scene.name == "testScene")
            {
                GameObject.Find("Player").transform.position = playerPos;
                InitOverworld();
            }
        }

		//Initializes the game for each level.
		void InitOverworld()
		{
			//Call the SetupScene function of the BoardManager script, pass it current level number.
			boardScript.SetupScene(level);
		}



		//Update is called every frame.
		void Update()
		{

		}
	}
}