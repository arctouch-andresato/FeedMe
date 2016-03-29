using System;
using CocosSharp;

namespace FeedMe.Common
{
    public class DogPlate : CCNode
    {
        public CCSprite Sprite;
        CCRepeatForever plateAnimated;

        public DogPlate(CCPoint? location=null) : base()
        {
            Sprite = new CCSprite("dog_plate_red.png");
            // Center the Sprite in this entity to simplify
            // centering the Ship on screen
            Sprite.Scale = 3f;
            Sprite.AnchorPoint = CCPoint.AnchorMiddle;
            this.AddChild(Sprite);
            if (location.HasValue)
                this.Position = location.Value;
            plateAnimated = new CCRepeatForever(new CCRotateBy(0.2f,50f));
            var moveAction = new CCRepeatForever(new CCMoveBy(1, new CCPoint(0, -150)));
            Sprite.RunAction(plateAnimated);
            Sprite.RunAction(moveAction);
        }
    }
}

