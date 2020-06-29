using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using VlcPlayer;

namespace VLCTest
{
	public partial class Form1 : Form
	{
		public bool m_bFullScreen = false;
		VlcPlayer.VlcPlayerBase VlcPlayerBase = new VlcPlayer.VlcPlayerBase(Environment.CurrentDirectory + "\\vlc\\plugins\\", "");
		VlcPlayer.VlcPlayerBase VlcPlayerSub = new VlcPlayer.VlcPlayerBase(Environment.CurrentDirectory + "\\vlc\\plugins\\", "--video-filter=transform{type=hflip}");
		string mainName = string.Empty;
		string subName = string.Empty;
		string[] files = { };

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{



		}

		private void playMedia(string mainPath, string subPath)
        {
			Panel panelDoubleClick = new Panel();       // this panel requires to catche double click evetns.
			panelDoubleClick.Dock = DockStyle.Fill;
			panelDoubleClick.BackColor = Color.Transparent;
			panelDoubleClick.MouseDoubleClick += pictureBox1_DoubleClick;
			pictureBox1.Controls.Add(panelDoubleClick);
			panelDoubleClick.BringToFront();



			VlcPlayerBase.SetRenderWindow(pictureBox1.Handle.ToInt32());
			VlcPlayerBase.LoadFile(mainPath);			// "E:\\VAVA\\VIDEO\\2020_0624_172324_310F.MP4");   //银河与极光.mp4");
			VlcPlayerBase.Play();


			VlcPlayerSub.SetRenderWindow(pictureBox2.Handle.ToInt32());
			//VlcPlayerSub.LoadFile("E:\\VAVA\\VIDEO\\2020_0624_171821_308B.MP4");
			VlcPlayerSub.LoadFile(subPath);				// "C:\\Users\\Tony\\Desktop\\tmp\\01D2BD_V-2.mp4");

			VlcPlayerSub.SetVolume(0);
			//VlcPlayerSub.SetOrient("ivtc");
			VlcPlayerSub.Play();

//			VlcPlayerSub.Stop();
//			VlcPlayerBase.Stop();

		}

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
			if (!m_bFullScreen)
			{
				this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
				this.WindowState	= System.Windows.Forms.FormWindowState.Maximized;
				m_bFullScreen		= true;
            }
            else
            {
				this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
				this.WindowState = System.Windows.Forms.FormWindowState.Normal;
				m_bFullScreen = false;
			}
		}
		private void form1_keyDown(object sender, KeyEventArgs e)
        {
			Console.WriteLine(e.KeyData);
			Console.WriteLine(e.KeyCode);
			Console.WriteLine(e.KeyValue);
			if (e.KeyValue == 32)								// 32 Space key
			{
				if (VlcPlayerBase.IsPlaying)
				{
					VlcPlayerBase.Pause();
					VlcPlayerSub.Pause();
                }
                else
                {
					VlcPlayerBase.Play();
					VlcPlayerSub.Play();
				}
			}
			if (e.KeyCode == Keys.Enter)
            {
				if (!m_bFullScreen)
				{
					this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
					this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
					m_bFullScreen = true;
				}
				else
				{
					this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
					this.WindowState = System.Windows.Forms.FormWindowState.Normal;
					m_bFullScreen = false;
				}
			}
			if (e.KeyCode == Keys.Escape)
            {
				this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
				this.WindowState = System.Windows.Forms.FormWindowState.Normal;
				m_bFullScreen = false;
			}
        }
		private void form1_KeyUp(object sender, KeyEventArgs e)
		{
			string selFileName = string.Empty;
			string selPathName = string.Empty;
			string selExtName = string.Empty;
			int selListNo = 0;
			string fbName = string.Empty;

			if (e.Control)
            {
				Console.WriteLine(e.KeyData);
				Console.WriteLine(e.KeyCode);
				Console.WriteLine(e.KeyValue);

				if (e.KeyCode.ToString() == "O")
				{
					//MessageBox.Show(e.KeyCode.ToString());
					OpenFileDialog ofd = new OpenFileDialog();
					if (ofd.ShowDialog() == DialogResult.OK)
					{
						//player.LoadFile(ofd.FileName);
						Console.WriteLine(ofd.FileName);
						selFileName = Path.GetFileName(ofd.FileName);
						selPathName = Path.GetDirectoryName(ofd.FileName);
						selExtName = Path.GetExtension(selFileName);
						selListNo = getListNo(ofd.FileName);
						files = Directory.GetFiles(selPathName, "*" + selExtName);
						fbName = getFB(ofd.FileName);
						if (fbName == "F")
						{
							mainName = ofd.FileName;
							subName = getPairFile(ofd.FileName, files);
						}
						else {
							mainName = getPairFile(ofd.FileName, files);
							subName = ofd.FileName;
						}

						//playMedia("E:\\VAVA\\VIDEO\\2020_0624_172324_310F.MP4", "E:\\VAVA\\VIDEO\\2020_0624_172323_309B.MP4");
						playMedia(mainName, subName);
					}
				}
				if (e.KeyValue == 39 && mainName != string.Empty && files.Length > 0)		//  39 Right键 ,有主视频，文件列表不为空 
				{
					string nextFile = string.Empty;

					selListNo = getListNo(mainName);
					files = files.OrderBy(s => s.ToString()).ToArray();
					foreach (var file in files)
					{
						if (getListNo(file) > selListNo && getFB(file) == "F")
						{
							nextFile = file;
							Console.WriteLine(file);
							break;
						}
					}
					mainName = nextFile;
					subName = getPairFile(mainName, files);
					playMedia(mainName, subName);
				}
				if (e.KeyValue == 37 && mainName != string.Empty && files.Length > 0)       // 37 Left键 ,有主视频，文件列表不为空 
				{
					string previousFile = string.Empty;

					selListNo = getListNo(mainName);
					files = files.OrderByDescending(s => s.ToString()).ToArray();
					foreach (var file in files)
					{
						if (getListNo(file) < selListNo && getFB(file) == "F")
						{
							previousFile = file;
							Console.WriteLine(file);
							break;
						}
					}
					mainName = previousFile;
					subName = getPairFile(mainName, files);
					playMedia(mainName, subName);
				}
			}
        }

