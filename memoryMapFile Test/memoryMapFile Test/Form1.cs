using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace memoryMapFile_Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void writeButton_Click(object sender, EventArgs e)
        {
            byte[] Buffer = ASCIIEncoding.ASCII.GetBytes(readTextBox.Text);
            MemoryMappedFile mmf = MemoryMappedFile.CreateNew("test", 1000);
            MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor();
            accessor.Write(54, (ushort)Buffer.Length);
            accessor.WriteArray(54 + 2, Buffer, 0, Buffer.Length);
        }

        private void readButton_Click(object sender, EventArgs e)
        {
            MemoryMappedFile mmf = MemoryMappedFile.OpenExisting("test");
            MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor();
            ushort Size = accessor.ReadUInt16(54);
            byte[] Buffer = new byte[Size];
            accessor.ReadArray(54 + 2, Buffer, 0, Buffer.Length);
            MessageBox.Show(ASCIIEncoding.ASCII.GetString(Buffer));
        }
    }
}
