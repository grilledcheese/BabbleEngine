using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;

namespace BabbleEngine
{
    public class NodeManager
    {
        private Dictionary<String, LinkedList<Vector2>> nodeDictionary = new Dictionary<string, LinkedList<Vector2>>();

        public void AddNode(String s, Vector2 p)
        {
            if (!nodeDictionary.ContainsKey(s))
                nodeDictionary.Add(s, new LinkedList<Vector2>());
            
            nodeDictionary[s].AddFirst(p);
        }

        public Tuple<String, Vector2> FindFirstNodeAtPosition(Vector2 p)
        {
            foreach (Tuple<String, Vector2> t in GetAllNodes())
                if (t.Item2 == p)
                    return t;
            return null;
        }

        public void RemoveFirstNodeAtPosition(Vector2 p)
        {
            foreach (String e in GetNames())
            {
                if (GetNodeList(e).Remove(p))
                    return;
            }
        }

        public ICollection<String> GetNames()
        {
            return nodeDictionary.Keys;
        }

        /// <summary>
        /// Returns a list of nodes with this name.
        /// </summary>
        public LinkedList<Vector2> GetNodeList(String name)
        {
            return nodeDictionary[name];
        }

        /// <summary>
        /// Returns the first node with this name.
        /// </summary>
        public Vector2 GetNode(String name)
        {
            return nodeDictionary[name].First.Value;
        }

        public IEnumerable<Tuple<String,Vector2>> GetAllNodes()
        {
            foreach (String e in GetNames())
            {
                foreach (Vector2 v in GetNodeList(e))
                {
                    yield return new Tuple<String, Vector2>(e, v);
                }
            }
        }
    }
}
