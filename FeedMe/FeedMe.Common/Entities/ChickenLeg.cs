using System;
using CocosSharp;

namespace FeedMe.Common
{
    public class ChickenLeg:CCNode
    {
        public CCSprite Sprite;
        CCRepeatForever plateAnimated;

        public ChickenLeg(CCPoint? location=null) : base()
        {
            Sprite = new CCSprite("chicken_leg.png");
            // Center the Sprite in this entity to simplify
            // centering the Ship on screen
            Sprite.Scale = 3f;
            Sprite.AnchorPoint = CCPoint.AnchorMiddle;
            this.AddChild(Sprite);
            if (location.HasValue)
                this.Position = location.Value;
            plateAnimated = new CCRepeatForever(new CCRotateBy(0.2f,100f));
            var moveAction = new CCRepeatForever(new CCMoveBy(1, new CCPoint(0, -130)));
            Sprite.RunAction(plateAnimated);
            Sprite.RunAction(moveAction);
        }
    }
}

