using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourneeFutee
{
    internal class Vertex
    {
        private string name;
        private float value;
        private List<Vertex> neighbor = new List<Vertex>();
        public Vertex(string name, float value = 0)
        {
            this.name = name;
            this.value = value;
        }

        public string Name
        {
            get { return name; }
            set { this.name = value; }  //A verif
        }
        public float Value
        {
            get { return value; }
            set { this.value = value; }
        }
        public List<Vertex> Neighbor
        {
            get { return neighbor; }
            set { this.neighbor = value; }
        }
    }
}
