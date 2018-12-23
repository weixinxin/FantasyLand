using BattleSystem.SkillModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BattleSystem.Config
{
    [XmlRoot("root")]
    public class buff
    {
        public class Row
        {
            [XmlAttribute("ID")]
            public int ID { get; set; }

            [XmlAttribute("Groups")]
            public int Groups { get; set; }

            [XmlAttribute("RejectionGroups")]
            public int RejectionGroups { get; set; }

            [XmlAttribute("OverlayTactics")]
            public OverlayTactics OverlayTactics { get; set; }

            [XmlAttribute("Desc")]
            public string Desc { get; set; }

            [XmlAttribute("Delay")]
            public float Delay { get; set; }

            [XmlAttribute("isLoop")]
            public bool isLoop { get; set; }

            [XmlAttribute("Duration")]
            public float Duration { get; set; }

            [XmlAttribute("isIndividual")]
            public bool isIndividual { get; set; }

            [XmlAttribute("isClearable")]
            public bool isClearable { get; set; }

            [XmlAttribute("isNegative")]
            public bool isNegative { get; set; }

            [XmlAttribute("isScriptBuff")]
            public bool isScriptBuff { get; set; }



            [XmlAttribute("BuffEffect")]
            public string _BuffEffect { get; set; }
            private XMLValueArray<int> __BuffEffect__;
            public XMLValueArray<int> BuffEffect
            {
                get
                {
                    if (__BuffEffect__ == null)
                    {
                        __BuffEffect__ = _BuffEffect;
                    }
                    return __BuffEffect__;
                }
            }
        }
        [XmlElement("Buff")]
        public Row[] Rows;
        public buff.Row getRow(int key)
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
