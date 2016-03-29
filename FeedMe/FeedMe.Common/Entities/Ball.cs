using System;
using CocosSharp;

namespace FeedMe.Common
{
    public class Ball : CCNode
    {
        public CCPhysicsSprite Sprite;

        CCEventListenerTouchAllAtOnce touchListener;

        CCRepeatForever ballAnimated;

        public Ball(CCPoint? location=null) : base()
        {
            var spriteSheet = new CCSpriteSheet ("Animations/dog_sprites.plist","Animations/dog_sprites.png");
            Sprite = new CCPhysicsSprite (spriteSheet.Frames.Find(f=>f.TextureFilename.StartsWith("ball_big_2")),32);
            // Center the Sprite in this entity to simplify
            // centering the Ship on screen
            Sprite.Scale = 3f;
            Sprite.AnchorPoint = CCPoint.AnchorMiddle;
            this.AddChild(Sprite);
            if (location.HasValue)
                this.Position = location.Value;
            ballAnimated = new CCRepeatForever(new CCRotateBy(0.2f,100f));
            Sprite.RunAction(ballAnimated);

        }



    }
}

