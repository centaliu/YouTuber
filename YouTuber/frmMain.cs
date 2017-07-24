using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using System.Reflection;
using System.Diagnostics;

namespace YouTuber
{
	/// <logs>
	/// 2016.08.10: version: 1.0.0.7, automatically add a cr/lf while pasting a url
	/// 2016.06.21: version: 1.0.0.6, %21 means " in file name of the mp4 file
	/// 2016.06.20: version: 1.0.0.5, add a feature to get all video links from a youtube playlist
	/// 2016.06.20: version: 1.0.0.5, change the icon
	/// 2016.05.19: version: 1.0.0.4, %21 means ! in file name of the mp4 file
	/// 2016.05.13: version: 1.0.0.3, add a multi-line textbox to log what are done till now.
	/// 2016.05.12: version: 1.0.0.2, it turns out that is the most important parameter to get information of video links.
	/// 2016.05.12: version: 1.0.0.2, add a label to show title while downloading
	/// 2016.05.11: initilized version: 1.0.0.1, an application to download videos from youtube.com
	/// </logs>

	/// <known issue>
	/// 2016.05.12: haven't find a way to get video URL from a video link with classical format only
	/// </known issue>
	public partial class frmMain : Form
	{
		public frmMain()
		{
			InitializeComponent();
		}

		private DateTime tStart = DateTime.Now;
		private DateTime tNow = DateTime.Now;
		private string[] arrUrl = null;
		private string curUrl = "";
		private string curMp4Url = "";
		private string curTitle = "";
		private string videoInfo = "";
		private string[] arrVideoInfo = null;
		private Dictionary<string, string> dicVideoInfo = null;
		private string videoInfoPrefix = "http://www.youtube.com/get_video_info?el=embedded&el=vevo&video_id=";
		private string startUpPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		private void btnGo_Click(object sender, EventArgs e)
		{
			txtLog.Text = "";
			goNextOneEx();
		}

		private void goNextOneEx()
		{
			string otherUrls = "";
			arrUrl = txtUrls.Text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			// step 2: start from the top on if the list in not empty
			if (arrUrl.Length > 0)
			{
				// step 2.1: get the very first url
				curUrl = arrUrl[0];
				txtLog.Text = curUrl + "\r\n" + "-------------------------------------------------------------------------------------\r\n" + txtLog.Text;
				// step 2.2: put the rest back to the list textbox
				for (int i = 1; i < arrUrl.Length; i++) otherUrls += arrUrl[i] + "\r\n";
				txtUrls.Text = otherUrls;
				// step 3: get video info
				string[] theVTag = curUrl.Split('=');
				goDownloadFile(theVTag[1]);
				Application.DoEvents();
				goNextOneEx();
			}
			else
			{
				txtLog.Text = "no more entry in the list.\r\n" + txtLog.Text;
				curMp4Url = "";
				curUrl = "";
				curTitle = "";
				lblProg.Text = "Progress: n/a";
				this.Text = "Done";
				return;
			}
		}

		private void goDownloadFile(string VideoID)
		{
			string res = "";
			Application.DoEvents();
			Process p = new Process();
			// Redirect the output stream of the child process.
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardOutput = true;
			//p.StartInfo.FileName = @"D:\Projects\youtube-dl.exe";
			p.StartInfo.FileName = startUpPath + @"\youtube-dl.exe";
			p.StartInfo.Arguments = String.Format("-o \""+  Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\%(title)s.%(ext)s\" {0}", VideoID);
			p.Start();
			// Read the output stream first and then wait.
			res = p.StandardOutput.ReadToEnd();
			p.WaitForExit();
		}

