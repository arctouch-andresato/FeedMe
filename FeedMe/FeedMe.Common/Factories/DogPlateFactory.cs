using System;
using CocosSharp;

namespace FeedMe.Common
{
    public class DogPlateFactory
    {
        static Lazy<DogPlateFactory> self = 
            new Lazy<DogPlateFactory>(()=>new DogPlateFactory());
        Random random = new Random(DateTime.Now.Millisecond);
        // simple singleton implementation
        public static DogPlateFactory Self
        {
            get
            {
                return self.Value;
            }
        }

        public event Action<DogPlate> DogPlateCreated;

        private DogPlateFactory()
        {

        }

        public DogPlate CreateNew(CCPoint? location=null)
        {
            DogPlate newDogPlate = new DogPlate ();
            if(location.HasValue)
                newDogPlate.Sprite.Position = location.Value;

            if (DogPlateCreated != null)
            {
                DogPlateCreated (newDogPlate);
            }

            return newDogPlate;
        }

        public DogPlate CreateNewRandomPosition(int maxX, int maxY, int minX, int minY)
        {
            
            CCPoint location = new CCPoint(random.Next(minX, maxX), random.Next(minY, maxY));

            DogPlate dogPlate = CreateNew(location);
            return dogPlate;
        }
    }
}

