using UnityEngine;
using System;
using System.Collections.Generic;       //Allows us to use Lists.
using Random = UnityEngine.Random;      //Tells Random to use the Unity Engine random number generator.
using MapGenerator;

namespace Tutorial2D

{

	public class BoardManager : MonoBehaviour
	{
		// Using Serializable allows us to embed a class with sub properties in the inspector.
		/*[Serializable]
		public class Count
		{
			public int minimum;             //Minimum value for our Count class.
			public int maximum;             //Maximum value for our Count class.


			//Assignment constructor.
			public Count (int min, int max)
			{
				minimum = min;
				maximum = max;
			}
		}


		public int columns = 36;                                         //Number of columns in our game board.
		public int rows = 36;                                            //Number of rows in our game board.
		public Count wallCount = new Count (5, 9);                      //Lower and upper limit for our random number of walls per level.
		public Count foodCount = new Count (1, 5);     */                 //Lower and upper limit for our random number of food items per level.
		public GameObject exit;                                         //Prefab to spawn for exit.
		public GameObject[] mapTiles;                             //Array of outer tile prefabs.

		private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
		private List <Vector3> gridPositions = new List <Vector3> ();   //A list of possible locations to place tiles.

		public static OverworldMap overworldMap;
		private OverworldGenerator overworldGenerator;


		//Sets up the outer walls and floor (background) of the game board.
		void BoardSetup ()
		{
			//Instantiate Board and set boardHolder to its transform.
			overworldGenerator = GetComponent<OverworldGenerator>();
			boardHolder = new GameObject ("Board").transform;

			overworldMap = overworldGenerator.initBoard ();
			generateTiles ();
            setInitialPlayerPosition();
		}

        private void setInitialPlayerPosition()
        {
            Transform playerTransform = GameObject.Find("Player").transform;
            if(null != playerTransform)
            {
                playerTransform.position = new Vector3(overworldMap.cityX, overworldMap.cityY, playerTransform.position.z);
            }
        }

        void generateTiles ()
		{
			for(int x = 0; x < OverworldMap.mapWidth; ++x) {
				for (int y = 0; y < OverworldMap.mapHeight; ++y) {
					generateTile (overworldMap.boardTiles[x, y], x, y);
				}
			}
		}

		public GameObject generateTile(int tileIndex, int x, int y) {
			GameObject toInstantiate = mapTiles [tileIndex];
			GameObject instance = Instantiate (toInstantiate, new Vector3 ((float) x, (float) y, 0f), Quaternion.identity) as GameObject;

			instance.transform.SetParent (boardHolder);

			return instance;
		}


		//RandomPosition returns a random position from our list gridPositions.
		Vector3 RandomPosition ()
		{
			//Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
			int randomIndex = Random.Range (0, gridPositions.Count);

			//Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
			Vector3 randomPosition = gridPositions[randomIndex];

			//Remove the entry at randomIndex from the list so that it can't be re-used.
			gridPositions.RemoveAt (randomIndex);

			//Return the randomly selected Vector3 position.
			return randomPosition;
		}


		//LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
		void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
		{
			//Choose a random number of objects to instantiate within the minimum and maximum limits
			int objectCount = Random.Range (minimum, maximum+1);

			//Instantiate objects until the randomly chosen limit objectCount is reached
			for(int i = 0; i < objectCount; i++)
			{
				//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
				Vector3 randomPosition = RandomPosition();

				//Choose a random tile from tileArray and assign it to tileChoice
				GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];

				//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
				Instantiate(tileChoice, randomPosition, Quaternion.identity);
			}
		}


		//SetupScene initializes our level and calls the previous functions to lay out the game board
		public void SetupScene (int level)
		{
			//Creates the outer walls and floor.
			BoardSetup ();

			//Reset our list of gridpositions.
			//InitialiseList ();

			//Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
			//LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);

			//Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
			//LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);

			//Determine number of enemies based on current level number, based on a logarithmic progression
			//int enemyCount = (int)Mathf.Log(level, 2f);

			//Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
			//LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);

			//Instantiate the exit tile in the upper right hand corner of our game board
			//Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity);
		}
	}
}