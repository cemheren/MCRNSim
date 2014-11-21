using CRSimClassLib.TerrainModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRSimClassLib.Repositories
{
    public class PrimaryUserRepository
    {
        public void CreateAndAddPrimaryUser()
        {
            var mobileUsers = Simulation.Instance.GetMobileStations();
            var randomLocation = mobileUsers[RandomNumberRepository.Instance.NextInt(0, mobileUsers.Count)].GetLocation();
            var randomPoint = new TerrainPoint(RandomNumberRepository.Instance.GetNextDouble(-20, 20),
                RandomNumberRepository.Instance.GetNextDouble(-20, 20));
            randomLocation += randomPoint;

            var pu = PrimaryUser.CreatePrimaryUser(randomLocation, SimParameters.PUTransmissionPower);

            var nextEventTime = (int)RandomNumberRepository.Instance.ExponantialRV(SimParameters.PUTalkDuration);
            Simulation.Instance.EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(nextEventTime), () => RemovePrimaryUser(pu)));

            Simulation.Instance.GetTerrain().PrimaryUsers.Add(pu);

            Statistics.LastTimeAPUCreated = Time.Instance.Now;
        }

        public void RemovePrimaryUser(PrimaryUser primaryUser)
        {
            var terrain = Simulation.Instance.GetTerrain();

            terrain.PrimaryUsers.Remove(primaryUser);

            var nextEventTime = (int)RandomNumberRepository.Instance.ExponantialRV(SimParameters.PUInterArrival);

            Simulation.Instance.EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(nextEventTime), () => CreateAndAddPrimaryUser()));

            Statistics.LastTimeAPURemoved = Time.Instance.Now;
        }
    }
}
