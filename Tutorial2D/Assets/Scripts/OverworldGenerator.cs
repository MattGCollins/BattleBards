using UnityEngine;
using System; 	
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace MapGenerator {

	public class OverworldTiles {
		public const int CITY_TILE = 0;
		public const int FOREST_TILE = 1;
		public const int GRASS_TILE = 2;
		public const int MOUNTAIN_TILE = 3;
		public const int PATH_TILE = 4;
		public const int SAND_TILE = 5;
		public const int WATER_TILE = 6;
		public const int CAVE_TILE = 7;
	}

	public class OverworldGenerator : MonoBehaviour {

		public OverworldMap overworldMap;

		public GameObject[] prototypeTiles;
		private Transform boardHolder;

		int[] bisectingDirections = new int[]{5, 6};

		int traversalX;
		int traversalY;

		public OverworldMap initBoard() {
			overworldMap = new OverworldMap ();
			boardHolder = new GameObject ("Board").transform;
			overworldMap.boardTiles = new int[OverworldMap.mapWidth, OverworldMap.mapHeight];

			traversalX = randomBetween (20, 30);
			traversalY = randomBetween (-15, 15);

			int outerEdgesHoriz = Convert.ToInt32(Math.Floor((double)(OverworldMap.mapWidth - Math.Abs(traversalX)) / 2));
			int outerEdgesVert = Convert.ToInt32(Math.Floor((double)(OverworldMap.mapHeight - Math.Abs(traversalY)) / 2));
			int cityHeight = traversalY > 0 ? outerEdgesVert : OverworldMap.mapHeight - outerEdgesVert;
			int caveHeight = traversalY > 0 ? OverworldMap.mapHeight - outerEdgesVert : outerEdgesVert;
			overworldMap.cityX = outerEdgesHoriz;
			overworldMap.cityY = cityHeight;
			overworldMap.caveX = (OverworldMap.mapWidth - outerEdgesHoriz);
			overworldMap.caveY = caveHeight;

			fillWaterAndGrass ();
			fillMountains ();
			fillForests ();
			createRiver (Convert.ToInt32(overworldMap.caveX), Convert.ToInt32(overworldMap.caveY) + 2, new int[]{ 0, 1, 2, 3 }, 2);
			createBisectingRiver (Convert.ToInt32(overworldMap.caveX) - 2, Convert.ToInt32(overworldMap.caveY), 6);
			createPath (Convert.ToInt32 (overworldMap.cityX), Convert.ToInt32 (overworldMap.cityY) + 1, Convert.ToInt32 (overworldMap.caveX) - 9, Convert.ToInt32 (overworldMap.caveY));
			createPath (Convert.ToInt32 (overworldMap.caveX) - 9, Convert.ToInt32 (overworldMap.caveY), Convert.ToInt32 (overworldMap.caveX), Convert.ToInt32 (overworldMap.caveY) - 9);
			createPath (Convert.ToInt32 (overworldMap.caveX), Convert.ToInt32 (overworldMap.caveY) - 9, Convert.ToInt32 (overworldMap.caveX), Convert.ToInt32 (overworldMap.caveY) - 1);
			fillSand ();

			setCity (Convert.ToInt32(overworldMap.cityX), Convert.ToInt32(overworldMap.cityY));
			setCave (Convert.ToInt32(overworldMap.caveX), Convert.ToInt32(overworldMap.caveY));
			Debug.Log ("Done generating");

			return overworldMap;
		}

		void fillWaterAndGrass ()
		{
			for (int x = 0; x < OverworldMap.mapWidth; ++x) {
				for (int y = 0; y < OverworldMap.mapHeight; ++y) {
					setWater (x, y);
				}
			}
			fillTypeRegardless (Convert.ToInt32 (overworldMap.cityX), Convert.ToInt32 (overworldMap.cityY), 30, 10, OverworldTiles.GRASS_TILE);
			fillTypeRegardless (Convert.ToInt32 (overworldMap.caveX), Convert.ToInt32 (overworldMap.caveY), 30, 10, OverworldTiles.GRASS_TILE);
			fillTypeRegardless (Convert.ToInt32 (overworldMap.caveX), Convert.ToInt32 (overworldMap.caveY) + (30 * ((randomBetween(0, 1) * 2) - 1)), 15, 10, OverworldTiles.GRASS_TILE);
			int bigAddDirection = traversalY > 0 ? -1 : 1;
			fillTypeRegardless (Convert.ToInt32 (overworldMap.cityX) + randomBetween(10, 15), Convert.ToInt32 (overworldMap.cityY) - (15 * bigAddDirection), 20, 10, OverworldTiles.GRASS_TILE);
		}

		void fillForests() {
			fillForest (Convert.ToInt32(overworldMap.cityX + 1), Convert.ToInt32(overworldMap.cityY), 5);
			fillForest (Convert.ToInt32(overworldMap.cityX - 1), Convert.ToInt32(overworldMap.cityY), 5);
			fillForest (Convert.ToInt32(overworldMap.cityX), Convert.ToInt32(overworldMap.cityY + 1), 5);
			fillForest (Convert.ToInt32(overworldMap.cityX), Convert.ToInt32(overworldMap.cityY - 1), 5);

			for (int i = 0; i < 6; ++i) {
				int x = 0;
				int y = 0;
				while (overworldMap.boardTiles [x, y] != OverworldTiles.GRASS_TILE) {
					x = randomBetween (0, OverworldMap.mapWidth - 1);
					y = randomBetween (0, OverworldMap.mapHeight - 1);
				}
				int size = randomBetween (4, 9);
				fillType (x, y, size, size - 1, OverworldTiles.FOREST_TILE);
			}
			
		}

		void fillForest(int x, int y, int chance) {
			fillType (x, y, chance, 5, OverworldTiles.FOREST_TILE);
		}

		void fillTypeRegardless(int x, int y, int chance, int outerRandomness, int type) {
			Queue<GeneratorNode> generatorNodes = new Queue<GeneratorNode> ();
			generatorNodes.Enqueue(new GeneratorNode(x, y, chance));
			int nodesProcessed = 0;
			while (generatorNodes.Count > 0) {
				nodesProcessed++;
				GeneratorNode currentNode = generatorNodes.Dequeue ();
				setType (currentNode.x, currentNode.y, type);
				if ((currentNode.x >= x) && currentNode.chance > (outerRandomness * Random.value)) {
					enqueueNode(generatorNodes, new GeneratorNode(currentNode.x + 1, currentNode.y, currentNode.chance - 1));
				}
				if ((currentNode.x <= x) && currentNode.chance > (outerRandomness * Random.value)) {
					enqueueNode(generatorNodes, new GeneratorNode(currentNode.x - 1, currentNode.y, currentNode.chance - 1));
				}
				if ((currentNode.y >= y) && currentNode.chance > (outerRandomness * Random.value)) {
					enqueueNode(generatorNodes, new GeneratorNode(currentNode.x, currentNode.y + 1, currentNode.chance - 1));
				}
				if ((currentNode.y <= y) && currentNode.chance > (outerRandomness * Random.value)) {
					enqueueNode(generatorNodes, new GeneratorNode(currentNode.x, currentNode.y - 1, currentNode.chance - 1));
				}
			}
		}

		void enqueueNode (Queue<GeneratorNode> generatorNodes, GeneratorNode generatorNode)
		{
			GeneratorNode existingNode = getNode (generatorNodes, generatorNode);
			if (null == existingNode) {
				generatorNodes.Enqueue (generatorNode);
			} else {
				existingNode.chance = Math.Max (existingNode.chance, generatorNode.chance);
				return;
			}

		}

		GeneratorNode getNode (Queue<GeneratorNode> generatorNodes, GeneratorNode generatorNode)
		{
			foreach (GeneratorNode iterNode in generatorNodes.ToArray ()) {
				if (iterNode.Equals (generatorNode)) {
					return iterNode;
				}
			}
			return null;
		}

		void fillType(int x, int y, int chance, int outerRandomness, int type) {
			if (overworldMap.boardTiles[x,y] == OverworldTiles.GRASS_TILE || overworldMap.boardTiles[x,y] == type) {
				setType (x, y, type);
				if (chance > (outerRandomness * Random.value)) {
					fillType (Convert.ToInt32 (x + 1), Convert.ToInt32 (y), chance - 1, outerRandomness, type);
				}
				if (chance > (outerRandomness * Random.value)) {
					fillType (Convert.ToInt32 (x - 1), Convert.ToInt32 (y), chance - 1, outerRandomness, type);
				}
				if (chance > (outerRandomness * Random.value)) {
					fillType (Convert.ToInt32 (x), Convert.ToInt32 (y + 1), chance - 1, outerRandomness, type);
				}
				if (chance > (outerRandomness * Random.value)) {
					fillType (Convert.ToInt32 (x), Convert.ToInt32 (y - 1), chance - 1, outerRandomness, type);
				}
			}
		}

		void fillMountains ()
		{
			fillMountains (Convert.ToInt32(overworldMap.caveX + 1), Convert.ToInt32(overworldMap.caveY), 7);
			fillMountains (Convert.ToInt32(overworldMap.caveX - 1), Convert.ToInt32(overworldMap.caveY), 7);
			fillMountains (Convert.ToInt32(overworldMap.caveX), Convert.ToInt32(overworldMap.caveY + 1), 7);
			fillMountains (Convert.ToInt32(overworldMap.caveX), Convert.ToInt32(overworldMap.caveY - 1), 7);
		}

		void fillMountains(int x, int y, int chance) {
			fillType (x, y, chance, 7, OverworldTiles.MOUNTAIN_TILE);
		}

		void createBisectingRiver(int x, int y, int lastDirection) {
			if (y >= overworldMap.cityY) {
				setWater (x, y);
				int directionToPick = randomBetween (0, bisectingDirections.Length - 1);
				int desiredDirection = bisectingDirections [directionToPick];
				if (desiredDirection > lastDirection) {
					desiredDirection = lastDirection + 1;
				} else if (desiredDirection < lastDirection) {
					desiredDirection = lastDirection - 1;
				}
				switch (desiredDirection) {
				case 4:
					createOneOfType (x + 1, y, x, y - 1, OverworldTiles.WATER_TILE);
					createBisectingRiver (x + 1, y - 1, desiredDirection);
					break;
				case 5:
					createBisectingRiver (x, y - 1, desiredDirection);
					break;
				case 6:
					createOneOfType (x - 1, y, x, y - 1, OverworldTiles.WATER_TILE);
					createBisectingRiver (x - 1, y - 1, desiredDirection);
					break;
				}
			} else if(!(overworldMap.boardTiles [x, y] == OverworldTiles.WATER_TILE)) {
				createRiver(x, y, new int[]{5, 6, 7}, lastDirection);
			}
		}

		void createRiver(int x, int y, int[] directions, int lastDirection) {
			if (!(overworldMap.boardTiles [x, y] == OverworldTiles.WATER_TILE)) {
				setWater (x, y);
				int directionToPick = randomBetween (0, directions.Length - 1);
				int desiredDirection = directions [directionToPick];
				if (desiredDirection > lastDirection) {
					desiredDirection = lastDirection + 1;
				} else if (desiredDirection < lastDirection) {
					desiredDirection = lastDirection - 1;
				}
				switch (desiredDirection) {
				case 0:
					createOneOfType (x - 1, y, x, y + 1, OverworldTiles.WATER_TILE);
					createRiver (x - 1, y + 1, directions, desiredDirection);
					break;
				case 1:
					createRiver (x, y + 1, directions, desiredDirection);
					break;
				case 2:
					createOneOfType (x + 1, y, x, y + 1, OverworldTiles.WATER_TILE);
					createRiver (x + 1, y + 1, directions, desiredDirection);
					break;
				case 3:
					createRiver (x + 1, y, directions, desiredDirection);
					break;
				case 4:
					createOneOfType (x + 1, y, x, y - 1, OverworldTiles.WATER_TILE);
					createRiver (x + 1, y - 1, directions, desiredDirection);
					break;
				case 5:
					createRiver (x, y - 1, directions, desiredDirection);
					break;
				case 6:
					createOneOfType (x - 1, y, x, y - 1, OverworldTiles.WATER_TILE);
					createRiver (x - 1, y - 1, directions, desiredDirection);
					break;
				case 7:
					createRiver (x - 1, y, directions, desiredDirection);
					break;
				}
			}
		}

		void setSurroundingGrassToSand (int x, int y)
		{
			setGrassToSand (x - 1, y + 1);
			setGrassToSand (x, y + 1);
			setGrassToSand (x + 1, y + 1);
			setGrassToSand (x + 1, y);
			setGrassToSand (x + 1, y - 1);
			setGrassToSand (x, y - 1);
			setGrassToSand (x - 1, y - 1);
			setGrassToSand (x - 1, y);
		}

		void setGrassToSand (int x, int y) {
			if (overworldMap.boardTiles [x, y] == OverworldTiles.GRASS_TILE) {
				setSand (x, y);
			}
		}

		void createPath(int fromX, int fromY, int toX, int toY) {
			int currentX = fromX;
			int currentY = fromY;
			float initialSlopeX = 0;
			float initialSlopeY = 0;
			if(fromY != toY && fromX != toX) {
				initialSlopeX = ((toX - fromX) / (toY - fromY));
				initialSlopeY = ((toY - fromY) / (toX - fromX));
			}
			setPath (fromX, fromY);
			while (currentX != toX || currentY != toY) {
				int xDirection = Math.Sign(toX - currentX);
				int yDirection = Math.Sign(toY - currentY);
				if (xDirection == 0) {
					currentY += yDirection;
				} else if (yDirection == 0) {
					currentX += xDirection;
				} else {
					float xSlope = ((toX - currentX + xDirection) / (toY - currentY));
					float ySlope = ((toY - currentY + yDirection) / (toX - currentX));

					if (Math.Abs (xSlope - initialSlopeX) > Math.Abs (ySlope - initialSlopeY)) {
						currentX += xDirection;
					} else {
						currentY += yDirection;
					}
				}
				setPath (currentX, currentY);
			}
		}

		void fillSand() {
			for (int x = 1; x < OverworldMap.mapWidth - 1; ++x) {
				for (int y = 1; y < OverworldMap.mapHeight - 1; ++y) {
					if (overworldMap.boardTiles [x, y] == OverworldTiles.GRASS_TILE && hasWaterAround(x, y)) {
						overworldMap.boardTiles [x, y] = OverworldTiles.SAND_TILE;
					}
				}
			}
		}

		bool hasWaterAround(int x, int y) {
			return overworldMap.boardTiles [x - 1, y + 1] == OverworldTiles.WATER_TILE ||
				overworldMap.boardTiles [x, y + 1] == OverworldTiles.WATER_TILE ||
				overworldMap.boardTiles [x + 1, y + 1] == OverworldTiles.WATER_TILE ||
				overworldMap.boardTiles [x + 1, y] == OverworldTiles.WATER_TILE ||
				overworldMap.boardTiles [x + 1, y - 1] == OverworldTiles.WATER_TILE ||
				overworldMap.boardTiles [x, y - 1] == OverworldTiles.WATER_TILE ||
				overworldMap.boardTiles [x - 1, y - 1] == OverworldTiles.WATER_TILE ||
				overworldMap.boardTiles [x - 1, y] == OverworldTiles.WATER_TILE;
		}

		void createOneOfType (int x1, int y1, int x2, int y2, int type)
		{
			if (Random.value > 0.5) {
				setType(x1, y1, type);
			} else {
				setType(x2, y2, type);
			}
		}

		void setCity (int x, int y)
		{
			overworldMap.boardTiles [x, y] = OverworldTiles.CITY_TILE;
		}

		void setForest (int x, int y)
		{
			overworldMap.boardTiles [x, y] = OverworldTiles.FOREST_TILE;
		}

		void setGrass (int x, int y)
		{
			overworldMap.boardTiles [x, y] = OverworldTiles.GRASS_TILE;
		}

		void setPath (int x, int y) {
			overworldMap.boardTiles [x, y] = OverworldTiles.PATH_TILE;
		}

		void setSand (int x, int y)
		{
			overworldMap.boardTiles [x, y] = OverworldTiles.SAND_TILE;
		}

		void setWater (int x, int y)
		{
			overworldMap.boardTiles [x, y] = OverworldTiles.WATER_TILE;
		}

		void setCave (int x, int y)
		{
			overworldMap.boardTiles [x, y] = OverworldTiles.CAVE_TILE;
		}

		void setType (int x, int y, int type)
		{
			overworldMap.boardTiles [x, y] = type;
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
			GameObject toInstantiate = prototypeTiles [tileIndex];
			GameObject instance = Instantiate (toInstantiate, new Vector3 ((float) x, (float) y, 0f), Quaternion.identity) as GameObject;

			instance.transform.SetParent (boardHolder);

			return instance;
		}

		public int randomBetween(int min, int max) {
			return Random.Range (min, max + 1);
		}

		private class GeneratorNode {

			public int x = 0;
			public int y = 0;
			public int chance = 0;

			public GeneratorNode(int x, int y, int chance) {
				this.x = x;
				this.y = y;
				this.chance = chance;
			}

			public bool Equals(GeneratorNode node) {
				if (null == node) {
					return false;
				}
				return x == node.x && y == node.y;
			}
		}
	}

	public class OverworldMap {
		public const int mapWidth = 110;
		public const int mapHeight = 110;

		public int cityX;
		public int cityY;
		public int caveX;
		public int caveY;
		public int[,] boardTiles;
	}
}