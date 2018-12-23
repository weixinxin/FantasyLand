using BattleSystem.ObjectModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem.SpaceModule
{
    internal class GridNode
    {
        internal List<UnitBase> mList;

        public bool isEmpty { get; private set; }

        public GridNode()
        {
            mList = new List<UnitBase>(4);
            isEmpty = true;
        }
        public void Remove(UnitBase obj)
        {
            if (mList.Contains(obj))
            {
                mList.Remove(obj);
            }
            isEmpty = mList.Count == 0;
        }
        public void Add(UnitBase obj)
        {
            if (!mList.Contains(obj))
            {
                mList.Add(obj);
            }
            isEmpty = mList.Count == 0;
        }

        public void Select(List<UnitBase> resultNodes, Predicate<UnitBase> match)
        {
            for (int i = 0; i < mList.Count; ++i)
            {
                if (match(mList[i]))
                {
                    resultNodes.Add(mList[i]);
                }
            }
        }

        public void SelectRect(float left, float bottom, float right, float top, List<UnitBase> resultNodes, Predicate<UnitBase> match)
        {
            for (int i = 0; i < mList.Count; ++i)
            {
                var obj = mList[i];
                if (obj.position.x >= left && obj.position.x <= right && obj.position.y >= bottom && obj.position.y <= top)
                {
                    if (match(mList[i]))
                    {
                        resultNodes.Add(mList[i]);
                    }
                }
            }
        }


        public void SelectCircle(float x, float y, float sqr_raduis, List<UnitBase> resultNodes, Predicate<UnitBase> match)
        {
            for (int i = 0; i < mList.Count; ++i)
            {
                var obj = mList[i];
                var dis = (obj.position.x - x) * (obj.position.x - x) + (obj.position.y - y) * (obj.position.y - y);
                if (dis <= sqr_raduis)
                {
                    if (match(mList[i]))
                    {
                        resultNodes.Add(mList[i]);
                    }
                }
            }
        }
    }
}
