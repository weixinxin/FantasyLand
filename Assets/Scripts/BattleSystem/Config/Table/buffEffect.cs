using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleSystem.SkillModule;
using System.Xml.Serialization;

namespace BattleSystem.Config
{
    [XmlRoot("root")]
    public class buffEffect
    {
        public class Row
        {
            [XmlAttribute("ID")]
            public int ID { get; set; }

            [XmlAttribute("BuffEffectType")]
            public BuffEffectType BuffEffectType { get; set; }


            [XmlAttribute("isPermanent")]
            public bool isPermanent { get; set; }


            [XmlAttribute("BaseDelta")]
            public int BaseDelta { get; set; }


            [XmlAttribute("CurDelta")]
            public int CurDelta { get; set; }


            [XmlAttribute("CurPercent")]
            public float CurPercent { get; set; }

            [XmlAttribute("BasePercent")]
            public float BasePercent { get; set; }
            
        }
        [XmlElement("BuffEffect")]
        public Row[] Rows;
        public buffEffect.Row getRow(int key)
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
