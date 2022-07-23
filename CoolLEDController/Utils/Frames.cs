using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolLEDController.Utils
{
    public class LEDState
    {
        public bool data;
        public LEDState(bool data)
        {
            this.data = data;
        }
    }

    public class Frame
    {
        public List<LEDState> panelStates;

        public Frame(List<LEDState> data)
        {
            this.panelStates = data;
        }

        // Only use if adding rows manually
        public Frame() : this(new List<LEDState>()) { }

        public void AddRow(List<LEDState> row)
        {
            panelStates.AddRange(row);
        }
    }

    public class Frames
    {
        public List<Frame> animationStates;

        public int speed = 50;

        public Frames(List<Frame> animationStates, int speed)
        {
            this.animationStates = animationStates;
            this.speed = speed;
            if (this.speed > 256) this.speed = 256;
        }

        public void AddFrame(Frame frame)
        {
            animationStates.Add(frame);
        }
    }
}
