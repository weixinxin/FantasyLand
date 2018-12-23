using BattleSystem.ObjectModule;
using BattleSystem.SpaceModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem.SkillModule
{
    public abstract class AoeRegion
    {
        public WorldSpace world;
        public abstract bool Select(List<UnitBase> resultNodes, Predicate<UnitBase> match);
    }

    public class CircleRegion : AoeRegion
    {
        private float x,y,r;
        public CircleRegion(WorldSpace w,float x,float y,float r)
        {
            this.world = w;
            this.x = x;
            this.y = y;
            this.r = r;
        }
        public override bool Select(List<UnitBase> resultNodes,Predicate<UnitBase> match)
        {
            world.SelectCircle(x, y, r, resultNodes, match);
            return resultNodes.Count > 0;
        }
    }

    public class RectRegion :AoeRegion
    {
        private float x, y, rx, ry, width, height;
        public RectRegion(WorldSpace w, float x, float y, float rx, float ry, float width, float height)
        {
            this.world = w;
            this.x = x;
            this.y = y;
            this.rx = rx;
            this.ry = ry;
            this.width = width;
            this.height = height;
        }
        public override bool Select(List<UnitBase> resultNodes, Predicate<UnitBase> match)
        {
            world.SelectRect(x, y, rx,ry,width,height,resultNodes, match);
            return resultNodes.Count > 0;
        }
    }

    public class SectorRegion :AoeRegion
    {
        private float x, y, rx, ry, radius, theta;
        public SectorRegion(WorldSpace w, float x, float y, float rx, float ry, float radius, float theta)
        {
            this.world = w;
            this.x = x;
            this.y = y;
            this.rx = rx;
            this.ry = ry;
            this.radius = radius;
            this.theta = theta;
        }
        public override bool Select(List<UnitBase> resultNodes, Predicate<UnitBase> match)
        {
            world.SelectSector(x, y, rx, ry, radius, theta, resultNodes, match);
            return resultNodes.Count > 0;
        }
    }
}
