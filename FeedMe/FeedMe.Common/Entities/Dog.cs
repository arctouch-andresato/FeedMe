using System;
using CocosSharp;
using System.Linq;
using System.Threading.Tasks;

namespace FeedMe.Common
{
    public class Dog : CCNode
    {
        CCAnimation walkAnim;
        CCAnimation dyingAnim;
        CCRepeatForever walkRepeat;
        public CCSprite Sprite;
        CCSprite deadSprite;
        CCRepeatForever walkAnimStop;
        CCAnimate catchBall;
        CCCallFuncN stop = new CCCallFuncN(node => node.StopAllActions());
        CCEventListenerTouchAllAtOnce touchListener;
        const float SPEED = 300f;

        bool isDead;

        public bool IsDead
        {
            get
            {
                return isDead;
            }
            set
            {
                isDead = value;
            }
        }

        float _positionY;

        public override float PositionY
        {
            get
            {
                return _positionY;
            }
            set
            {
                _positionY = value;
                base.PositionY = _positionY;
            }
        }

        public Dog() : base()
        {
            var spriteSheet = new CCSpriteSheet ("Animations/dog_sprites.plist","Animations/dog_sprites.png");
            var dyingSpriteSheet = new CCSpriteSheet ("Animations/dying_dog.plist","Animations/dying_dog.png");
            dyingAnim = new CCAnimation(dyingSpriteSheet.Frames.FindAll(f => f.TextureFilename.StartsWith("ghost")),0.2f);
            var walkingAnimation = spriteSheet.Frames.FindAll(f=>f.TextureFilename.StartsWith("walk_right"));
            var seatAnimation = spriteSheet.Frames.FindAll(f=>f.TextureFilename.StartsWith("seat"));
            var frame4 = spriteSheet.Frames.Find(f=>f.TextureFilename.StartsWith("seat_4"));
                seatAnimation.Add(frame4);
            seatAnimation.Add(frame4);
            seatAnimation.Add(frame4);
            seatAnimation.Add(frame4);
            seatAnimation.Add(frame4);
            seatAnimation.Add(frame4);

            walkAnim = new CCAnimation (walkingAnimation, 0.1f);
            walkRepeat = new CCRepeatForever( new CCAnimate (walkAnim));
            walkAnimStop = new CCRepeatForever(new CCAnimate(new CCAnimation(seatAnimation, 0.2f)));
            catchBall = new CCAnimate(new CCAnimation(spriteSheet.Frames.FindAll(f => f.TextureFilename.StartsWith("ball_right")), 0.1f));
            deadSprite = new CCSprite(dyingSpriteSheet.Frames.First());
            deadSprite.Scale = 3f;
            Sprite = new CCSprite(walkingAnimation.First()){ Name = "Dog" };;
            Sprite.Scale = 3f;

            Sprite.RunAction(walkAnimStop);
            this.AddChild(Sprite);

            touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesEnded = HandleInput;
            AddEventListener(touchListener, this);
        }

        private void HandleInput(System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
        {
            if (!IsDead)
            {
                Sprite.StopAllActions();
                Sprite.PositionY = _positionY;
                if (this.Children.Contains(deadSprite))
                {
                    this.RemoveChild(deadSprite);
                    deadSprite.Dispose();
                }

                var location = touches[0].LocationOnScreen;
                location = ScreenToWorldspace(location);
                float ds = CCPoint.Distance(new CCPoint(Sprite.Position.X, Sprite.PositionY), new CCPoint(location.X, Sprite.PositionY));
                var dt = ds / SPEED;


                if (touches.Count > 0)
                {
                    CCTouch firstTouch = touches[0];

                    if (Sprite.PositionWorldspace.X > location.X)
                        Sprite.FlipX = true;
                    else
                        Sprite.FlipX = false;

                    var move = new CCMoveTo(dt, new CCPoint(location.X, Sprite.PositionY));

                    Sprite.RunAction(walkRepeat);
                    Sprite.RunActions(move, stop);
                    Sprite.ScheduleOnce((t => Idle()), dt);
                } 
            }

        }

        private void Idle()
        {
            IsDead = false;
            Sprite.PositionY = _positionY;
            if (this.Children.Contains(deadSprite))
            {
                this.RemoveChild(deadSprite);
                deadSprite.Dispose();
            }
            Sprite.StopAllActions();
            Sprite.RunAction(walkAnimStop);
        }

        public void CatchBall()
        {
            Sprite.StopAllActions();
            Sprite.RunAction(catchBall);
        }

        public void PlayDead()
        {
            IsDead = true;
            Sprite.StopAllActions();
            deadSprite.Position = Sprite.Position;
            deadSprite.PositionY = deadSprite.PositionY - 15f;
            this.AddChild(deadSprite);
            Sprite.RunAction(new CCRepeat(new CCAnimate(dyingAnim),12));
            var moveAction = new CCRepeat(new CCMoveBy(1, new CCPoint(0, 30)),4);
            Sprite.RunAction(moveAction);


            Sprite.ScheduleOnce((t=>Idle()), 5);
        }
    }
}

