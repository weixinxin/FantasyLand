using System;
using System.Xml.Serialization;

namespace BattleSystem.Config
{
    [XmlRoot("root")]
    public class buffEmitter
    {

        public class Row
        {
            [XmlAttribute("ID")]
            public int ID { get; set; }

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

            [XmlAttribute("AoeFilter")]
            public AoeFilter AoeFilter { get; set; }
            
        }

        [XmlElement("BuffEmitter")]
        public Row[] Rows;
        public buffEmitter.Row getRow(int key)
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
