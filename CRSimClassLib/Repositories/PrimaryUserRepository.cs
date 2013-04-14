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
            var mobileUsers = Simulation.GetMobileStations();
            var randomLocation = mobileUsers[RandomNumberRepository.Instance.NextInt(0, mobileUsers.Count)].GetLocation();
            var randomPoint = new TerrainPoint(RandomNumberRepository.Instance.GetNextDouble(-20, 20),
                RandomNumberRepository.Instance.GetNextDouble(-20, 20));
            randomLocation += randomPoint;

            var pu = PrimaryUser.CreatePrimaryUser(randomLocation, SimParameters.PUTransmissionPower);

            var nextEventTime = RandomNumberRepository.Instance.NextInt(0, SimParameters.MaximalRemovePUAfter);
            Simulation.EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(nextEventTime), () => RemovePrimaryUser(pu)));

            Simulation.GetTerrain().PrimaryUsers.Add(pu);
        }

        public void RemovePrimaryUser(PrimaryUser primaryUser)
        {
            var terrain = Simulation.GetTerrain();

            terrain.PrimaryUsers.Remove(primaryUser);

            var nextEventTime = RandomNumberRepository.Instance.NextInt(0, SimParameters.MaximalCreatePUAfter);

            Simulation.EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(nextEventTime), () => CreateAndAddPrimaryUser()));
        }
    }
}
