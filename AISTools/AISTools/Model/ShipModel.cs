using AISTools.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AISTools.Model
{
    class ShipModel
    {
        SeamapEntities entities = null;
        public ShipModel() {
            entities = new SeamapEntities();
        }
        public void insert(string mmsi,string vsnm,int type,string _class)
        {
            var ship = new Ship();
            ship.@class = _class;
            ship.mmsi = mmsi;
            ship.vsnm = vsnm;
            ship.type = type;
            entities.Ships.Add(ship);
            entities.SaveChanges();

        }
        public HashSet<string> getAll()
        {
            var a = (from mmsi in entities.Ships select mmsi);
            var b = a.Select(p => p.mmsi).ToList();
            return new HashSet<string>(b);
           
        }
    }
}