		// get the top one url from the list and start working on this one.
		private void goNextOne()
		{
			string otherUrls = "";
			// step 1: turn the list of those URLs to an array
			arrUrl = txtUrls.Text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			// step 2: start from the top on if the list in not empty
			if (arrUrl.Length > 0) {
				// step 2.1: get the very first url
				curUrl = arrUrl[0];
				txtLog.Text = curUrl + "\r\n" + txtLog.Text;
				// step 2.2: put the rest back to the list textbox
				for (int i = 1; i < arrUrl.Length; i++) otherUrls += arrUrl[i] + "\r\n";
				txtUrls.Text = otherUrls;
			} else {
				txtLog.Text = "no more entry in the list.\r\n" + txtLog.Text;
				curMp4Url = "";
				curUrl = "";
				curTitle = "";
				lblProg.Text = "Progress: n/a";
				this.Text = "Done";
				return;
			}
			// step 3: get video info
			string[] theVTag = curUrl.Split('=');
			using (WebClient client = new WebClient()) videoInfo = client.DownloadString(videoInfoPrefix + theVTag[1]);
			arrVideoInfo = videoInfo.Split('&');
			dicVideoInfo.Clear();
			foreach (string str in arrVideoInfo) {
				string[] tmpData = str.Split('=');
				dicVideoInfo.Add(tmpData[0], htmlDecode(tmpData[1]));
			}
			// step 3.1: if errorcode key is found, it means that there is something wrong
			if (dicVideoInfo.ContainsKey("errorcode")) {
				txtLog.Text = "got errorcode " + dicVideoInfo["errorcode"] + "\r\n" + txtLog.Text;
				goNextOne();
				return;
			}
			// step 4: get further detail information from adaptive_fmts parameter
			curTitle = refineTitle(dicVideoInfo["title"]);
			txtLog.Text = curTitle + "\r\n" + txtLog.Text;
			videoInfo = "";
			bool getQ = false;
			// step 4.1: try to get data of video in url_encoded_fmt_stream_map
			if (dicVideoInfo.ContainsKey("url_encoded_fmt_stream_map")) {
				videoInfo = dicVideoInfo["url_encoded_fmt_stream_map"];
				arrVideoInfo = videoInfo.Split(',');
				foreach (string str in arrVideoInfo) {
					if (str.IndexOf("quality=medium") >= 0) {
						videoInfo = str;
						getQ = true;
						break;
					}
				}
				// step 4.2: get even further video information for getting specific 360p video if data string got.
				if (getQ) {
					arrVideoInfo = videoInfo.Split('&');
					foreach (string str in arrVideoInfo) {
						if (str.IndexOf("url=") >= 0) {
							string[] tmpData = str.Split('=');
							curMp4Url = urlDecode(tmpData[1]);
							break;
						}
					}
				}
			}
			if (videoInfo == "") {
				txtLog.Text = "fail on getting video info from " + curUrl + "\r\n" + txtLog.Text;
				return;
			}
			// step 6: start downloading the file, avoid duplication as well
			if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\" + curTitle + ".mp4")) DownloadFile(curMp4Url, Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\" + curTitle + ".mp4");
			goNextOne();
		}

