using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG
{
    class Animation
    {
        private int start;
        private int end;

        private int curFrame;

        public Animation(int start, int end)
        {
            this.start = start;
            this.end = end;
            curFrame = end;
        }

        public void advanceFrame()
        {
            curFrame++;
            if (curFrame > end)
                curFrame = start;
        }

        public int getFrame()
        {
            return curFrame;
        }

        public void resetStart()
        {
            curFrame = start;
        }

        public void resetEnd()
        {
            curFrame = end;
        }
    }
}
