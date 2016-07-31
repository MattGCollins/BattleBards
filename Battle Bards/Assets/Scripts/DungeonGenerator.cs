using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using System.Collections.Generic;

namespace MapGenerator {

    public class DungeonTiles
    {
        public const int TILE_WALL = 0;
        public const int TILE_FLOOR = 1;
        public const int TILE_START = 1;
        public const int TILE_END = 1;
    }

    public class Dungeon
    {
        public int[,] boardTiles;
        public int startX;
        public int startY;
        public int endX;
        public int endY;
        public int level;
    }

	public class DungeonGenerator : MonoBehaviour {
        
        private Dungeon dungeon;
		private ArrayList rooms = new ArrayList();

		Room startRoom;
		Room endRoom;

		public Dungeon createLevel(int numOfRooms, int maxRoomSize, int startingPaths, int maxPathLength, int mapWidth, int mapHeight) {
            dungeon = new Dungeon();
			dungeon.boardTiles = new int[mapWidth, mapHeight];

			generateStartingRooms (numOfRooms, maxRoomSize, mapWidth, mapHeight);

			generateStartingPaths (startingPaths, maxPathLength, mapWidth, mapHeight);

			generateConnectingPaths (mapWidth, mapHeight);

			foreach(Room room in rooms) {
				addRoomToBoard (room);
			}

			addStartAndEnd ();
            
            return dungeon;
		}

		void generateStartingRooms (int numOfRooms, int maxRoomSize, int mapWidth, int mapHeight)
		{
			startRoom = createRoom (1, 1, 2, 2);
			rooms.Add (startRoom);
			generateEndRoom (mapWidth, mapHeight);

			for (int i = 0; i < numOfRooms; ++i) {
				generateRoom (maxRoomSize, mapWidth, mapHeight);
			}
		}

		void generateEndRoom (int mapWidth, int mapHeight)
		{
			int quadrant = Random.Range (0, 3);
			int xStart = quadrant == 0 ? 1 : mapWidth / 2;
			int yStart = quadrant == 2 ? 1 : mapHeight / 2;
			int roomX = Random.Range (xStart, mapWidth - 3 - 2);
			int roomY = Random.Range (yStart, mapHeight - 3 - 2);
			endRoom = createRoom (roomX, roomY, 2, 2);
			rooms.Add (endRoom);
		}

		void generateRoom (int maxRoomSize, int mapWidth, int mapHeight)
		{
			int roomWidth = Random.Range (2, maxRoomSize + 1);
			int roomHeight = Random.Range (2, maxRoomSize + 1);
			int roomX = Random.Range (1, mapWidth - roomWidth - 2);
			int roomY = Random.Range (1, mapHeight - roomHeight - 2);
			rooms.Add (createRoom (roomX, roomY, roomWidth, roomHeight));
		}

		void generateStartingPaths (int startingPaths, int maxPathLength, int mapWidth, int mapHeight)
		{
			int halfWidth = (mapWidth - maxPathLength - 1) / 2;
			int halfHeight = (mapHeight - maxPathLength - 1) / 2;
			for (int i = 0; i < startingPaths; ++i) {
				int startX = Random.Range (1, halfHeight) * 2;
				int startY = Random.Range (1, halfWidth) * 2;
				int pathLength = Random.Range (3, maxPathLength);
				if (Random.Range (0, 2) != 0) {
					rooms.Add (new Room (startX, startY, startX + pathLength, startY));
				}
				else {
					rooms.Add (new Room (startX, startY, startX, startY + pathLength));
				}
			}
		}

		void generateConnectingPaths (int mapWidth, int mapHeight)
		{
			connectRoomGroups (mapWidth, mapHeight, generateRoomGroups ());
		}