		private bool DownloadFile(string sSourceURL, string sDestinationPath)
		{
			string[] arrPath = sDestinationPath.Split('\\');
			bool completed = false;
			TimeSpan TS = DateTime.Now - DateTime.Now;
			long iFileSize = 0;
			int iBufferSize = 1024;
			iBufferSize *= 100;
			long iExistLen = 0;
			System.IO.FileStream saveFileStream;
			if (System.IO.File.Exists(sDestinationPath))
			{
				System.IO.FileInfo fINfo = new System.IO.FileInfo(sDestinationPath);
				iExistLen = fINfo.Length;
			}
			if (iExistLen > 0)
				saveFileStream = new System.IO.FileStream(sDestinationPath, System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite);
			else
				saveFileStream = new System.IO.FileStream(sDestinationPath, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite);
			System.Net.HttpWebRequest hwRq;
			System.Net.HttpWebResponse hwRes;
			hwRq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(sSourceURL);
			hwRq.AddRange((int)iExistLen);
			System.IO.Stream smRespStream;
			try
			{
				hwRes = (System.Net.HttpWebResponse)hwRq.GetResponse();
				smRespStream = hwRes.GetResponseStream();
				iFileSize = hwRes.ContentLength;
			}
			catch (Exception ex)
			{
				txtLog.Text = "exception: " + ex.Message + "\r\n" + txtLog.Text;
				return true;
			}
			double bytesIn = double.Parse(iExistLen.ToString());
			double totalBytes = double.Parse(iFileSize.ToString()) + bytesIn;
			int iByteSize;
			double thisDown = 0;
			byte[] downBuffer = new byte[iBufferSize];
			tStart = DateTime.Now;
			try
			{
				while ((iByteSize = smRespStream.Read(downBuffer, 0, downBuffer.Length)) > 0)
				{
					saveFileStream.Write(downBuffer, 0, iByteSize);
					tNow = DateTime.Now;
					TS = tNow - tStart;
					bytesIn = bytesIn + iByteSize;
					thisDown = thisDown + iByteSize;
					double spd = Math.Truncate(thisDown / TS.TotalSeconds / 1024);
					if (TS.TotalSeconds == 0.0) spd = 0.0;
					double percentage = bytesIn / totalBytes * 100;
					if (spd >= 1024.0)
					{
						spd = spd / 1024.0d;
						spd = Math.Round(spd, 3);
						lblProg.Text = "Progress: " + spd.ToString() + " MB/Sec..." + remain(totalBytes, bytesIn, spd * 1024) + "..." + Math.Round(percentage, 2).ToString() + "%...Downloading";
						this.Text = Math.Round(percentage, 2).ToString() + "% - " + spd.ToString() + " MB/Sec";
					}
					else
					{
						lblProg.Text = "Progress: " + spd.ToString() + " KB/Sec..." + remain(totalBytes, bytesIn, spd) + "..." + Math.Round(percentage, 2).ToString() + "%...Downloading";
						this.Text = Math.Round(percentage, 2).ToString() + "% - " + spd.ToString() + " KB/Sec";
					}
					Application.DoEvents();
				}
			}
			catch (Exception ex)
			{
				//MessageBox.Show("error: " + ex.Message.ToString());
				return true;
			}
			smRespStream.Close();
			saveFileStream.Close();
			return completed;
		}

		private string remain(double totalBytes, double bytesIn, double spd)
		{
			string ret = "00:00:00";
			if (spd > 0.0)
			{
				ret = "";
				double dEst = Math.Truncate((totalBytes - bytesIn) / (spd * 1024));
				int iEst = Convert.ToInt32(dEst);
				ret = Math.Floor((double)iEst / 3600).ToString();
				iEst = iEst % 3600;
				ret = ret + ":" + Math.Floor((double)iEst / 60).ToString().PadLeft(2, '0');
				iEst = iEst % 60;
				ret = ret + ":" + iEst.ToString().PadLeft(2, '0');
			}
			return ret;
		}

		// to decode url
		private string urlDecode(string instr)
		{
			string ret = instr;
			//ret = ret.Replace("%22", "\"");
			//ret = ret.Replace("%23", "#");
			//ret = ret.Replace("%24", "$");
			ret = ret.Replace("%25", "%");
			ret = ret.Replace("%26", "&");
			//ret = ret.Replace("%27", "'");
			//ret = ret.Replace("%28", "(");
			//ret = ret.Replace("%29", ")");
			//ret = ret.Replace("%2A", "*");
			//ret = ret.Replace("%2B", "+");
			ret = ret.Replace("%2C", ",");
			//ret = ret.Replace("%2D", "-");
			//ret = ret.Replace("%2E", ".");
			ret = ret.Replace("%2F", "/");
			ret = ret.Replace("%3A", ":");
			//ret = ret.Replace("%3B", ";");
			//ret = ret.Replace("%3C", "<");
			ret = ret.Replace("%3D", "=");
			//ret = ret.Replace("%3E", ">");
			ret = ret.Replace("%3F", "?");
			//ret = ret.Replace("%40", "@");
			if (ret.IndexOf("%25") >= 0) ret = urlDecode(ret);
			return ret;
		}

