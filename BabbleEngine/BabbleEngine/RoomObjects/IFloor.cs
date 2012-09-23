using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BabbleEngine
{
    public interface IFloor
    {
        float GetSlopeSlow();
        float GetSlopeSign();
    }
}
