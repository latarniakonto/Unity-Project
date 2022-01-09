using System;

namespace MyEssentials
{
    [Serializable]
    public struct Player
    {
        public string id;
        public float xPosition;
        public float yPosition;
        public float zPosition;
        public Player(string id, float xPosition, float yPosition, float zPosition)
        {
            this.id = id;
            this.xPosition = xPosition;
            this.yPosition = yPosition;
            this.zPosition = zPosition;
        }
    }
}