		void addStartAndEnd ()
        {
            dungeon.startX = startRoom.x1 + 1;
            dungeon.startY = startRoom.y1 + 1;
            dungeon.endX = endRoom.x1 + 1;
            dungeon.endY = endRoom.y1 + 1;
            dungeon.boardTiles [startRoom.x1 + 1, startRoom.y1 + 1] = DungeonTiles.TILE_START;
            dungeon.boardTiles [endRoom.x1 + 1, endRoom.y1 + 1] = DungeonTiles.TILE_END;
		}

		ArrayList generateRoomGroups()
		{
			ArrayList roomGroups = new ArrayList();
			foreach (Room room in rooms) {
				ArrayList intersectedGroups = new ArrayList ();
				foreach (RoomGroup roomGroup in roomGroups) {
					if (roomGroup.intersects (room)) {
						intersectedGroups.Add (roomGroup);
					}
				}
				if (intersectedGroups.Count > 0) {
					((RoomGroup)intersectedGroups [0]).addRoom (room);
					if (intersectedGroups.Count > 1) {
						for (int i = 1; i < intersectedGroups.Count; ++i) {
							((RoomGroup)intersectedGroups [0]).addGroup ((RoomGroup)intersectedGroups [i]);
							roomGroups.Remove (intersectedGroups [i]);
						}
					}
				}
				else {
					RoomGroup roomGroup = new RoomGroup ();
					roomGroup.addRoom (room);
					roomGroups.Add (roomGroup);
				}
			}
			return roomGroups;
		}

		void connectRoomGroups (int mapWidth, int mapHeight, ArrayList roomGroups)
		{
			while (roomGroups.Count > 1) {
				RoomGroup randomGroup = (RoomGroup)roomGroups [Random.Range (0, roomGroups.Count)];
				int minDistance = mapWidth + mapHeight;
				Room addingRoom = null;
				Room addingGroupRoom = null;
				foreach (Room room in rooms) {
					if (!randomGroup.contains (room)) {
						foreach(Room groupRoom in randomGroup.groupRooms) {
							int distance = groupRoom.distance (room);
							if (distance < minDistance) {
								addingRoom = room;
								addingGroupRoom = groupRoom;
								minDistance = distance;
							}
						}
					}
				}

				createConnectingRoomInGroup (addingRoom, addingGroupRoom, randomGroup);
				foreach (RoomGroup roomGroup in roomGroups) {
					if (roomGroup.contains(addingRoom)) {
						randomGroup.addGroup (roomGroup);
						roomGroups.Remove (roomGroup);
						break;
					}
				}
			}
		}

		void createConnectingRoomInGroup (Room addingRoom, Room addingGroupRoom, RoomGroup addingRoomGroup)
		{
			if (addingRoom.xDistance (addingGroupRoom) == 0) {
				int x = addingRoom.intersectingX (addingGroupRoom);
				int y1;
				int y2;
				if (addingRoom.y1 > addingGroupRoom.y1) {
					y1 = addingGroupRoom.y2;
					y2 = addingRoom.y1;
				} else {
					y1 = addingRoom.y2;
					y2 = addingGroupRoom.y1;
				}
				Room room = new Room (x, y1, x, y2);
				addRoomToGroup (addingRoomGroup, room);
			} else if (addingRoom.yDistance (addingGroupRoom) == 0) {
				int y = addingRoom.intersectingY (addingGroupRoom);
				int x1 = 0;
				int x2 = 0;
				if (addingRoom.x1 > addingGroupRoom.x1) {
					x1 = addingGroupRoom.x2;
					x2 = addingRoom.x1;
				} else {
					x1 = addingRoom.x2;
					x2 = addingGroupRoom.x1;
				}
				Room room = new Room (x1, y, x2, y);
				addRoomToGroup (addingRoomGroup, room);
			} else {
				int startX = Random.Range (addingRoom.x1, addingRoom.x2 + 1);
				int startY = Random.Range (addingRoom.y1, addingRoom.y2 + 1);
				int endX = Random.Range (addingGroupRoom.x1, addingGroupRoom.x2 + 1);
				int endY = Random.Range (addingGroupRoom.y1, addingGroupRoom.y2 + 1);
				bool moveX = Random.Range (0, 2) != 0;
				int intermediateX = moveX ? endX : startX;
				int intermediateY = moveX ? startY : endY;
				createAndAddRoomSafe (startX, startY, intermediateX, intermediateY, addingRoomGroup);
				createAndAddRoomSafe (intermediateX, intermediateY, endX, endY, addingRoomGroup);
			}
		}

