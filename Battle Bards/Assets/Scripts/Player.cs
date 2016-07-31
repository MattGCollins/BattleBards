using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using MapGenerator;

namespace Tutorial2D
{
	//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
	public class Player : MovingObject
	{
		private Animator animator;                  //Used to store a reference to the Player's animator component.
        public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
        private bool wasMoving = false;
        public bool stoppedMoving = false;
        
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
            stoppedMoving = false;
            int horizontal = 0;     //Used to store the horizontal move direction.
			int vertical = 0;       //Used to store the vertical move direction.


			//Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction 
			if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
				horizontal = 1;
			if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
				horizontal = -1;
			if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
				vertical = 1;
			if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
				vertical = -1;
			if (horizontal != 0)
				vertical = 0;
			if (vertical != 0)
				horizontal = 0;
            //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction

            //Check if we have a non-zero value for horizontal or vertical
            if(!moving)
            {
                if (wasMoving)
                {
                    stoppedMoving = true;
                } else
                {
                    AttemptMove(horizontal, vertical);
                }
            }
            wasMoving = moving;
		}

		//AttemptMove overrides the AttemptMove function in the base class MovingObject
		//AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
		protected override void AttemptMove (int xDir, int yDir)
        {
            if ((xDir != 0 || yDir != 0))
            {
                //Hit allows us to reference the result of the Linecast done in Move.
                RaycastHit2D hit;

			    //If Move returns true, meaning Player was able to move into an empty space.
			    if (Move (xDir, yDir, out hit)) 
			    {
                    //Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
                }
            }
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
            //SceneManager.LoadScene("Combat");
		}
	}
}