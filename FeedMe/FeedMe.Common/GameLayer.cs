using System;
using System.Collections.Generic;
using CocosSharp;
using FeedMe.Common;
using Box2D.Dynamics;
using Box2D.Common;
using Box2D.Collision.Shapes;

namespace FeedMe
{
    public class GameLayer : CCLayerColor
    {
        Dog dog;
        List<Ball> balls;
        List<ChickenLeg> chickenLegs;
        List<DogPlate> dogPlates;
        // physics world
        b2World world;
        // point to meter ratio for physics
        const int PTM_RATIO = 32;
        Random random;
        CCLabel lifeLabel;
        CCSprite background;
        public GameLayer()
            : base(CCColor4B.AliceBlue)
        {
            background = new CCSprite("full_background.png");
            background.ZOrder = -6;
            this.AddChild(background);
            
            balls = new List<Ball>();
            chickenLegs = new List<ChickenLeg>();
            dogPlates = new List<DogPlate>();
            // Load and instantate your assets here
            dog = new Dog();
            dog.PositionX = 40;
            dog.PositionY = 90;
            // Make any renderable node objects (e.g. sprites) children of this layer
            this.AddChild(dog);
            random = new Random();



            lifeLabel = new CCLabel("life=","arial",56f);
            lifeLabel.Position = new CCPoint(0,VisibleBoundsWorldspace.MaxY);
            lifeLabel.AnchorPoint = CCPoint.AnchorMiddle;
            lifeLabel.Color = CCColor3B.Black;
            this.AddChild(lifeLabel);

            BallFactory.Self.BallCreated += HandleBallCreated;
            ChickenLegFactory.Self.ChickenLegCreated += HandleChickenLegCreated;
            DogPlateFactory.Self.DogPlateCreated += HandleDogPlateCreated;

            Schedule (t => CheckCollision ());
            Schedule(t => {
                world.Step (t, 8, 1);
                var newBalls = 0;
                var deletedBalls = new List<Ball>();
                var deletedChickens = new List<ChickenLeg>();
                var deletedPlates = new List<DogPlate>();
                foreach (Ball ball in balls) {
                    if (ball.Sprite.Visible && ball.Sprite.PhysicsBody.Position.x < 0f || ball.Sprite.PhysicsBody.Position.x * PTM_RATIO > ContentSize.Width) { //or should it be Layer.VisibleBoundsWorldspace.Size.Width
                        world.DestroyBody (ball.Sprite.PhysicsBody);
                        this.RemoveChild(ball);
                        deletedBalls.Add(ball);
                        newBalls ++;
                    } else {
                        ball.Sprite.UpdateBallTransform();
                    }
                }
                foreach(ChickenLeg chicken in chickenLegs)
                {
                    if (chicken.Sprite.Visible && chicken.Sprite.Position.Y < 0f ) { //or should it be Layer.VisibleBoundsWorldspace.Size.Width
                        this.RemoveChild(chicken);
                        deletedChickens.Add(chicken);
                    } 
                }

                foreach(DogPlate plate in dogPlates)
                {
                    if (plate.Sprite.Visible && plate.Sprite.Position.Y < 0f ) { //or should it be Layer.VisibleBoundsWorldspace.Size.Width
                        this.RemoveChild(plate);
                        deletedPlates.Add(plate);
                    } 
                }

                foreach (var ball in deletedBalls)
                {
                    balls.Remove(ball);
                    ball.Dispose();
                }

                foreach (var chicken in deletedChickens)
                {
                    chickenLegs.Remove(chicken);
                    chicken.Dispose();
                }

                foreach (var plate in deletedPlates)
                {
                    dogPlates.Remove(plate);
                    plate.Dispose();
                }
                for(int i=0; i < newBalls;i++)
                {
                    BallFactory.Self.CreateNewRandomPosition((int)VisibleBoundsWorldspace.MaxX, 800,50,600);
                }
            });

            Schedule((t =>
                {
                    ChickenLegFactory.Self.CreateNewRandomPosition((int)VisibleBoundsWorldspace.MaxX,(int)VisibleBoundsWorldspace.MaxY,50,(int)VisibleBoundsWorldspace.MaxY);
                }), 10);

            Schedule((t =>
                {
                    DogPlateFactory.Self.CreateNewRandomPosition((int)VisibleBoundsWorldspace.MaxX,(int)VisibleBoundsWorldspace.MaxY,50,(int)VisibleBoundsWorldspace.MaxY);
                }), 3);
                


        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            // Use the bounds to layout the positioning of our drawable assets
            CCRect bounds = VisibleBoundsWorldspace;

            // Register for touch events
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesEnded = OnTouchesEnded;
            AddEventListener(touchListener, this);
        }