		void addRoomToGroup (RoomGroup addingRoomGroup, Room room)
		{
			addingRoomGroup.addRoom (room);
			rooms.Add (room);
		}

		void createAndAddRoomSafe (int startX, int startY, int endX, int endY, RoomGroup roomGroup)
		{
			int x1 = Mathf.Min (startX, endX);
			int x2 = Mathf.Max (startX, endX);
			int y1 = Mathf.Min (startY, endY);
			int y2 = Mathf.Max (startY, endY);
			addRoomToGroup (roomGroup, new Room (x1, y1, x2, y2));
		}

		void addRoomToBoard (Room room)
		{
			for(int x = room.x1; x <= room.x2; ++x) {
				for(int y = room.y1; y <= room.y2; ++y) {
                    dungeon.boardTiles [x, y] = DungeonTiles.TILE_FLOOR;
				}
			}
		}

		private Room createRoom(int x1, int y1, int width, int height) {
				return new Room(x1, y1, x1 + width, y1 + height);
		}

		private class Room {
			public int x1;
			public int y1;
			public int x2;
			public int y2;

			public Room(Room room) {
				this.x1 = room.x1;
				this.y1 = room.y1;
				this.x2 = room.x2;
				this.y2 = room.y2;
			}

			public Room(int x1, int y1, int x2, int y2) {
				this.x1 = x1;
				this.y1 = y1;
				this.x2 = x2;
				this.y2 = y2;
			}

			public bool intersects(Room room) {
				if (x1 <= room.x2 && x2 >= room.x1 && y1 <= room.y2 && y2 >= room.y1) {
					return true;
				}
				return false;
			}

			public int distance(Room room) {
				int xDiff = xDistance(room);
				int yDiff = yDistance(room);

				return xDiff + yDiff;
			}

			public int xDistance(Room room) {
				if (x1 > room.x2) {
					return x1 - room.x2;
				} else if (room.x1 > x2) {
					return room.x1 - x2;
				}
				return 0;
			}

			public int yDistance(Room room) {
				if (y1 > room.y2) {
					return y1 - room.y2;
				} else if (room.y1 > y2) {
					return room.y1 - y2;
				}
				return 0;
			}

			public int intersectingX(Room room) {
				int xMin = Mathf.Max (x1, room.x1);
				int xMax = Mathf.Min (x2, room.x2);
				return Random.Range (xMin, xMax + 1);
			}

			public int intersectingY(Room room) {
				int yMin = Mathf.Max (y1, room.y1);
				int yMax = Mathf.Min (y2, room.y2);
				return Random.Range (yMin, yMax + 1);
			}
		}

		private class RoomGroup {
			public ArrayList groupRooms = new ArrayList ();
			Room bounds;

			public void addRoom(Room room) {
				groupRooms.Add (room);

				if (null == bounds) {
					bounds = new Room (room);
				} else {
					bounds.x1 = Mathf.Min (room.x1, bounds.x1);
					bounds.y1 = Mathf.Min (room.y1, bounds.y1);
					bounds.x2 = Mathf.Max (room.x2, bounds.x2);
					bounds.y2 = Mathf.Max (room.y2, bounds.y2);
				}
			}

			public bool intersects(Room room) {
				if(bounds.intersects(room)) {
					foreach(Room groupRoom in groupRooms) {
						if (groupRoom.intersects (room)) {
							return true;
						}
					}
				}
				return false;
			}

			public void addGroup(RoomGroup group) {
				foreach (Room room in group.groupRooms) {
					addRoom (room);
				}
			}

			public bool contains(Room room) {
				return groupRooms.Contains (room);
			}
		}
	}

}