using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sunny.UI;

namespace Framework2Core
{
    public partial class StationSelectForm : UIForm
    {
        public int stationNum = -1;
        public StationSelectForm()
        {
            InitializeComponent();
        }

        private void StationDoubleClick(object sender, EventArgs e)
        {
            if (ReferenceEquals(sender, uiStationButton1))
            {
                stationNum = 3;
            }
            if (ReferenceEquals(sender, uiStationButton2))
            {
                stationNum = 4;
            }
            if (ReferenceEquals(sender, uiStationButton3))
            {
                stationNum = 6;
            }
            if (ReferenceEquals(sender, uiStationButton4))
            {
                stationNum = 8;
            }
            if (ReferenceEquals(sender, uiStationButton5))
            {
                stationNum = 9;
            }
            if (ReferenceEquals(sender, uiStationButton25))
            {
                stationNum = 29;
            }
            if (ReferenceEquals(sender, uiStationButton6))
            {
                stationNum = 11;
            }
            Dispose();
        }

        private void uiOKButton_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