        private string getPairFile(string selFilePath, string[] files)
        {
			int selListNo = getListNo(selFilePath);

			string fbName = getFB(selFilePath);
			string selFileName = Path.GetFileNameWithoutExtension(selFilePath);
			string preFileName = string.Empty;
			string fwdFileName = string.Empty;
			string fName = string.Empty;
			DateTime selDT;
			DateTime preDT;
			DateTime fwdDT;

			if (fbName == "F")
			{
				//files = files.OrderBy(s => int.Parse(Regex.Match(s, @"\d+").Value)).ToArray();
				files = files.OrderBy(s => s.ToString()).ToArray();
				foreach (var file in files)
				{
					if (getListNo(file) > selListNo && getFB(file) == "B")
					{
						fwdFileName = file;
						Console.WriteLine(file);
						break;
					}
				}
				files = files.OrderByDescending(s => s.ToString()).ToArray();
				foreach (var file in files)
				{
					if (getListNo(file) < selListNo && getFB(file) == "B")
					{
						preFileName = file;
						Console.WriteLine(file);
						break;
					}
				}
			}else
            {
				files = files.OrderBy(s => s.ToString()).ToArray();
				foreach (var file in files)
				{
					if (getListNo(file) > selListNo && getFB(file) == "F")
					{
						fwdFileName = file;
						Console.WriteLine(file);
						break;
					}
				}
				files = files.OrderByDescending(s => s.ToString()).ToArray();
				foreach (var file in files)
				{
					if (getListNo(file) < selListNo && getFB(file) == "F")
					{
						preFileName = file;
						Console.WriteLine(file);
						break;
					}
				}
			}
			//selDT = Convert.ToDateTime(selFileName.Substring(selFileName.Length - 21, 4) + selFileName.Substring(selFileName.Length - 16, 4) + selFileName.Substring(selFileName.Length - 11, 6), dtFormat);
			selDT = DateTime.ParseExact(selFileName.Substring(selFileName.Length - 21, 4) + selFileName.Substring(selFileName.Length - 16, 4) + selFileName.Substring(selFileName.Length - 11, 6), "yyyyMMddHHmmss", null);

			fName = Path.GetFileNameWithoutExtension(preFileName);
			//preDT = Convert.ToDateTime(fName.Substring(fName.Length - 21, 4) + fName.Substring(fName.Length - 16, 4) + fName.Substring(fName.Length - 11, 6), dtFormat);
			preDT = DateTime.ParseExact(fName.Substring(fName.Length - 21, 4) + fName.Substring(fName.Length - 16, 4) + fName.Substring(fName.Length - 11, 6), "yyyyMMddHHmmss", null);

			fName = Path.GetFileNameWithoutExtension(fwdFileName);
			//fwdDT = Convert.ToDateTime(fName.Substring(fName.Length - 21, 4) + fName.Substring(fName.Length - 16, 4) + fName.Substring(fName.Length - 11, 6), dtFormat);
			fwdDT = DateTime.ParseExact(fName.Substring(fName.Length - 21, 4) + fName.Substring(fName.Length - 16, 4) + fName.Substring(fName.Length - 11, 6), "yyyyMMddHHmmss", null);

			TimeSpan ts1 = fwdDT - selDT;
			TimeSpan ts2 = selDT - preDT;
			if (ts1.TotalSeconds < ts2.TotalSeconds)
			{
				return fwdFileName;
			}
			else
			{
				return preFileName;
			}

		}

        private int getListNo(string filePath)
        {
			int selListNo = -1;
			string fbName = string.Empty;
			fbName = Path.GetFileNameWithoutExtension(filePath);
			selListNo = int.Parse(fbName.Substring(fbName.Length - 4, 3));
			return selListNo;
		}
		private string getFB(string filePath)
        {
			string fbName = string.Empty;
			fbName = Path.GetFileNameWithoutExtension(filePath);
			return fbName.Substring(fbName.Length - 1, 1);
		}
    }
}
