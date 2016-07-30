using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace Tutorial2D
{
	//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
	public class Player : MovingObject
	{
		public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
		public int pointsPerFood = 10;              //Number of points to add to player food points when picking up a food object.
		public int pointsPerSoda = 20;              //Number of points to add to player food points when picking up a soda object.
		public int wallDamage = 1;                  //How much damage a player does to a wall when chopping it.


		private Animator animator;                  //Used to store a reference to the Player's animator component.
		private int food;                           //Used to store player food points total during level.
        
		//Start overrides the Start function of MovingObject
		protected override void Start ()
		{
			//Get a component reference to the Player's animator component
			animator = GetComponent<Animator>();

			//Call the Start function of the MovingObject base class.
			base.Start ();
		}


		//This function is called when the behaviour becomes disabled or inactive.
		private void OnDisable ()
		{
		}


		private void Update ()
		{

			int horizontal = 0;     //Used to store the horizontal move direction.
			int vertical = 0;       //Used to store the vertical move direction.


			//Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction 
			if (Input.GetKey (KeyCode.RightArrow))
				horizontal = 1;
			if (Input.GetKey(KeyCode.LeftArrow))
				horizontal = -1;
			if (Input.GetKey(KeyCode.UpArrow))
				vertical = 1;
			if (Input.GetKey(KeyCode.DownArrow))
				vertical = -1;
			if (horizontal != 0)
				vertical = 0;
			if (vertical != 0)
				horizontal = 0;
            //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction

            //Check if we have a non-zero value for horizontal or vertical
            if (!moving && (horizontal != 0 || vertical != 0))
			{
				//Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
				//Pass in horizontal and vertical as parameters to specify the direction to move Player in.
				AttemptMove (horizontal,vertical);
                Debug.Log(transform.position.x + ", " + transform.position.y);
            }
		}

		//AttemptMove overrides the AttemptMove function in the base class MovingObject
		//AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
		protected override void AttemptMove (int xDir, int yDir)
		{
            Debug.Log("Attempting Move");
			//Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
			base.AttemptMove (xDir, yDir);

			//Hit allows us to reference the result of the Linecast done in Move.
			RaycastHit2D hit;

			//If Move returns true, meaning Player was able to move into an empty space.
			if (Move (xDir, yDir, out hit)) 
			{
                //Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
            }

            System.Random random = new System.Random();

            // 5% chance of initiating combat on move
            // TODO: this is really hacky; 
            // scene transitions should be managed by the game manager
            if (random.Next(0, 100) > 95)
            {
                //GameObject.Find("GameManager").GetComponent<GameManager>().playerPos = transform.position;
                //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
                //Invoke("LoadCombat", restartLevelDelay);
            }
			//Since the player has moved and lost food points, check if the game has ended.
			CheckIfGameOver();
            Debug.Log("Moved");
        }


		//OnCantMove overrides the abstract function OnCantMove in MovingObject.
		//It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
		protected override void OnCantMove <T> (T component)
		{
		}


		//OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
		private void OnTriggerEnter2D (Collider2D other)
		{
		}


		//Restart reloads the scene when called.
		private void LoadCombat ()
		{
			//Load the last scene loaded, in this case Main, the only scene in the game.
            SceneManager.LoadScene("Combat");
		}


		//LoseFood is called when an enemy attacks the player.
		//It takes a parameter loss which specifies how many points to lose.
		public void LoseFood (int loss)
		{
			//Set the trigger for the player animator to transition to the playerHit animation.
			animator.SetTrigger ("playerHit");

			//Check to see if game has ended.
			CheckIfGameOver ();
		}


		//CheckIfGameOver checks if the player is out of food points and if so, ends the game.
		private void CheckIfGameOver ()
		{
		}
	}
}