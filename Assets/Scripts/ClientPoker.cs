using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class ClientPoker
    {
        private int id;
        private bool isOut;

        public int ID
        {
            set { id = value; }
            get { return id; }
        }

        public bool IsOut
        {
            set { isOut = value; }
            get { return isOut; }
        }
    }
}
