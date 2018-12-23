using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BattleSystem.Config
{
    public class Animator
    {
        [XmlAttribute("id")]
        public int id { get; set; }

        [XmlAttribute("avatar")]
        public string avatar { get; set; }
        public class Animation
        {
            [XmlAttribute("name")]
            public string name { get; set; }

            [XmlAttribute("loop")]
            public bool loop { get; set; }

            [XmlAttribute("duration")]
            public int duration { get; set; }

            public class Event
            {
                [XmlAttribute("frame")]
                public int frame { get; set; }
                [XmlAttribute("name")]
                public string name { get; set; }
            }
            [XmlElement("Event")]
            public Event[] Events;
        }
        
        [XmlElement("Animation")]
        public Animation[] Animations;

        public Animation getAnimation(string name)
        {
            for (int i = 0; i < Animations.Length; ++i)
            {
                var row = Animations[i];
                if (row.name == name)
                {
                    return row;
                }
            }
            return null;
        }
    }
    [XmlRoot("Animators")]
    public class Animators
    {
        [XmlElement("Animator")]
        public Animator[] Rows;
        public Animator getRow(int key)
        {
            for (int i = 0; i < Rows.Length; ++i)
            {
                var row = Rows[i];
                if (row.id == key)
                {
                    return row;
                }
            }
            return null;
        }
    }
}
