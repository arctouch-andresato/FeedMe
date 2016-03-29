using System;
using CocosSharp;

namespace FeedMe.Common
{
    public class BallFactory
    {
        static Lazy<BallFactory> self = 
            new Lazy<BallFactory>(()=>new BallFactory());

        // simple singleton implementation
        public static BallFactory Self
        {
            get
            {
                return self.Value;
            }
        }

        public event Action<Ball> BallCreated;

        private BallFactory()
        {

        }

        public Ball CreateNew(CCPoint? location=null)
        {
            Ball newBall = new Ball ();
            if(location.HasValue)
                newBall.Sprite.Position = location.Value;

            if (BallCreated != null)
            {
                BallCreated (newBall);
            }

            return newBall;
        }

        public Ball CreateNewRandomPosition(int maxX, int maxY, int minX, int minY)
        {
            var random = new Random(DateTime.Now.Millisecond);
            CCPoint location = new CCPoint(random.Next(minX, maxX), random.Next(minY, maxY));

            Ball ball = CreateNew(location);
            return ball;
        }
    }
}

