using System;
using System.Xml.Serialization;

namespace BattleSystem.Config
{
    [XmlRoot("root")]
    public class bullet
    {

        public class Row
        {
            [XmlAttribute("ID")]
            public int ID { get; set; }

            [XmlAttribute("BulletType")]
            public BulletType BulletType { get; set; }


            [XmlAttribute("Radius")]
            public float Radius { get; set; }

            [XmlAttribute("FixRange")]
            public float FixRange { get; set; }

            [XmlAttribute("DecayScale")]
            public float DecayScale { get; set; }

            [XmlAttribute("Speed")]
            public float Speed { get; set; }

            [XmlAttribute("Acceleration")]
            public float Acceleration { get; set; }

            [XmlAttribute("Damage")]
            public int Damage   { get; set; }

            [XmlAttribute("DamageType")]
            public DamageType DamageType { get; set; }

            [XmlAttribute("AoeRadius")]
            public float AoeRadius { get; set; }

            [XmlAttribute("AoeDuration")]
            public float AoeDuration { get; set; }

            [XmlAttribute("AoeInterval")]
            public float AoeInterval { get; set; }


            [XmlAttribute("BuffEmitter")]
            public string _BuffEmitter { get; set; }
            private XMLValueArray<int> __BuffEmitter__;
            public XMLValueArray<int> BuffEmitter
            {
                get
                {
                    if (__BuffEmitter__ == null)
                    {
                        __BuffEmitter__ = _BuffEmitter;
                    }
                    return __BuffEmitter__;
                }
            }

            [XmlAttribute("Buffs")]
            public string _Buffs { get; set; }
            private XMLValueArray<int> __Buffs__;
            public XMLValueArray<int> Buffs
            {
                get
                {
                    if (__Buffs__ == null)
                    {
                        __Buffs__ = _Buffs;
                    }
                    return __Buffs__;
                }
            }
        }

        [XmlElement("Bullet")]
        public Row[] Rows;
        public bullet.Row getRow(int key)
        {
            for (int i = 0; i < Rows.Length; ++i)
            {
                var row = Rows[i];
                if (row.ID == key)
                {
                    return row;
                }
            }
            return null;
        }
    }
}