        void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0)
            {
                
                // Perform touch handling here
            }
        }

        void CheckCollision ()
        {
            var newBalls = 0;
            var deletedBalls = new List<Ball>();
            var deletedChickens = new List<ChickenLeg>();
            var deletedPlates = new List<DogPlate>();

            if (balls.Count >= 1 && !dog.IsDead)
            {
                foreach (var ball in balls)
                {
                    bool hit = ball.Sprite.BoundingBoxTransformedToWorld.IntersectsRect(dog.Sprite.BoundingBoxTransformedToWorld);
                    if (hit && this.Children.Contains(ball))
                    {
                        dog.CatchBall();
                        world.DestroyBody(ball.Sprite.PhysicsBody);
                        this.RemoveChild(ball);
                        deletedBalls.Add(ball);
                        newBalls = 1;
                    }
                }

                foreach (var chicken in chickenLegs)
                {
                    bool hit = chicken.Sprite.BoundingBoxTransformedToWorld.IntersectsRect(dog.Sprite.BoundingBoxTransformedToWorld);
                    if (hit && this.Children.Contains(chicken))
                    {
                        this.RemoveChild(chicken);
                        deletedChickens.Add(chicken);
                    }

                }

                foreach (var plate in dogPlates)
                {
                    bool hit = plate.Sprite.BoundingBoxTransformedToWorld.IntersectsRect(dog.Sprite.BoundingBoxTransformedToWorld);
                    if (hit && this.Children.Contains(plate))
                    {
                        dog.PlayDead();
                        this.RemoveChild(plate);
                        deletedPlates.Add(plate);
                    }

                }

                foreach (var ball in deletedBalls)
                {
                    balls.Remove(ball);
                    ball.Dispose();
                }

                foreach (var chicken in deletedChickens)
                {
                    chickenLegs.Remove(chicken);
                    chicken.Dispose();
                }

                foreach (var ball in deletedBalls)
                {
                    balls.Remove(ball);
                    ball.Dispose();
                }
                for(int i=0; i < newBalls;i++)
                {
                    if(balls.Count < 10)
                        BallFactory.Self.CreateNewRandomPosition((int)VisibleBoundsWorldspace.MaxX, 800,0,(int)VisibleBoundsWorldspace.MinY+50);
                }
            }
        }

        void HandleBallCreated(Ball newBall)
        {
            this.AddChild(newBall,1,1);
            balls.Add(newBall);

            var def = new b2BodyDef ();
                def.position = new b2Vec2 (newBall.Sprite.Position.X / PTM_RATIO, newBall.Sprite.Position.Y / PTM_RATIO);
            def.linearVelocity = new b2Vec2 ((float)(random.Next(4) + random.NextDouble() - 2),(float)(random.Next(4) + random.NextDouble() - 2));
            def.type = b2BodyType.b2_dynamicBody;
            b2Body body = world.CreateBody (def);

            var circle = new b2CircleShape ();
            circle.Radius = 0.5f;

            var fd = new b2FixtureDef ();
            fd.shape = circle;
            fd.density = 1f;
            fd.restitution = 0.85f;
            fd.friction = 0f;
            body.CreateFixture (fd);
            //body.World.

            newBall.Sprite.PhysicsBody = body;

        }

        void HandleChickenLegCreated(ChickenLeg chickenLeg)
        {
            chickenLegs.Add(chickenLeg);
            this.AddChild(chickenLeg, 1, 1);

        }
        void HandleDogPlateCreated(DogPlate dogPlate)
        {
            dogPlates.Add(dogPlate);
            this.AddChild(dogPlate,1,1);
        }

        void InitPhysics ()
        {
            CCSize s = Layer.VisibleBoundsWorldspace.Size;

            var gravity = new b2Vec2 (0.0f, -10.0f);
            world = new b2World (gravity);

            world.SetAllowSleeping (true);
            world.SetContinuousPhysics (true);

            var def = new b2BodyDef ();
            def.allowSleep = true;
            def.position =  new b2Vec2(0,40);
            def.type = b2BodyType.b2_staticBody;
            b2Body groundBody = world.CreateBody (def);
            groundBody.SetActive (true);

            b2EdgeShape groundBox = new b2EdgeShape ();
            groundBox.Set (new b2Vec2(0,40), new b2Vec2 (s.Width / PTM_RATIO, 40));
            b2FixtureDef fd = new b2FixtureDef ();
            fd.shape = groundBox;
            groundBody.CreateFixture (fd);

            var rightSideDef = new b2BodyDef ();
            rightSideDef.allowSleep = true;
            rightSideDef.position = new b2Vec2((s.Width/ PTM_RATIO)-12, 60);
            rightSideDef.type = b2BodyType.b2_staticBody;
            var rightSideBody = world.CreateBody (rightSideDef);
            rightSideBody.SetActive (true);

            var rightSideBox = new b2EdgeShape ();
            rightSideBox.Set (new b2Vec2((s.Width/ PTM_RATIO)-12, 60), new b2Vec2 ((s.Width / PTM_RATIO)-12, s.Height / PTM_RATIO));
            var rightSideFd = new b2FixtureDef ();
            rightSideFd.shape = rightSideBox;
            rightSideBody.CreateFixture (rightSideFd);

            var leftSideDef = new b2BodyDef ();
            leftSideDef.allowSleep = true;
            leftSideDef.position = new b2Vec2(0,60);
            leftSideDef.type = b2BodyType.b2_staticBody;
            var leftSideBody = world.CreateBody (leftSideDef);
            leftSideBody.SetActive (true);

            var leftSideBox = new b2EdgeShape ();
            leftSideBox.Set (new b2Vec2(0,60), new b2Vec2 (0, s.Height / PTM_RATIO));
            var leftSideFd = new b2FixtureDef ();
            leftSideFd.shape = leftSideBox;
            leftSideBody.CreateFixture (leftSideFd);

        }

        public override void OnEnter ()
        {
            base.OnEnter ();

            InitPhysics ();

            BallFactory.Self.CreateNewRandomPosition((int)VisibleBoundsWorldspace.MaxX, 800,50,600);

            lifeLabel.Position = new CCPoint(30,VisibleBoundsWorldspace.MaxY-40);
            background.ScaleY = VisibleBoundsWorldspace.MaxY / background.ContentSize.Height;
            background.AnchorPoint = CCPoint.AnchorLowerLeft;
        }


    }
}
