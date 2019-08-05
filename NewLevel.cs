using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DirectX11TutorialLevelEditor
{
    public partial class NewLevel : Form
    {
        public NewLevel()
        {
            InitializeComponent();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (tbName.Text == "")
            {
                MessageBox.Show("맵 이름은 공백일 수 없습니다.", "알림",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.None;

                return;
            }

            if (numSizeX.Value == 0)
            {
                MessageBox.Show("맵의 가로 크기는 0보다 커야 합니다.", "알림",
                   MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.None;

                return;
            }

            if (numSizeY.Value == 0)
            {
                MessageBox.Show("맵의 세로 크기는 0보다 커야 합니다.", "알림",
                   MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.None;

                return;
            }
        }
    }
}
