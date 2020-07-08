using System;

namespace CommandCenter.Data
{
    public class Channel
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public DateTime Added { get; set; }
        
        public bool Monitor { get; set; }
    }
}