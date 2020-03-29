using AISTools.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AISTools.Model
{
    class ShipJourneyModel
    {
        SeamapEntities entities = null;
        public ShipJourneyModel()
        {
            entities = new SeamapEntities();
        }
        public void insert(string mmsi,float lat,float lng,string sog,string cog,string time)
        {
            var shipJourney = new ShipJourney();
            shipJourney.time = time;
            shipJourney.mmsi = mmsi;
            shipJourney.lat = lat;
            shipJourney.lng = lng;
            shipJourney.sog = sog;
            shipJourney.cog = cog;
            //if(((entities.ShipJourneys.Any(x=> x.mmsi == mmsi)) && (entities.ShipJourneys.Any(x => x.time==time)))){
            // ///
            //}
            //else
            //{
            //    entities.ShipJourneys.Add(shipJourney);
            //    entities.SaveChanges();
            //}
            entities.ShipJourneys.Add(shipJourney);
            entities.SaveChanges();
        }
    }
}
