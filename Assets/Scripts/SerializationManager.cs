using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using MyEssentials;

namespace MyEssentials.Serialization
{
    public class SerializationManager 
    {
        public byte[] SerializePlayer(Player player)
        {
            var formatter = new BinaryFormatter();
            using(var stream = new MemoryStream())
            {        
                formatter.Serialize(stream, player);
                return stream.ToArray();
            }
        }
        public Player DeserializePlayer(byte[] bytes)
        {
            var formatter = new BinaryFormatter();
            using(var stream = new MemoryStream())
            {        
                stream.Write(bytes, 0, bytes.Length);
                stream.Seek(0, SeekOrigin.Begin);
                Player player = (Player)formatter.Deserialize(stream);
                return player;
            }
        }
        public byte[] SerializePlayers(Player[] players)
        {
            var formatter = new BinaryFormatter();
            using(var stream = new MemoryStream())
            {        
                formatter.Serialize(stream, players);
                return stream.ToArray();
            }
        }

        public Player[] DeserializePlayers(byte[] bytes)
        {
            var formatter = new BinaryFormatter();
            using(var stream = new MemoryStream())
            {        
                stream.Write(bytes, 0, bytes.Length);
                stream.Seek(0, SeekOrigin.Begin);
                Player[] players = (Player[])formatter.Deserialize(stream);
                return players;
            }
        }
    }
}