		private string htmlDecode(string instr)
		{
			string ret = instr;
			//ret = ret.Replace("%22", "\"");
			//ret = ret.Replace("%23", "#");
			//ret = ret.Replace("%24", "$");
			//ret = ret.Replace("%25", "%");
			ret = ret.Replace("%26", "&");
			//ret = ret.Replace("%27", "'");
			//ret = ret.Replace("%28", "(");
			//ret = ret.Replace("%29", ")");
			//ret = ret.Replace("%2A", "*");
			//ret = ret.Replace("%2B", "+");
			ret = ret.Replace("%2C", ",");
			//ret = ret.Replace("%2D", "-");
			//ret = ret.Replace("%2E", ".");
			//ret = ret.Replace("%2F", "/");
			//ret = ret.Replace("%3A", ":");
			//ret = ret.Replace("%3B", ";");
			//ret = ret.Replace("%3C", "<");
			ret = ret.Replace("%3D", "=");
			//ret = ret.Replace("%3E", ">");
			ret = ret.Replace("%3F", "?");
			ret = ret.Replace("%40", "@");
			return ret;
		}

		// due to title is the file name, but there are some constraits as a file name, so has to refine it.
		private string refineTitle(string instr)
		{
			string ret = instr;
			ret = ret.Replace("+", " ");
			ret = ret.Replace("?", "-");
			ret = ret.Replace(":", "-");
			ret = ret.Replace("%3A", "-");
			ret = ret.Replace("|", "-");
			ret = ret.Replace("%7C", "-");
			ret = ret.Replace("<", "-");
			ret = ret.Replace(">", "-");
			ret = ret.Replace("/", "-");
			ret = ret.Replace("\\", "-");
			ret = ret.Replace("\"", "-");
			ret = ret.Replace("%2F", "-");
			ret = ret.Replace("%21", "!");
			ret = ret.Replace("%22", "-");
			ret = Uri.UnescapeDataString(ret);
			return ret;
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			// step 1: initialize the dictionary variable
			dicVideoInfo = new Dictionary<string, string>();
		}

		// click the "List" to get all video links
		private void btnGetList_Click(object sender, EventArgs e)
		{
			// step 1: check if txtList is empty
			txtList.Text = txtList.Text.Trim();
			if (txtList.Text == "")
			{
				MessageBox.Show("please fulfill the list ID");
				return;
			}
			// browse the playlist
			WebBrowser x = new WebBrowser();
			x.DocumentCompleted += x_DocumentCompleted;
			x.Navigate(@"https://www.youtube.com/playlist?list=" + txtList.Text);
		}

		// process data once the webpage is navigated
		void x_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			WebBrowser y = (WebBrowser)sender;
			string vID = "";
			string url = e.Url.ToString();
			if (url.IndexOf(txtList.Text) > -1) {
				HtmlElement ele = y.Document.All["browse-items-primary"];
				foreach (HtmlElement child in ele.All) {
					if (child.TagName.ToLower() == "tr") {
						vID = child.GetAttribute("data-video-id");
						if (vID != "") {
							if (txtUrls.Text.IndexOf(vID) < 0) txtUrls.Text = txtUrls.Text + "https://www.youtube.com/watch?v=" + vID + "\r\n";
						}
					}
				}
			}
			txtList.Text = "";
		}

		private void txtUrls_TextChanged(object sender, EventArgs e)
		{
			if (txtUrls.Text.Length <= 1) return;
			string lastCh = txtUrls.Text.Substring(txtUrls.Text.Length - 1, 1);
			byte[] asciiBytes = Encoding.ASCII.GetBytes(lastCh);
			if (asciiBytes[0] != 10) txtUrls.Text = txtUrls.Text + "\n";
			else {
				txtUrls.SelectionStart = txtUrls.Text.Length;
				txtUrls.SelectionLength = 0;
				txtUrls.Focus();
			}
		}

		private void txtUrls_KeyDown(object sender, KeyEventArgs e)
		{
		}

	}
}
