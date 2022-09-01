using System;
using System.Text;

namespace NbtTools.Mca
{
    public class HeaderEntry
    {
        public int Id { get; private set; }
        public int Offset { get; set; }
        public int Length { get; set; }
        public int Timestamp { get; set; }

        public HeaderEntry(int id)
        {
            Id = id;
        }

        public bool IsEmpty() {
            return Offset == 0
                && Length == 0;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"Chunk {Id}\n");
            sb.Append($"\tOffset: {Offset}\n");
            sb.Append($"\tLength: {Length}\n");
            sb.Append($"\tTimestamp: {Timestamp}");
            return sb.ToString();
        }
    }
}
