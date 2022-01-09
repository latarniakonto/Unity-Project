using System;
[Serializable]
public struct Player
{
    public int id;
    public float xPosition;
    public float yPosition;
    public float zPosition;
    public Player(int id, float xPosition, float yPosition, float zPosition)
    {
        this.id = id;
        this.xPosition = xPosition;
        this.yPosition = yPosition;
        this.zPosition = zPosition;
    }
}