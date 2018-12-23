using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BattleSystem.Config
{

    [XmlRoot("root")]
    public class unit
    {
        public class Row
        {

            [XmlAttribute("ID")]
            public int ID { get; set; }


            [XmlAttribute("MaxHP")]
            public int MaxHP { get; set; }


            [XmlAttribute("Attack")]
            public int Attack { get; set; }

            [XmlAttribute("Radius")]
            public float Radius { get; set; }

            [XmlAttribute("ModelName")]
            public string ModelName { get; set; }


            [XmlAttribute("MoveSpeed")]
            public float MoveSpeed { get; set; }


            [XmlAttribute("AttackDuration")]
            public float AttackDuration { get; set; }

            [XmlAttribute("AttackRange")]
            public float AttackRange { get; set; }

            [XmlAttribute("VisualRange")]
            public float VisualRange { get; set; }

            [XmlAttribute("Animator")]
            public int Animator { get; set; }
            [XmlAttribute("Bullet")]
            public int Bullet { get; set; }
            
            [XmlAttribute("Skills")]
            public string _Skills { get; set; }
            private XMLValueArray<int> _Skills__;
            public XMLValueArray<int> Skills
            {
                get
                {
                    if (_Skills__ == null)
                    {
                        _Skills__ = _Skills;
                    }
                    return _Skills__;
                }
            }

        }

        [XmlElement("Unit")]
        public Row[] Rows;
        public unit.Row getRow(int key)
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
