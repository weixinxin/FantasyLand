using BattleSystem.ObjectModule;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BattleSystem.SpaceModule
{
    public class WorldSpace
    {
        private float mWidth;
        private float mHeight;
        private float mGridSize;
        private float mRecGridSize;

        private GridNode[,] mNodes;

        private int mRowCount;
        private int mColCount;
        private int mExpandCount;


        public bool Init(float width, float height, float gridSize, float safeExpand)
        {
            mWidth = width;
            mHeight = height;
            mGridSize = gridSize;
            mRecGridSize = 1.0f / gridSize;


            mColCount = (int)Math.Ceiling(width * mRecGridSize);
            mRowCount = (int)Math.Ceiling(height * mRecGridSize);
            mExpandCount = (int)Math.Ceiling(safeExpand * mRecGridSize);
            mNodes = new GridNode[mColCount + mExpandCount * 2, mRowCount + mExpandCount * 2];
            GridNode empty = new GridNode();
            for (int x = 0; x < mColCount + mExpandCount * 2; ++x)
            {
                for (int y = 0; y < mRowCount + mExpandCount * 2; ++y)
                {

                    if (x < mExpandCount || x >= mColCount + mExpandCount || y < mExpandCount || y >= mRowCount + mExpandCount)
                    {
                        mNodes[x, y] = empty;
                    }
                    else
                    {
                        mNodes[x, y] = new GridNode();
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 更新物体的坐标和所属格子
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public void UpdateNode(UnitBase obj)
        {
            var cur = GetGridNodeByPosition(obj.position.x, obj.position.y);
            if (obj.mGridNode != null && obj.mGridNode != cur)
            {
                obj.mGridNode.Remove(obj);
                cur.Add(obj);
            }
            else
            {
                cur.Add(obj);
            }
            obj.mGridNode = cur;
        }

        /// <summary>
        /// 单位死亡
        /// </summary>
        /// <param name="obj"></param>
        public void RemoveUnit(UnitBase obj)
        {
            if (obj.mGridNode != null)
            {
                obj.mGridNode.Remove(obj);
            }
        }
        /// <summary>
        /// 通过坐标获取所属格子
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private GridNode GetGridNodeByPosition(float x, float y)
        {
            x = mathf.Clamp(x, 0, mWidth);
            y = mathf.Clamp(y, 0, mHeight);

            int ix = (int)(x * mRecGridSize);
            int iy = (int)(y * mRecGridSize);

            return mNodes[ix + mExpandCount, iy + mExpandCount];
        }

        //public Stopwatch stopwatch1 = new Stopwatch();

        //public Stopwatch stopwatch2 = new Stopwatch();

        //public Stopwatch stopwatch3 = new Stopwatch();

        //public Stopwatch stopwatch4 = new Stopwatch();
        /// <summary>
        /// 筛选垂直矩形内的单位
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="resultNodes"></param>
        /// <param name="match"></param>
        public void Select(float x, float y, float width, float height, List<UnitBase> resultNodes, Predicate<UnitBase> match)
        {
            float _r = x + width;
            float _t = y + height;
            int left = (int)(x * mRecGridSize) + 1;
            int right = (int)(_r * mRecGridSize) - 1;
            int bottom = (int)(y * mRecGridSize) + 1;
            int top = (int)(_t * mRecGridSize) - 1;
            //外侧
            if (bottom != top + 2)
            {
                for (int ix = left - 1; ix <= right + 1; ++ix)
                {
                    var node = mNodes[ix + mExpandCount, bottom - 1 + mExpandCount];
                    if (!node.isEmpty)
                        node.SelectRect(x, y, _r, _t, resultNodes, match);
                    node = mNodes[ix + mExpandCount, top + 1 + mExpandCount];
                    if (!node.isEmpty)
                        node.SelectRect(x, y, _r, _t, resultNodes, match);
                }
            }
            else
            {
                for (int ix = left - 1; ix <= right + 1; ++ix)
                {
                    var node = mNodes[ix + mExpandCount, bottom - 1 + mExpandCount];
                    if (!node.isEmpty)
                        node.SelectRect(x, y, _r, _t, resultNodes, match);
                }
            }
            if (bottom <= top)
            {
                if (left != right + 2)
                {
                    for (int iy = bottom; iy <= top; ++iy)
                    {
                        var node = mNodes[left - 1 + mExpandCount, iy + mExpandCount];
                        if (!node.isEmpty)
                            node.SelectRect(x, y, _r, _t, resultNodes, match);
                        node = mNodes[right + 1 + mExpandCount, iy + mExpandCount];
                        if (!node.isEmpty)
                            node.SelectRect(x, y, _r, _t, resultNodes, match);
                    }
                }
                else
                {
                    for (int iy = bottom; iy <= top; ++iy)
                    {
                        var node = mNodes[left - 1 + mExpandCount, iy + mExpandCount];
                        if (!node.isEmpty)
                            node.SelectRect(x, y, _r, _t, resultNodes, match);
                    }

                }
            }

            //内部不检测越界
            left = mathf.Clamp(left, 0, mColCount - 1);
            right = mathf.Clamp(right, 0, mColCount - 1);
            bottom = mathf.Clamp(bottom, 0, mRowCount - 1);
            top = mathf.Clamp(top, 0, mRowCount - 1);
            if (left <= right && bottom <= top)
            {
                for (int ix = left; ix <= right; ++ix)
                {
                    for (int iy = bottom; iy <= top; ++iy)
                    {
                        var node = mNodes[ix + mExpandCount, iy + mExpandCount];
                        if (!node.isEmpty)
                            node.Select(resultNodes, match);
                    }
                }
            }
        }
        /// <summary>
        /// 筛选圆形区域内的单位
        /// </summary>
        /// <param name="x">圆心坐标x</param>
        /// <param name="y">圆心坐标y</param>
        /// <param name="radius">圆半径</param>
        /// <param name="resultNodes">符合条件单位集合</param>
        /// <param name="match">筛选条件</param>
        public void SelectCircle(float x, float y, float radius, List<UnitBase> resultNodes, Predicate<UnitBase> match)
        {
            float sqr_raduis = radius * radius;
            int left = (int)((x - radius) * mRecGridSize);
            int right = (int)((x + radius) * mRecGridSize);
            int bottom = (int)((y - radius) * mRecGridSize);
            int top = (int)((y + radius) * mRecGridSize);
            float in_radius = radius * 0.7071f;
            int in_left = (int)((x - in_radius) * mRecGridSize) + 1;
            int in_right = (int)((x + in_radius) * mRecGridSize) - 1;
            int in_bottom = (int)((y - in_radius) * mRecGridSize) + 1;
            int in_top = (int)((y + in_radius) * mRecGridSize) - 1;
            if (in_left > in_right || in_bottom > in_top)
            {
                //不存在内部区域

                left = mathf.Clamp(left, 0, mColCount - 1);
                right = mathf.Clamp(right, 0, mColCount - 1);
                bottom = mathf.Clamp(bottom, 0, mRowCount - 1);
                top = mathf.Clamp(top, 0, mRowCount - 1);
                for (int mx = left; mx <= right; ++mx)
                {
                    for (int my = bottom; my <= top; ++my)
                    {
                        var node = mNodes[mx + mExpandCount, my + mExpandCount];
                        if (!node.isEmpty)
                            node.SelectCircle(x, y, sqr_raduis, resultNodes, match);
                    }
                }

            }
            else
            {
                //存在内部区域

                for (int mx = left; mx <= right; ++mx)
                {
                    for (int my = bottom; my < in_bottom; ++my)
                    {

                        var node = mNodes[mx + mExpandCount, my + mExpandCount];
                        if (!node.isEmpty)
                        {
                            node.SelectCircle(x, y, sqr_raduis, resultNodes, match);
                        }
                    }

                    for (int my = in_top + 1; my <= top; ++my)
                    {
                        var node = mNodes[mx + mExpandCount, my + mExpandCount];
                        if (!node.isEmpty)
                        {
                            node.SelectCircle(x, y, sqr_raduis, resultNodes, match);
                        }
                    }
                }
                for (int my = in_bottom; my <= in_top; ++my)
                {
                    for (int mx = left; mx < in_left; ++mx)
                    {
                        var node = mNodes[mx + mExpandCount, my + mExpandCount];
                        if (!node.isEmpty)
                        {
                            node.SelectCircle(x, y, sqr_raduis, resultNodes, match);
                        }
                    }

                    for (int mx = in_right + 1; mx <= right; ++mx)
                    {
                        var node = mNodes[mx + mExpandCount, my + mExpandCount];
                        if (!node.isEmpty)
                        {
                            node.SelectCircle(x, y, sqr_raduis, resultNodes, match);
                        }
                    }
                }
                //内部不检测越界
                in_left = mathf.Clamp(in_left, 0, mColCount - 1);
                in_right = mathf.Clamp(in_right, 0, mColCount - 1);
                in_bottom = mathf.Clamp(in_bottom, 0, mRowCount - 1);
                in_top = mathf.Clamp(in_top, 0, mRowCount - 1);
                for (int ix = in_left; ix <= in_right; ++ix)
                {
                    for (int iy = in_bottom; iy <= in_top; ++iy)
                    {
                        var node = mNodes[ix + mExpandCount, iy + mExpandCount];
                        if (!node.isEmpty)
                            node.Select(resultNodes, match);
                    }
                }
            }
        }
        /// <summary>
        /// 选择旋转角度的矩形区域单位
        /// </summary>
        /// <param name="x">坐标x</param>
        /// <param name="y">坐标y</param>
        /// <param name="dx">方向向量x</param>
        /// <param name="dy">方向向量y</param>
        /// <param name="width">矩形宽</param>
        /// <param name="height">矩形长</param>
        /// <param name="resultNodes">符合条件单位集合</param>
        /// <param name="match">筛选条件</param>
        public void SelectRect(float x, float y, float dx, float dy, float width, float height, List<UnitBase> resultNodes, Predicate<UnitBase> match)
        {
            if (dx == 0)
            {
                var nx = x - 0.5f * width;
                var ny = dy > 0 ? y : y - height;
                Select(nx, ny, width, height, resultNodes, match);
            }
            else if (dx == 0)
            {
                var nx = dx > 0 ? x : x - width;
                var ny = y - 0.5f * height;
                Select(nx, ny, width, height, resultNodes, match);
            }
            else
            {
                var l_d = 1 / Math.Sqrt(dx * dx + dy * dy);
                var n_dx = dx * l_d;
                var n_dy = dy * l_d;

                var a_x = x - n_dy * 0.5f * width;
                var a_y = y + n_dx * 0.5f * width;

                var b_x = a_x + n_dy * width;
                var b_y = a_y - n_dx * width;

                var c_x = a_x + n_dx * height;
                var c_y = a_y + n_dy * height;

                var d_x = c_x + n_dy * width;
                var d_y = c_y - n_dx * width;

                var v0_x = n_dx * height;
                var v0_y = n_dy * height;

                var v1_x = n_dy * width;
                var v1_y = -n_dx * width;

                var dot00 = v0_x * v0_x + v0_y * v0_y;
                var dot01 = v0_x * v1_x + v0_y * v1_y;
                var dot11 = v1_x * v1_x + v1_y * v1_y;
                //var inverDeno = 1 / (dot00 * dot11 - dot01 * dot01);
                var deno = (dot00 * dot11 - dot01 * dot01);

                var l = a_x;
                if (l > b_x) l = b_x;
                if (l > c_x) l = c_x;
                if (l > d_x) l = d_x;
                var r = a_x;
                if (r < b_x) r = b_x;
                if (r < c_x) r = c_x;
                if (r < d_x) r = d_x;
                var t = a_y;
                if (t < b_y) t = b_y;
                if (t < c_y) t = c_y;
                if (t < d_y) t = d_y;
                var b = a_y;
                if (b > b_y) b = b_y;
                if (b > c_y) b = c_y;
                if (b > d_y) b = d_y;

                int left = (int)(l * mRecGridSize);
                int right = (int)(r * mRecGridSize);
                int bottom = (int)(b * mRecGridSize);
                int top = (int)(t * mRecGridSize);

                for (int ix = left; ix <= right; ++ix)
                {
                    for (int iy = bottom; iy <= top; ++iy)
                    {
                        var node = mNodes[ix + mExpandCount, iy + mExpandCount];
                        if (!node.isEmpty)
                        {
                            for (int i = 0; i < node.mList.Count; ++i)
                            {
                                var obj = node.mList[i];
                                var v2_x = obj.position.x - a_x;
                                var v2_y = obj.position.y - a_y;
                                var dot02 = v0_x * v2_x + v0_y * v2_y;
                                var dot12 = v1_x * v2_x + v1_y * v2_y;

                                var u = (dot11 * dot02 - dot01 * dot12);
                                if (u < 0 || u > deno)
                                {
                                    continue;
                                }

                                var v = (dot00 * dot12 - dot01 * dot02);
                                if (v < 0 || v > deno) // if v out of range, return directly
                                {
                                    continue;
                                }
                                if (match(obj))
                                {
                                    resultNodes.Add(obj);
                                }
                            }
                        }
                    }
                }
            }
        }
        public void SelectSector(float x, float y, float dx, float dy, float radius, float theta, List<UnitBase> resultNodes, Predicate<UnitBase> match)
        {
            float halfTheta = theta * 0.5f;

            float d = (float)(1 / Math.Sqrt(dx * dx + dy * dy));
            dx *= d;
            dy *= d;
            float n_dx = dx * radius;
            float n_dy = dy * radius;
            double sinht = Math.Sin(halfTheta);
            double cosht = Math.Cos(halfTheta);

            float x1 = (float)(n_dx * cosht - n_dy * sinht);
            float y1 = (float)(n_dx * sinht + n_dy * cosht);

            float x2 = (float)(n_dx * cosht + n_dy * sinht);
            float y2 = (float)(n_dy * cosht - n_dx * sinht);
            //计算大概边界
            float f_left, f_right, f_bottom, f_top;
            float y12 = y1 * y2;
            float x12 = x1 * x2;
            if (dx < 0 && y12 < 0)
                f_left = x - radius;
            else if (x1 < 0)
            {
                if (x1 < x2)
                    f_left = x + x1;
                else
                    f_left = x + x2;
            }
            else if (x2 < 0)
                f_left = x + x2;
            else
                f_left = x;

            if (dx > 0 && y12 < 0)
                f_right = x + radius;
            else if (x1 > 0)
            {
                if (x1 > x2)
                    f_right = x + x1;
                else
                    f_right = x + x2;
            }
            else if (x2 > 0)
                f_right = x + x2;
            else
                f_right = x;
            if (dy < 0 && x12 < 0)
                f_bottom = y - radius;
            else if (y1 < 0)
            {
                if (y1 < y2)
                    f_bottom = y + y1;
                else
                    f_bottom = y + y2;
            }
            else if (y2 < 0)
                f_bottom = y + y2;
            else
                f_bottom = y;

            if (dy > 0 && x12 < 0)
                f_top = y + radius;
            else if (y1 > 0)
            {
                if (y1 > y2)
                    f_top = y + y1;
                else
                    f_top = y + y2;
            }
            else if (y2 > 0)
                f_top = y + y2;
            else
                f_top = y;

            int left = (int)(f_left * mRecGridSize);
            int right = (int)(f_right * mRecGridSize);
            int bottom = (int)(f_bottom * mRecGridSize);
            int top = (int)(f_top * mRecGridSize);
            var sqr_radius = radius * radius;
            for (var ix = left; ix <= right; ++ix)
            {
                for (var iy = bottom; iy <= top; ++iy)
                {
                    var node = mNodes[ix + mExpandCount, iy + mExpandCount];
                    if (!node.isEmpty)
                    {
                        for (int i = 0; i < node.mList.Count; ++i)
                        {
                            var obj = node.mList[i];
                            var _x = obj.position.x - x;
                            var _y = obj.position.y - y;
                            var sqr_l = _x * _x + _y * _y;
                            bool inSector;
                            if (sqr_l <= sqr_radius)
                            {
                                float DdotU = _x * dx + _y * dy;
                                if (DdotU >= 0 && cosht >= 0)
                                    inSector = DdotU * DdotU > sqr_l * cosht * cosht;
                                else if (DdotU < 0 && cosht < 0)
                                    inSector = DdotU * DdotU < sqr_l * cosht * cosht;
                                else
                                    inSector = DdotU >= 0;
                                if (inSector && match(obj))
                                {
                                    resultNodes.Add(obj);
                                }
                            }

                        }
                    }
                }
            }

        }
        public void Destroy()
        {

        }
        /*
        public UnitBase GetNearestAlly(float x, float y, int id, int campID)
        {
            x = Mathf.Clamp(x, 0, mWidth);
            y = Mathf.Clamp(y, 0, mHeight);
            int ix = (int)(x * mRecGridSize);
            int iy = (int)(y * mRecGridSize);
            GridNode node = mNodes[ix + mExpandCount, iy + mExpandCount];

            UnitBase unit = null;
            float sqr_dis = 0;
            if(!node.isEmpty)
            {
                
                for (int i = 0; i < node.mList.Count; ++i)
                {
                    var cunit = node.mList[i];
                    if( cunit.CampID == campID && id != cunit.ID)
                    {
                        var _sqr_dis = (cunit.position.x - x) * (cunit.position.x - x) + (cunit.position.y - y) * (cunit.position.y - y);
                        if (unit == null || _sqr_dis < sqr_dis)
                        {
                            unit = cunit;
                            sqr_dis = _sqr_dis;
                        }
                    }
                }
                if (unit != null)
                    return unit;
            }
            int left = ix, right = ix,bottom = iy,top = iy;
            int level = 0;
            int count = 4;
            do
            {
                level++;
                left = ix - level;
                right = ix + level;
                bottom = iy - level;
                top = iy + level;
                count = 4;
                if(top >= mRowCount)
                {
                    count--;
                }
                else
                {
                    for(int n = left;n < right;++n)
                    {
                        var cNode = mNodes[n + mExpandCount, top + mExpandCount];
                        if(!cNode.isEmpty)
                        {
                            for (int i = 0; i < cNode.mList.Count; ++i)
                            {
                                var cunit = cNode.mList[i];
                                if (cunit.CampID == campID )
                                {
                                    var _sqr_dis = (cunit.position.x - x) * (cunit.position.x - x) + (cunit.position.y - y) * (cunit.position.y - y);
                                    if (unit == null || _sqr_dis < sqr_dis)
                                    {
                                        unit = cunit;
                                        sqr_dis = _sqr_dis;
                                    }
                                }
                            }
                        }
                    }
                }
                if (right >= mColCount)
                {
                    count--;
                }
                else
                {
                    for (int n = top; n > bottom; --n)
                    {
                        var cNode = mNodes[right + mExpandCount, n + mExpandCount];
                        if (!cNode.isEmpty)
                        {
                            for (int i = 0; i < cNode.mList.Count; ++i)
                            {
                                var cunit = cNode.mList[i];
                                if (cunit.CampID == campID )
                                {
                                    var _sqr_dis = (cunit.position.x - x) * (cunit.position.x - x) + (cunit.position.y - y) * (cunit.position.y - y);
                                    if (unit == null || _sqr_dis < sqr_dis)
                                    {
                                        unit = cunit;
                                        sqr_dis = _sqr_dis;
                                    }
                                }
                            }
                        }
                    }
                }
                if (bottom < 0)
                {
                    count--;
                }
                else
                {
                    for (int n = right; n > left; --n)
                    {
                        var cNode = mNodes[n + mExpandCount, bottom + mExpandCount];
                        if (!cNode.isEmpty)
                        {
                            for (int i = 0; i < cNode.mList.Count; ++i)
                            {
                                var cunit = cNode.mList[i];
                                if (cunit.CampID == campID)
                                {
                                    var _sqr_dis = (cunit.position.x - x) * (cunit.position.x - x) + (cunit.position.y - y) * (cunit.position.y - y);
                                    if (unit == null || _sqr_dis < sqr_dis)
                                    {
                                        unit = cunit;
                                        sqr_dis = _sqr_dis;
                                    }
                                }
                            }
                        }
                    }
                }
                if (left < 0)
                {
                    count--;
                }
                else
                {
                    for (int n = bottom; n < top; ++n)
                    {
                        var cNode = mNodes[left + mExpandCount, n + mExpandCount];
                        if (!cNode.isEmpty)
                        {
                            for (int i = 0; i < cNode.mList.Count; ++i)
                            {
                                var cunit = cNode.mList[i];
                                if (cunit.CampID == campID)
                                {
                                    var _sqr_dis = (cunit.position.x - x) * (cunit.position.x - x) + (cunit.position.y - y) * (cunit.position.y - y);
                                    if (unit == null || _sqr_dis < sqr_dis)
                                    {
                                        unit = cunit;
                                        sqr_dis = _sqr_dis;
                                    }
                                }
                            }
                        }
                    }
                }
                if (unit != null)
                    return unit;
            }
            while (count > 0);
            return null;
        }
        */
    }
}
