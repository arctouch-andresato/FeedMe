using System;
using CocosSharp;

namespace FeedMe.Common
{
    public class ChickenLegFactory
    {
        static Lazy<ChickenLegFactory> self = 
            new Lazy<ChickenLegFactory>(()=>new ChickenLegFactory());

        // simple singleton implementation
        public static ChickenLegFactory Self
        {
            get
            {
                return self.Value;
            }
        }

        public event Action<ChickenLeg> ChickenLegCreated;

        private ChickenLegFactory()
        {

        }

        public ChickenLeg CreateNew(CCPoint? location=null)
        {
            ChickenLeg newChickenLeg = new ChickenLeg ();
            if(location.HasValue)
                newChickenLeg.Sprite.Position = location.Value;

            if (ChickenLegCreated != null)
            {
                ChickenLegCreated (newChickenLeg);
            }

            return newChickenLeg;
        }

        public ChickenLeg CreateNewRandomPosition(int maxX, int maxY, int minX, int minY)
        {
            var random = new Random();
            CCPoint location = new CCPoint(random.Next(minX, maxX), random.Next(minY, maxY));

            ChickenLeg chickenLeg = CreateNew(location);
            return chickenLeg;
        }
    }
}

