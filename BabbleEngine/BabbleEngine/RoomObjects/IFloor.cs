using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BabbleEngine
{
    /// <summary>
    /// An interface for an object that can be stood on. Such objects must provide
    /// a value for how much slower an object will be walking on it; and a sign indicating
    /// which direction is hard to walk up.
    /// </summary>
    public interface IFloor
    {
        float GetSlopeSlow();
        float GetSlopeSign();
    }
}
