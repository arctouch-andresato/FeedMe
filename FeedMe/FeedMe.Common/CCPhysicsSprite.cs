using System;
using CocosSharp;
using Box2D.Dynamics;
using Box2D.Common;

namespace FeedMe.Common
{
    public class CCPhysicsSprite : CCSprite
    {
        readonly float ptmRatio;

        public CCPhysicsSprite (CCSpriteFrame f, float ptmRatio) : base (f)
        {
            this.ptmRatio = ptmRatio;
        }

        public b2Body PhysicsBody { get; set; }

        public void UpdateBallTransform()
        {
            if (PhysicsBody != null)
            {
                b2Vec2 pos = PhysicsBody.Position;

                float x = pos.x * ptmRatio;
                float y = pos.y * ptmRatio;

                if (IgnoreAnchorPointForPosition) 
                {
                    x += AnchorPointInPoints.X;
                    y += AnchorPointInPoints.Y;
                }

                // Make matrix
                float radians = PhysicsBody.Angle;
                var c = (float)Math.Cos (radians);
                var s = (float)Math.Sin (radians);

                if (!AnchorPointInPoints.Equals (CCPoint.Zero)) 
                {
                    x += c * -AnchorPointInPoints.X + -s * -AnchorPointInPoints.Y;
                    y += s * -AnchorPointInPoints.X + c * -AnchorPointInPoints.Y;
                }

                Position = new CCPoint(x, y);
            }
        }
    }
}

