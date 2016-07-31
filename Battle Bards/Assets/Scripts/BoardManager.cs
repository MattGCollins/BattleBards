using UnityEngine;
using System;
using System.Collections.Generic;       //Allows us to use Lists.
using Random = UnityEngine.Random;      //Tells Random to use the Unity Engine random number generator.
using MapGenerator;

namespace Tutorial2D

{

	public class BoardManager : MonoBehaviour
    {
        public GameObject[] overworldTiles;
        public GameObject[] dungeonTiles;

        private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
		private List <Vector3> gridPositions = new List <Vector3> ();   //A list of possible locations to place tiles.

		public OverworldMap overworldMap;
        public Dungeon[] dungeons;

        private OverworldGenerator overworldGenerator;
        private DungeonGenerator dungeonGenerator;

        private int currentFloor = -1;

        Player player;

        //Sets up the outer walls and floor (background) of the game board.
        public void boardSetup ()
		{
            //Instantiate Board and set boardHolder to its transform.
            player = FindObjectOfType(typeof(Player)) as Player;
            overworldGenerator = GetComponent<OverworldGenerator>();
            dungeonGenerator = GetComponent<DungeonGenerator>();
            boardHolder = new GameObject ("Board").transform;

			overworldMap = overworldGenerator.initBoard ();
            dungeons = new Dungeon[5];
            dungeons[0] = dungeonGenerator.createLevel(4, 5, 0, 10, 30, 30);
            dungeons[1] = dungeonGenerator.createLevel(20, 6, 0, 10, 50, 50);
            dungeons[2] = dungeonGenerator.createLevel(40, 7, 0, 10, 70, 70);
            dungeons[3] = dungeonGenerator.createLevel(60, 8, 0, 10, 90, 90);
            dungeons[4] = dungeonGenerator.createLevel(80, 9, 0, 10, 110, 110);

            generateOverworldTiles();
            setInitialPlayerPosition();
		}

        private void setInitialPlayerPosition()
        {
            setPlayerPosition(overworldMap.cityX, overworldMap.cityY);
        }

        private void setPlayerPosition(int x, int y)
        {
            Transform playerTransform = player.gameObject.transform;
            if (null != playerTransform)
            {
                playerTransform.position = new Vector3(x, y, playerTransform.position.z);
            }
        }

        void generateOverworldTiles()
        {
            for (int x = 0; x < OverworldMap.mapWidth; ++x)
            {
                for (int y = 0; y < OverworldMap.mapHeight; ++y)
                {
                    generateOverworldTile(overworldMap.boardTiles[x, y], x, y);
                }
            }
        }

        void generateDungeonTiles()
        {
            for (int x = 0; x < dungeons[currentFloor].mapWidth; ++x)
            {
                for (int y = 0; y < dungeons[currentFloor].mapHeight; ++y)
                {
                    generateDungeonTile(dungeons[currentFloor].boardTiles[x, y], x, y);
                }
            }
        }

        public GameObject generateOverworldTile(int tileIndex, int x, int y)
        {
            GameObject toInstantiate = overworldTiles[tileIndex];
            GameObject instance = Instantiate(toInstantiate, new Vector3((float)x, (float)y, 0f), Quaternion.identity) as GameObject;

            instance.transform.SetParent(boardHolder);

            return instance;
        }

        public GameObject generateDungeonTile(int tileIndex, int x, int y)
        {
            GameObject toInstantiate = dungeonTiles[tileIndex];
            GameObject instance = Instantiate(toInstantiate, new Vector3((float)x, (float)y, 0f), Quaternion.identity) as GameObject;

            instance.transform.SetParent(boardHolder);

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

        public void doTileActions()
        {
            Transform playerTransform = player.gameObject.transform;
            int x = Convert.ToInt32(playerTransform.position.x);
            int y = Convert.ToInt32(playerTransform.position.y);
            if (-1 == currentFloor)
            {
                doOverworldActions(x, y);
            } else
            {
                doDungeonActions(x, y);
            }
        }

        private void doOverworldActions(int x, int y)
        {
            switch (overworldMap.boardTiles[x, y])
            {
                case OverworldTiles.PATH_TILE:
                    checkForEnemyInRegion(2);
                    break;
                case OverworldTiles.GRASS_TILE:
                    checkForEnemyInRegion(5);
                    break;
                case OverworldTiles.FOREST_TILE:
                    checkForEnemyInRegion(10);
                    break;
                case OverworldTiles.SAND_TILE:
                    checkForEnemyInRegion(20);
                    break;
                case OverworldTiles.CAVE_TILE:
                    changeFloor(1);
                    break;
            }
        }

        private void doDungeonActions(int x, int y)
        {
            switch (dungeons[currentFloor].boardTiles[x, y])
            {
                case DungeonTiles.TILE_FLOOR:
                    checkForEnemyInRegion(2);
                    break;
                case DungeonTiles.TILE_START:
                    changeFloor(-1);
                    break;
                case DungeonTiles.TILE_END:
                    changeFloor(1);
                    break;
            }
        }

        private void checkForEnemyInRegion(int chance)
        {
            System.Random random = new System.Random();
            if (random.Next(0, 100) <= chance)
            {
                //GameObject.Find("GameManager").GetComponent<GameManager>().playerPos = transform.position;
                //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
                Debug.Log("Battle!");
                //Invoke("LoadCombat", restartLevelDelay);
            }
        }

        private void changeFloor(int difference)
        {
            if(currentFloor < 4 || 1 != difference)
            {
                GameObject.Destroy(boardHolder.gameObject);
                currentFloor += difference;
                boardHolder = new GameObject("Board").transform;
                if (-1 == currentFloor)
                {
                    generateOverworldTiles();
                    setPlayerPosition(overworldMap.caveX, overworldMap.caveY);
                }
                else
                {
                    generateDungeonTiles();
                    if (-1 == difference)
                    {
                        setPlayerPosition(dungeons[currentFloor].endX, dungeons[currentFloor].endY);
                    }
                    else if (1 == difference)
                    {
                        setPlayerPosition(dungeons[currentFloor].startX, dungeons[currentFloor].startX);
                    }
                }
            }
        }
	}
}