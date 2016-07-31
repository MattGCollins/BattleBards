using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Tutorial2D;

namespace Tutorial2D
{
	using System.Collections.Generic;       //Allows us to use Lists. 

	public class GameManager : MonoBehaviour
	{

		public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
		private BoardManager boardScript;                       //Store a reference to our BoardManager which will set up the level.
        Player player;

        //Awake is always called before any Start functions
        void Awake()
		{
            player = FindObjectOfType(typeof(Player)) as Player;
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
            boardScript.boardSetup();
        }
        
		//Update is called every frame.
		void Update()
        {
            if (player.stoppedMoving)
            {
                boardScript.doTileActions();
            }
        }



    }
}