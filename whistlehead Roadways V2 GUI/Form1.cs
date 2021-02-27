using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO.Pipes;
using System.Text;
using System.Threading;

//using namespace Profiles;

namespace whistlehead_Roadways_V2_GUI
{
    public partial class Form1 : Form
    {
        private bool programInitialised = false;
        private bool unityInitialised = false;
        private static readonly string programVersion = "v0.0.1";
        private static readonly string programFilePath = Application.StartupPath;
        private static readonly string programOpeningMessage = "";
        private static readonly string programOutputName = "wh roadways v2";
        private static string programDatPath = programFilePath + "\\dat";
        private static string programTexturePath = programDatPath + "\\texture";
        private static string programSnowPath = programTexturePath + "\\wintersnow";

        private string filePathOMSI;
        private string projectName;
        private string projectSubPath;
        private string projectPath;

        private string debugFileName = "aaaaaaa26";

        private string out_name;
        private string out_name_auto;
        private string out_name_custom;
        private splineData out_data;
        private string out_string;
        private Dictionary<string, texData> out_textures;
        private static eLaneWidthMode splineLaneWidthMode = eLaneWidthMode.std;
        private static ePavWidthMode[] splinePavWidthMode = { ePavWidthMode.std, ePavWidthMode.std };
        private static double[] splinePavOffset = { 0, 0 };
        private static bool[] splineHasPav = { false, false };
        private static bool[] splineHasKerbSC = { false, false };

        private bool viewportHidden = false;

        private static List<ComboBox> sc_laneList;
        private static List<ComboBox> sc_laneModeList;
        private static List<ComboBox> sc_laneMarkingList;
        private static List<ComboBox> sc_laneAIList;

        private eLaneEnabled laneEnabled;

        private Dictionary<string, texData> textureDictionary;

        [DllImport("User32.dll")]
        static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);
        internal delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
        [DllImport("user32.dll")]
        internal static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);
        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        private Process process;
        private IntPtr unityHWND = IntPtr.Zero;
        private const int WM_ACTIVATE = 0x0006;
        private readonly IntPtr WA_ACTIVE = new IntPtr(1);
        private readonly IntPtr WA_INACTIVE = new IntPtr(0);
        private StreamString serverDataStream;
        private StreamString serverAliveStream;
        Thread _AliveThread;

        private int createButtonPresses = 0;

        public Form1()
        {
            InitializeComponent();

            MakeTextureDictionary();

            filePathOMSI = Properties.Settings.Default.filePathOMSI;
            lbl_browseOMSI.Text = filePathOMSI;
            projectName = Properties.Settings.Default.projectName;
            tbx_projectName.Text = projectName;
            UpdateProjectPath();

            // create server before starting Unity process
            NamedPipeServerStream dataServer = new NamedPipeServerStream("whistleheadRoadwaysDataPipe", PipeDirection.InOut, 1);
            NamedPipeServerStream aliveServer = new NamedPipeServerStream("whistleheadRoadwaysAlivePipe", PipeDirection.InOut, 1);

            try
            {
                process = new Process();
                IntPtr handle = pnl_Unity.Handle;
                int width = pnl_Unity.Width;
                int height = pnl_Unity.Height;
                process.StartInfo.FileName = programFilePath + "\\Viewer\\Child.exe";
                process.StartInfo.Arguments = "-parentHWND " + handle.ToInt32() + " " + Environment.CommandLine;
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.CreateNoWindow = true;

                process.Start();

                process.WaitForInputIdle();
                EnumChildWindows(handle, WindowEnum, IntPtr.Zero);

                MoveWindow(unityHWND, 0, 0, width, height, true);
                ActivateUnityWindow();
                // wait for client to connect
                dataServer.WaitForConnection();
                aliveServer.WaitForConnection();
                // created stream for reading and writing
                serverDataStream = new StreamString(dataServer);
                serverAliveStream = new StreamString(aliveServer);
                unityInitialised = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ".\nCheck if Container.exe is placed next to Child.exe.");
            }

             _AliveThread = new Thread(() =>
            {
                while (true)
                {
                    _ = serverAliveStream.ReadString();
                    Thread.Sleep(100);
                }
            });
            _AliveThread.Start();

            WriteToDataStreamAndDiscardResponse("Update Dat Path");
            WriteToDataStreamAndDiscardResponse(programDatPath);
        }

        private void ActivateUnityWindow()
        {
            SendMessage(unityHWND, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
        }

        private void DeactivateUnityWindow()
        {
            SendMessage(unityHWND, WM_ACTIVATE, WA_INACTIVE, IntPtr.Zero);
        }

        private int WindowEnum(IntPtr hwnd, IntPtr lparam)
        {
            unityHWND = hwnd;
            ActivateUnityWindow();
            return 0;
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            MoveWindow(unityHWND, 0, 0, pnl_Unity.Width, pnl_Unity.Height, true);
            ActivateUnityWindow();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                process.CloseMainWindow();
                while (process.HasExited == false) process.Kill();
            }
            catch (Exception)
            {

            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            ActivateUnityWindow();
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            DeactivateUnityWindow();
        }
        
        // Make methods

        private void MakeFile()
        {
            string[] out_path = { projectPath };

            if (out_data.Count() > 0 && out_string != "")
            {
                DialogResult result;

                for (int i = 0; i < out_path.Length; i++)
                {
                    string full_out_path = out_path[i] + "\\" + out_name;

                    if(full_out_path.Length > 260)
                    {
                        MessageBox.Show("File path exceeds Windows character limit (260 characters)\r\nTry using a custom file name");
                        MessageBox.Show("File writing error; file not created");
                        return;
                    }

                    if (!Directory.Exists(projectPath))
                    {
                        result = MessageBox.Show("There is no project folder:\r\n" + projectSubPath + "\r\nCreate new project folder?", "", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            Directory.CreateDirectory(projectPath);
                            string projectTexturePath = projectPath + "\\texture";
                            string projectSnowPath = projectTexturePath + "\\wintersnow";
                            CopyDirectory(programTexturePath, projectTexturePath);
                            CopyDirectory(programSnowPath, projectSnowPath);
                        }
                    }

                    result = MessageBox.Show("You are about to create:\r\n" + out_name + "\r\nIn folder:\r\n" + out_path[i] + "\r\nContinue?", "", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            File.WriteAllText(full_out_path, out_string);
                        }
                        catch
                        {
                            MessageBox.Show("File writing error; file not created");
                        }
                    }
                }
            }
        }

        private void MakeData()
        {
            if (programInitialised)
            {
                EnableEdgeControlsAndCheckForPavementAndKerbSC();

                out_name_auto = "";
                out_data = new splineData();
                out_textures = new Dictionary<string, texData>();

                switch (cmb_carriageways.SelectedItem)
                {
                    case "Single carriageway":

                        int lanes = cmb_SC_lanes.SelectedIndex + 1;
                        double offset = lanes * eLaneW.Get(splineLaneWidthMode)/ 2;

                        // carriageway mode
                        out_name_auto += "SC ";
                        out_name_auto += cmb_SC_lanes.SelectedItem.ToString();
                        out_name_auto += "L ";

                        // lane width mode
                        //if (rbt_laneWidth_std.Checked) out_name_auto += "Standard ";
                        //else if (rbt_laneWidth_nar.Checked) out_name_auto += "Narrow ";
                        if (cmb_laneWidth.SelectedIndex == 1) out_name_auto += "Narrow "; else out_name_auto += "Standard ";

                        // ai
                        for (int i = 0; i < lanes; i++)
                        {
                            (out_data, out_name_auto) = Switches.SwitchCarAI(splineLaneWidthMode, -offset + (eLaneW.Get(splineLaneWidthMode) / 2) + i * eLaneW.Get(splineLaneWidthMode), sc_laneAIList[i].SelectedIndex - 1, out_data, out_name_auto);
                        }
                        out_name_auto += " ";

                        // left edge
                        (out_data, out_name_auto, splinePavOffset[0]) = Switches.SwitchEdge(cmb_SC_leftEdgeProfile.SelectedItem, cmb_SC_leftEdgeShoulder.SelectedItem, sc_laneList[0].SelectedItem, cmb_SC_leftEdgeShoulderMode.SelectedIndex, splineLaneWidthMode, cmb_SC_leftEdgeKerb.SelectedIndex, cmb_SC_leftEdgePavement.SelectedItem.ToString(), splinePavWidthMode[0], -offset, true, out_data, out_name_auto, splineHasPav[0], splineHasKerbSC[0], false);
                        //(out_data, out_name_auto) = Switches.SwitchEdgeMarking(cmb_SC_leftEdgeMarking.SelectedItem, -offset, true, out_data, out_name_auto, cmb_SC_leftEdgeProfile.SelectedIndex > 0);
                        (out_data, out_name_auto) = Switches.SwitchEdgeMarking(cmb_SC_leftEdgeMarking.SelectedIndex, -offset, true, out_data, out_name_auto, cmb_SC_leftEdgeProfile.SelectedIndex > 0);

                        // lanes
                        bool lane_delimit = (cmb_SC_leftEdgeProfile.SelectedIndex > 0 || cmb_SC_leftEdgeMarking.SelectedIndex > 0);
                        for (int i = 0; i < lanes; i++)
                        {
                            bool mirror = i < (double)lanes / 2;
                            double mirrorCorrection = Convert.ToDouble(mirror) * eLaneW.Get(splineLaneWidthMode);
                            (out_data, out_name_auto) = Switches.SwitchLane(sc_laneList[i].SelectedItem, sc_laneModeList[i].SelectedItem, splineLaneWidthMode, -offset + (i * eLaneW.Get(splineLaneWidthMode)) + mirrorCorrection, mirror, out_data, out_name_auto, lane_delimit);
                            if (i < lanes - 1) (out_data, out_name_auto) = Switches.SwitchLaneMarking(sc_laneMarkingList[i].SelectedIndex, -offset + ((i + 1) * eLaneW.Get(splineLaneWidthMode)), false, out_data, out_name_auto);
                            lane_delimit = true;
                        }

                        //right edge
                        //(out_data, out_name_auto) = Switches.SwitchEdgeMarking(cmb_SC_rightEdgeMarking.SelectedItem, offset, false, out_data, out_name_auto);
                        (out_data, out_name_auto) = Switches.SwitchEdgeMarking(cmb_SC_rightEdgeMarking.SelectedIndex, offset, false, out_data, out_name_auto);
                        (out_data, out_name_auto, splinePavOffset[1]) = Switches.SwitchEdge(cmb_SC_rightEdgeProfile.SelectedItem, cmb_SC_rightEdgeShoulder.SelectedItem, sc_laneList[lanes - 1].SelectedItem, cmb_SC_rightEdgeShoulderMode.SelectedIndex, splineLaneWidthMode, cmb_SC_rightEdgeKerb.SelectedIndex, cmb_SC_rightEdgePavement.SelectedItem.ToString(), splinePavWidthMode[1], offset, false, out_data, out_name_auto, splineHasPav[1], splineHasKerbSC[1], true);

                        // ped ai
                        (out_data, out_name_auto) = Switches.SwitchPedAI(splineHasPav, cbx_SC_pedestrianAI.Checked, splinePavWidthMode, splinePavOffset, out_data, out_name_auto);

                        break;
                    case "Dual carriageway":


                        break;
                    default:
                        MessageBox.Show("Unhandled carriageways case");
                        break;
                }

                out_name_auto += ".sli";
                MakeTextureList();
                UpdateFileName();
                MakeSpline();
                UpdateUnityViewport();
            }
        }

        private void MakeSpline()
        {
            out_string = "";

            out_string += MakeHeader();

            out_string += "##### TEXTURES #####\r\n\r\n";
            out_string += MakeTexs();

            out_string += "##### HEIGHTPROFILES #####\r\n\r\n";
            out_string += MakeHeightProfile();

            out_string += "##### PROFILES #####\r\n\r\n";

            out_string += "----- EDGES -----\r\n\r\n";
            out_string += MakeProfiles(out_data.edges);

            out_string += "----- LANES -----\r\n\r\n";
            out_string += MakeProfiles(out_data.lanes);

            out_string += "----- LINES -----\r\n\r\n";
            out_string += MakeProfiles(out_data.lines);

            out_string += "##### PATHS #####\r\n\r\n";
            out_string += MakePaths(out_data.paths);
        }

        private string MakeHeader()
        {
            string ret_string = "";
            ret_string += "Roadways V2" + "\r\n";
            ret_string += "Created by whistlehead / Chris Nightingale, 2020" + "\r\n";
            return ret_string;
        }

        private string MakeHeightProfile()
        {
            (double min, double max) = FindMaxAndMinPoints();
            string ret_string = "";
            ret_string += "[heightprofile]\r\n";
            ret_string += min + "\r\n";
            ret_string += max + "\r\n";
            ret_string += Profiles.h_lane + "\r\n";
            ret_string += Profiles.h_lane + "\r\n";
            ret_string += "\r\n";
            return ret_string;
        }
        
        private string MakeTexs()
        {
            string ret_string = "";
            foreach (KeyValuePair<string, texData> entry in out_textures) ret_string += MakeTex(entry);
            return ret_string;
        }
        
        private string MakeTex(KeyValuePair<string, texData> entry)
        {
            (string name, int alpha) = entry.Value.GetNameAlpha();

            string ret_string = "";
            ret_string += "[texture]" + "\r\n";
            ret_string += name + "\r\n";

            if (alpha != 0) {
                ret_string += "[matl_alpha]" + "\r\n";
                ret_string += alpha + "\r\n";
            }

            ret_string += "\r\n";
            return ret_string;
        }

        private string MakeProfiles(List<sectData> sectList)
        {
            string ret_string = "";
            foreach (sectData sect in sectList)
            {
                ret_string += MakeProfile(sect);
            }
            return ret_string;
        }

        private string MakeProfile(sectData section)
        {
            List<profData> profiles = section.profiles;
            // tex is the relative texture path
            string tex;
            string ret_string = "";

            for (int i_prof = 0; i_prof < profiles.Count; i_prof++)
            {
                profData profile = profiles[i_prof];
                if (section.mat_override) tex = section.mat; else tex = profile.mat;

                int len = profile.len;

                double[] p_pnts = new double[len];
                double[] p_hgts = new double[len];
                double[] p_texs = new double[len];
                double[] p_reps = new double[len];

                // de-reference original arrays
                Array.Copy(profile.pnts, p_pnts, len);
                Array.Copy(profile.hgts, p_hgts, len);
                Array.Copy(profile.texs, p_texs, len);
                Array.Copy(profile.reps, p_reps, len);

                ret_string += "[profile]" + "\r\n";
                ret_string += out_textures[tex].GetIndex() + "\r\n";

                if (section.mirror)
                {
                    Array.Reverse(p_pnts);
                    for (int i = 0; i < len; i++) { p_pnts[i] *= -1; };
                    Array.Reverse(p_hgts);
                    Array.Reverse(p_texs);
                    Array.Reverse(p_reps);
                }

                for (int i = 0; i < len; i++) p_pnts[i] += section.offset;

                for (int i_pnt = 0; i_pnt < len; i_pnt++)
                {
                    ret_string += "[profilepnt]" + "\r\n";
                    ret_string += p_pnts[i_pnt] + "\r\n";
                    ret_string += p_hgts[i_pnt] + "\r\n";
                    ret_string += p_texs[i_pnt] + "\r\n";
                    ret_string += p_reps[i_pnt] + "\r\n";
                }

                ret_string += "\r\n";
            }
             
            return ret_string;
        }

        private string MakePaths(List<aiPath> pathList)
        {
            string ret_string = "";
            foreach (aiPath path in pathList)
            {
                ret_string += MakePath(path);
            }
            return ret_string;
        }

        private string MakePath(aiPath path)
        {
            string ret_string = "";
            ret_string += "[path]\r\n";
            ret_string += path.type + "\r\n";
            ret_string += path.offset + "\r\n";
            ret_string += path.height + "\r\n";
            ret_string += path.width + "\r\n";
            ret_string += path.direction + "\r\n";
            ret_string += "\r\n";
            return ret_string;
        }


        // Helper methods
        
        private string CleanDirtyString(string dirty)
        {
            return Regex.Replace(dirty, "[^A-Za-z0-9_ ]", "_");
        }

        private void UpdateProjectPath()
        {
            projectSubPath = "splines\\" + projectName + "\\" + programOutputName;
            projectPath = filePathOMSI + "\\" + projectSubPath;
            lbl_projectPath.Text = projectSubPath;
        }

        private void UpdateFileName()
        {
            if (rbt_fileMode_automatic.Checked) { out_name = out_name_auto; }
            else if (rbt_fileMode_custom.Checked) { out_name = out_name_custom; }
            lbl_outputFileName.Text = out_name;
        }

        private void WriteToDataStreamAndDiscardResponse(String input)
        {
            if (unityInitialised)
            {
                serverDataStream.WriteString(input);
                _ = serverDataStream.ReadString();
            }
        }

        private void UpdateUnityViewport()
        {
            WriteToDataStreamAndDiscardResponse("Regenerate");
            WriteToDataStreamAndDiscardResponse(out_string);
        }

        private void MakeTextureList()
        {
            out_textures = new Dictionary<string, texData>();
            int i = 0;
            foreach (sectData sect in out_data.lanes)
            {
                if (sect.mat_override)
                {
                    if (!out_textures.ContainsKey(sect.mat))
                    {
                        out_textures.Add(sect.mat, textureDictionary[sect.mat]);
                        out_textures[sect.mat].SetIndex(i);
                        i++;
                    }
                }
                else
                {
                    foreach (profData prof in sect.profiles)
                    {
                        if (!out_textures.ContainsKey(prof.mat))
                        {
                            out_textures.Add(prof.mat, textureDictionary[prof.mat]);
                            out_textures[prof.mat].SetIndex(i);
                            i++;
                        }
                    }
                }
            }
            foreach (sectData sect in out_data.edges)
            {
                if (sect.mat_override)
                {
                    if (!out_textures.ContainsKey(sect.mat))
                    {
                        out_textures.Add(sect.mat, textureDictionary[sect.mat]);
                        out_textures[sect.mat].SetIndex(i);
                        i++;
                    }
                }
                foreach (profData prof in sect.profiles)
                {
                    if (!out_textures.ContainsKey(prof.mat))
                    {
                        out_textures.Add(prof.mat, textureDictionary[prof.mat]);
                        out_textures[prof.mat].SetIndex(i);
                        i++;
                    }
                }
            }
            foreach (sectData sect in out_data.lines)
            {
                foreach (profData prof in sect.profiles)
                {
                    if (!out_textures.ContainsKey(prof.mat))
                    {
                        out_textures.Add(prof.mat, textureDictionary[prof.mat]);
                        out_textures[prof.mat].SetIndex(i);
                        i++;
                    }
                }
            }
        }

        private void MakeTextureDictionary()
        {
            textureDictionary = new Dictionary<string, texData>();

            textureDictionary.Add(eTex.asph_norm, new texData("asphalt norm\\main.bmp", 0));
            textureDictionary.Add(eTex.asph_norm_worn, new texData("asphalt norm\\worn.bmp", 0));
            //textureDictionary.Add(eTex.asph_norm_verg, new texData("asphalt norm\\verge.dds", 1));

            textureDictionary.Add(eTex.asph_light, new texData("asphalt light\\main.bmp", 0));
            textureDictionary.Add(eTex.asph_light_worn, new texData("asphalt light\\worn.bmp", 0));
            //textureDictionary.Add(eTex.asph_light_verg, new texData("asphalt light\\verge.dds", 1));

            textureDictionary.Add(eTex.asph_mid, new texData("asphalt mid\\main.bmp", 0));
            textureDictionary.Add(eTex.asph_mid_worn, new texData("asphalt mid\\worn.bmp", 0));
            //textureDictionary.Add(eTex.asph_mid_verg, new texData("asphalt mid\\verge.dds", 1));

            textureDictionary.Add(eTex.asph_dark, new texData("asphalt dark\\main.bmp", 0));
            textureDictionary.Add(eTex.asph_dark_worn, new texData("asphalt dark\\worn.bmp", 0));

            textureDictionary.Add(eTex.treat_red, new texData("treatment red\\main.bmp", 0));
            textureDictionary.Add(eTex.treat_red_worn, new texData("treatment red\\worn.bmp", 0));

            textureDictionary.Add(eTex.treat_yel, new texData("treatment yellow\\main.bmp", 0));
            textureDictionary.Add(eTex.treat_yel_worn, new texData("treatment yellow\\worn.bmp", 0));

            textureDictionary.Add(eTex.treat_gre, new texData("treatment green\\main.bmp", 0));
            textureDictionary.Add(eTex.treat_gre_worn, new texData("treatment green\\worn.bmp", 0));

            textureDictionary.Add(eTex.treat_blu, new texData("treatment blue\\main.bmp", 0));
            textureDictionary.Add(eTex.treat_blu_worn, new texData("treatment blue\\worn.bmp", 0));

            textureDictionary.Add(eTex.pav_cobbl, new texData("pavement cobble\\main.bmp", 0));
            textureDictionary.Add(eTex.pav_cobbl_worn, new texData("pavement cobble\\worn.bmp", 0));

            textureDictionary.Add(eTex.pav_herri, new texData("pavement herringbone\\main.bmp", 0));
            textureDictionary.Add(eTex.pav_herri_worn, new texData("pavement herringbone\\worn.bmp", 0));

            textureDictionary.Add(eTex.pav_slabs, new texData("pavement slabs small\\main.bmp", 0));
            textureDictionary.Add(eTex.pav_slabs_worn, new texData("pavement slabs small\\worn.bmp", 0));

            textureDictionary.Add(eTex.pav_slabl, new texData("pavement slabs large\\main.bmp", 0));
            textureDictionary.Add(eTex.pav_slabl_worn, new texData("pavement slabs large\\worn.bmp", 0));

            textureDictionary.Add(eTex.gravel, new texData("gravel\\main.bmp", 0));
            textureDictionary.Add(eTex.gravel_worn, new texData("gravel\\worn.bmp", 0));

            textureDictionary.Add(eTex.verge, new texData("verge.dds", 1));
            textureDictionary.Add(eTex.white, new texData("markings\\white.dds", 1));
            textureDictionary.Add(eTex.yello, new texData("markings\\yellow.dds", 1));
            textureDictionary.Add(eTex.hatch, new texData("markings\\hatching.dds", 1));
            textureDictionary.Add(eTex.kerbs, new texData("kerbs\\kerb_norm.bmp", 0));
            textureDictionary.Add(eTex.terra, new texData("terrain.bmp", 0));
        }

        private void CopyDirectory(string sourcePath, string outputPath)
        {
            if (!Directory.Exists(sourcePath))
            {
                throw new InvalidOperationException("Source folder does not exist");
            }

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            DirectoryInfo dir = new DirectoryInfo(sourcePath);
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(outputPath, file.Name);
                try
                {
                    file.CopyTo(temppath, false);
                }
                catch { }
            }

            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                CopyDirectory(subDir.FullName, outputPath + "\\" + subDir.Name);
            }
        }

        private (double, double) FindMaxAndMinPoints()
        {
            double min = 0, max = 0;
            FindMaxAndMinPointsInListOfSectData(out_data.lanes);
            FindMaxAndMinPointsInListOfSectData(out_data.lines);
            FindMaxAndMinPointsInListOfSectData(out_data.edges);
            return (min, max);

            void FindMaxAndMinPointsInListOfSectData(List<sectData> sectList)
            {
                foreach (sectData sect in sectList)
                {
                    foreach (profData prof in sect.profiles)
                    {
                        foreach (double point in prof.pnts)
                        {
                            double offsetPoint = point;
                            if (sect.mirror) { offsetPoint *= -1; }
                            offsetPoint += sect.offset;
                            max = Math.Max(offsetPoint, max);
                            min = Math.Min(offsetPoint, min);
                        }
                    }
                }
            }
        }

        private void InitComboBox(ComboBox in_combobox, string[] in_list, int in_index)
        {
            in_combobox.Items.Clear();
            foreach (string s in in_list) in_combobox.Items.Add(s);
            in_combobox.SelectedIndex = in_index;
        }
        private void InitComboBox(ComboBox in_combobox, string[] in_list)
        {
            in_combobox.Items.Clear();
            foreach (string s in in_list) in_combobox.Items.Add(s);
        }

        private void InitComboBoxLists() //TODO: add DC lane lists
        {
            sc_laneList = new List<ComboBox>();
            sc_laneList.Add(cmb_SC_lane1);
            sc_laneList.Add(cmb_SC_lane2);
            sc_laneList.Add(cmb_SC_lane3);
            sc_laneList.Add(cmb_SC_lane4);
            sc_laneList.Add(cmb_SC_lane5);
            sc_laneList.Add(cmb_SC_lane6);

            sc_laneModeList = new List<ComboBox>();
            sc_laneModeList.Add(cmb_SC_laneMode1);
            sc_laneModeList.Add(cmb_SC_laneMode2);
            sc_laneModeList.Add(cmb_SC_laneMode3);
            sc_laneModeList.Add(cmb_SC_laneMode4);
            sc_laneModeList.Add(cmb_SC_laneMode5);
            sc_laneModeList.Add(cmb_SC_laneMode6);

            sc_laneMarkingList = new List<ComboBox>();
            sc_laneMarkingList.Add(cmb_SC_laneMarking12);
            sc_laneMarkingList.Add(cmb_SC_laneMarking23);
            sc_laneMarkingList.Add(cmb_SC_laneMarking34);
            sc_laneMarkingList.Add(cmb_SC_laneMarking45);
            sc_laneMarkingList.Add(cmb_SC_laneMarking56);

            sc_laneAIList = new List<ComboBox>();
            sc_laneAIList.Add(cmb_SC_laneAI1);
            sc_laneAIList.Add(cmb_SC_laneAI2);
            sc_laneAIList.Add(cmb_SC_laneAI3);
            sc_laneAIList.Add(cmb_SC_laneAI4);
            sc_laneAIList.Add(cmb_SC_laneAI5);
            sc_laneAIList.Add(cmb_SC_laneAI6);
        }

        private void EnableLaneControlsSC()
        {
            int lanes = cmb_SC_lanes.SelectedIndex + 1;

            if (!cbx_SC_linkLanes.Checked)
            {
                for (int i = 0; i < sc_laneList.Count; i++)
                {
                    if (i < lanes) sc_laneList[i].Enabled = true;
                    else sc_laneList[i].Enabled = false;
                }

                for (int i = 0; i < sc_laneModeList.Count; i++)
                {
                    if (i < lanes) sc_laneModeList[i].Enabled = true;
                    else sc_laneModeList[i].Enabled = false;
                }
            }

            for (int i = 0; i < sc_laneAIList.Count; i++)
            {
                if (i < lanes) sc_laneAIList[i].Enabled = true;
                else sc_laneAIList[i].Enabled = false;
                if (i < (lanes + 1) / 2) sc_laneAIList[i].SelectedIndex = 1;
                else sc_laneAIList[i].SelectedIndex = 2;
            }

            for (int i = 0; i < sc_laneMarkingList.Count; i++)
            {
                if (i < lanes - 1) sc_laneMarkingList[i].Enabled = true;
                else sc_laneMarkingList[i].Enabled = false;
            }
        }

        private void EnableEdgeControlsAndCheckForPavementAndKerbSC()
        {
            if (!cmb_SC_leftEdgeProfile.SelectedItem.ToString().Contains("avement"))
            {
                cmb_SC_leftEdgePavement.Enabled = false;
                cmb_SC_leftEdgePavementWidth.Enabled = false;
            }
            else
            {
                cmb_SC_leftEdgePavement.Enabled = true;
                cmb_SC_leftEdgePavementWidth.Enabled = true;
            }

            if (cmb_SC_leftEdgeProfile.SelectedIndex < 2)
            {
                cmb_SC_leftEdgeKerb.Enabled = false;
                cmb_SC_leftEdgePavement.Enabled = false;
                cmb_SC_leftEdgePavementWidth.Enabled = false;
            }
            else
            {
                cmb_SC_leftEdgeKerb.Enabled = true;
            }

            if (cmb_SC_leftEdgeProfile.SelectedIndex == 0)
            {
                cmb_SC_leftEdgeShoulder.Enabled = false;
                cmb_SC_leftEdgeShoulderMode.Enabled = false;
                cmb_SC_leftEdgeKerb.Enabled = false;
            }
            else
            {
                cmb_SC_leftEdgeShoulder.Enabled = true;
                cmb_SC_leftEdgeShoulderMode.Enabled = true;
            }
            splineHasPav[0] = cmb_SC_leftEdgePavement.Enabled;
            splineHasKerbSC[0] = cmb_SC_leftEdgeKerb.Enabled;


            if (!cmb_SC_rightEdgeProfile.SelectedItem.ToString().Contains("avement"))
            {
                cmb_SC_rightEdgePavement.Enabled = false;
                cmb_SC_rightEdgePavementWidth.Enabled = false;
            }
            else
            {
                cmb_SC_rightEdgePavement.Enabled = true;
                cmb_SC_rightEdgePavementWidth.Enabled = true;
            }

            if (cmb_SC_rightEdgeProfile.SelectedIndex < 2)
            {
                cmb_SC_rightEdgeKerb.Enabled = false;
                cmb_SC_rightEdgePavement.Enabled = false;
                cmb_SC_rightEdgePavementWidth.Enabled = false;
            }
            else
            {
                cmb_SC_rightEdgeKerb.Enabled = true;
            }

            if (cmb_SC_rightEdgeProfile.SelectedIndex == 0)
            {
                cmb_SC_rightEdgeShoulder.Enabled = false;
                cmb_SC_rightEdgeShoulderMode.Enabled = false;
                cmb_SC_rightEdgeKerb.Enabled = false;
            }
            else
            {
                cmb_SC_rightEdgeShoulder.Enabled = true;
                cmb_SC_rightEdgeShoulderMode.Enabled = true;
            }
            splineHasPav[1] = cmb_SC_rightEdgePavement.Enabled;
            splineHasKerbSC[1] = cmb_SC_rightEdgeKerb.Enabled;
        }

        // GUI methods

        private void Form1_Load(object sender, EventArgs e) //TODO: add DC lane lists
        {
            InitComboBoxLists();

            InitComboBox(cmb_carriageways, DropdownLists.carriageways, 0);
            InitComboBox(cmb_SC_lanes, DropdownLists.SC_lanes, 1);
            InitComboBox(cmb_laneWidth, DropdownLists.laneWidth, 0);

            InitComboBox(cmb_SC_lane1, DropdownLists.lane, 2);
            InitComboBox(cmb_SC_lane2, DropdownLists.lane, 2);
            InitComboBox(cmb_SC_lane3, DropdownLists.lane, 0);
            InitComboBox(cmb_SC_lane4, DropdownLists.lane, 0);
            InitComboBox(cmb_SC_lane5, DropdownLists.lane, 0);
            InitComboBox(cmb_SC_lane6, DropdownLists.lane, 0);

            InitComboBox(cmb_SC_laneMode1, DropdownLists.laneMode, 1);
            InitComboBox(cmb_SC_laneMode2, DropdownLists.laneMode, 1);
            InitComboBox(cmb_SC_laneMode3, DropdownLists.laneMode, 0);
            InitComboBox(cmb_SC_laneMode4, DropdownLists.laneMode, 0);
            InitComboBox(cmb_SC_laneMode5, DropdownLists.laneMode, 0);
            InitComboBox(cmb_SC_laneMode6, DropdownLists.laneMode, 0);

            InitComboBox(cmb_SC_laneAI1, DropdownLists.ai, 1);
            InitComboBox(cmb_SC_laneAI2, DropdownLists.ai, 2);
            InitComboBox(cmb_SC_laneAI3, DropdownLists.ai, 0);
            InitComboBox(cmb_SC_laneAI4, DropdownLists.ai, 0);
            InitComboBox(cmb_SC_laneAI5, DropdownLists.ai, 0);
            InitComboBox(cmb_SC_laneAI6, DropdownLists.ai, 0);

            List<string> eList = new List<string>();
            eList.Add("None");
            foreach (lineMetaData l in Profiles.data_edgeline)
            {
                eList.Add(l.LongName());
            }
            string[] edgeMarking = eList.ToArray();
            eList = null;
            InitComboBox(cmb_SC_leftEdgeMarking, edgeMarking, 1);
            InitComboBox(cmb_SC_rightEdgeMarking, edgeMarking, 1);


            List<string> lList = new List<string>();
            lList.Add("None");
            foreach (lineMetaData l in Profiles.data_centreline)
            {
                lList.Add(l.LongName());
            }
            string[] laneMarking = lList.ToArray();
            lList = null;
            InitComboBox(cmb_SC_laneMarking12, laneMarking, 3);
            InitComboBox(cmb_SC_laneMarking23, laneMarking, 0);
            InitComboBox(cmb_SC_laneMarking34, laneMarking, 0);
            InitComboBox(cmb_SC_laneMarking45, laneMarking, 0);
            InitComboBox(cmb_SC_laneMarking56, laneMarking, 0);

            InitComboBox(cmb_SC_leftEdgeProfile, DropdownLists.edge, 1);
            InitComboBox(cmb_SC_leftEdgeShoulder, DropdownLists.shoulder, 0);
            InitComboBox(cmb_SC_leftEdgeShoulderMode, DropdownLists.shoulderMode, 1);
            InitComboBox(cmb_SC_leftEdgeKerb, DropdownLists.kerb, 0);
            InitComboBox(cmb_SC_leftEdgePavement, DropdownLists.pavement, 0);
            InitComboBox(cmb_SC_leftEdgePavementWidth, DropdownLists.pavementWidth, 1);

            InitComboBox(cmb_SC_rightEdgeProfile, DropdownLists.edge, 1);
            InitComboBox(cmb_SC_rightEdgeShoulder, DropdownLists.shoulder, 0);
            InitComboBox(cmb_SC_rightEdgeShoulderMode, DropdownLists.shoulderMode, 1);
            InitComboBox(cmb_SC_rightEdgeKerb, DropdownLists.kerb, 0);
            InitComboBox(cmb_SC_rightEdgePavement, DropdownLists.pavement, 0);
            InitComboBox(cmb_SC_rightEdgePavementWidth, DropdownLists.pavementWidth, 1);

            tbx_fileNameCustom.Text = "default";

            programInitialised = true;
            MakeData();

            WindowState = FormWindowState.Minimized;
            WindowState = FormWindowState.Normal;
            Focus(); 
            Show();
        }
        private void Form1_FormClosing(object sender, EventArgs e)
        {
            _AliveThread.Abort();
        }


        private void btn_browseOMSI_Click(object sender, EventArgs e)
        {
            DialogResult result = ofd_browseOMSI.ShowDialog();
            if (result == DialogResult.OK)
            {
                filePathOMSI = Path.GetDirectoryName(ofd_browseOMSI.FileName);
                lbl_browseOMSI.Text = filePathOMSI;
                Properties.Settings.Default.filePathOMSI = filePathOMSI;
                Properties.Settings.Default.Save();
                UpdateProjectPath();
            }
        }
        private void btn_create_Click(object sender, EventArgs e)
        {
            //createButtonPresses++;
            //WriteToDataStreamAndDiscardResponse("You pressed the create button " + createButtonPresses + " time(s)");
            MakeData();
            MakeFile();
        }

        private void tbx_projectName_TextChanged(object sender, EventArgs e)
        {
            if (tbx_projectName.Text != null && tbx_projectName.Text != "")
            {
                string projectNameDirty = tbx_projectName.Text;
                projectName = CleanDirtyString(projectNameDirty);
                Properties.Settings.Default.projectName = projectName;
                Properties.Settings.Default.Save();
                UpdateProjectPath();
            }
            else
            {
                projectName = "default";
                Properties.Settings.Default.projectName = projectName;
                Properties.Settings.Default.Save();
                UpdateProjectPath();
            }
        }
        private void tbx_fileNameCustom_TextChanged(object sender, EventArgs e)
        {
            if (tbx_fileNameCustom.Text.Length > 0) { out_name_custom = CleanDirtyString(tbx_fileNameCustom.Text); out_name_custom += ".sli"; }
            else { out_name_custom = "default.sli"; }
            UpdateFileName();
        }

        private void rbt_fileMode_automatic_CheckedChanged(object sender, EventArgs e)
        {
            tbx_fileNameCustom.Enabled = false;
            UpdateFileName();
        }
        private void rbt_fileMode_custom_CheckedChanged(object sender, EventArgs e)
        {
            tbx_fileNameCustom.Enabled = true;
            UpdateFileName();
        }

        private void cmb_template_SelectedIndexChanged(object sender, EventArgs e)
        {
            MakeData();
        }

        private void cmb_SC_leftLane_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbx_SC_linkLanes.Checked)
            {
                int index = cmb_SC_lane1.SelectedIndex;
                foreach (ComboBox c in sc_laneList)
                {
                    c.SelectedIndex = index;
                    c.Enabled = false;
                }
                int modeIndex = cmb_SC_laneMode1.SelectedIndex;
                foreach (ComboBox c in sc_laneModeList)
                {
                    c.SelectedIndex = modeIndex;
                    c.Enabled = false;
                }
                cmb_SC_lane1.Enabled = true;
                cmb_SC_laneMode1.Enabled = true;
            }
            MakeData();
        }
        private void cmb_SC_leftEdgeMarking_SelectedIndexChanged(object sender, EventArgs e)
        {
            MakeData();
        }
        private void cmb_SC_rightEdgeMarking_SelectedIndexChanged(object sender, EventArgs e)
        {
            MakeData();
        }
        private void cmb_SC_centreLaneMarking_SelectedIndexChanged(object sender, EventArgs e)
        {
            MakeData();
        }
        private void cmb_SC_leftEdgeProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_SC_leftEdgeProfile.SelectedItem.ToString().Contains("avement")) splineHasPav[0] = true; else splineHasPav[0] = false;
            if (cmb_SC_leftEdgeProfile.SelectedIndex > 3) splineHasKerbSC[0] = true; else splineHasKerbSC[0] = false;
            MakeData();
        }
        private void cmb_SC_rightEdgeProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_SC_rightEdgeProfile.SelectedItem.ToString().Contains("avement")) splineHasPav[1] = true; else splineHasPav[1] = false;
            if (cmb_SC_rightEdgeProfile.SelectedIndex > 3) splineHasKerbSC[1] = true; else splineHasKerbSC[1] = false;
            MakeData();
        }
        private void cmb_SC_leftLaneAI_SelectedIndexChanged(object sender, EventArgs e)
        {
            MakeData();
        }

        private void cbx_SC_pedestrianAI_CheckedChanged(object sender, EventArgs e)
        {
            MakeData();
        }

        private void cmb_SC_leftEdgeKerb_SelectedIndexChanged(object sender, EventArgs e)
        {

            MakeData();
        }

        private void cmb_SC_leftEdgeShoulderMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            MakeData();
        }
        private void cmb_SC_leftEdgeShoulder_SelectedIndexChanged(object sender, EventArgs e)
        {
            MakeData();
        }
        private void cmb_SC_lanes_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableLaneControlsSC();
            MakeData();
        }
        private void cmb_SC_lane2_SelectedIndexChanged(object sender, EventArgs e)
        {

            MakeData();
        }
        private void cmb_SC_lane3_SelectedIndexChanged(object sender, EventArgs e)
        {

            MakeData();
        }
        private void cmb_SC_lane4_SelectedIndexChanged(object sender, EventArgs e)
        {

            MakeData();
        }

        private void cmb_SC_lane5_SelectedIndexChanged(object sender, EventArgs e)
        {

            MakeData();
        }
        private void cmb_SC_lane6_SelectedIndexChanged(object sender, EventArgs e)
        {

            MakeData();
        }

        private void cmb_SC_laneMode1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbx_SC_linkLanes.Checked)
            {
                int index = cmb_SC_lane1.SelectedIndex;
                foreach (ComboBox c in sc_laneList)
                {
                    c.SelectedIndex = index;
                    c.Enabled = false;
                }
                int modeIndex = cmb_SC_laneMode1.SelectedIndex;
                foreach (ComboBox c in sc_laneModeList)
                {
                    c.SelectedIndex = modeIndex;
                    c.Enabled = false;
                }
                cmb_SC_lane1.Enabled = true;
                cmb_SC_laneMode1.Enabled = true;
            }
            MakeData();
        }
        private void cmb_SC_laneMode2_SelectedIndexChanged(object sender, EventArgs e)
        {

            MakeData();
        }
        private void cmb_SC_laneMode3_SelectedIndexChanged(object sender, EventArgs e)
        {

            MakeData();
        }
        private void cmb_SC_laneMode4_SelectedIndexChanged(object sender, EventArgs e)
        {

            MakeData();
        }
        private void cmb_SC_laneMode5_SelectedIndexChanged(object sender, EventArgs e)
        {

            MakeData();
        }
        private void cmb_SC_laneMode6_SelectedIndexChanged(object sender, EventArgs e)
        {

            MakeData();
        }

        private void cmb_SC_laneMarking12_SelectedIndexChanged(object sender, EventArgs e)
        {

            MakeData();
        }
        private void cmb_SC_laneMarking23_SelectedIndexChanged(object sender, EventArgs e)
        {

            MakeData();
        }
        private void cmb_SC_laneMarking45_SelectedIndexChanged(object sender, EventArgs e)
        {

            MakeData();
        }
        private void cmb_SC_laneMarking56_SelectedIndexChanged(object sender, EventArgs e)
        {

            MakeData();
        }

        private void cmb_SC_laneAI2_SelectedIndexChanged(object sender, EventArgs e)
        {

            MakeData();
        }
        private void cmb_SC_laneAI3_SelectedIndexChanged(object sender, EventArgs e)
        {

            MakeData();
        }
        private void cmb_SC_laneAI4_SelectedIndexChanged(object sender, EventArgs e)
        {

            MakeData();
        }
        private void cmb_SC_laneAI5_SelectedIndexChanged(object sender, EventArgs e)
        {

            MakeData();
        }
        private void cmb_SC_laneAI6_SelectedIndexChanged(object sender, EventArgs e)
        {

            MakeData();
        }

        private void cmb_SC_linkLanes_CheckedChanged(object sender, EventArgs e)
        {
            if (cbx_SC_linkLanes.Checked)
            {
                int index = cmb_SC_lane1.SelectedIndex;
                foreach (ComboBox c in sc_laneList)
                {
                    c.SelectedIndex = index;
                    c.Enabled = false;
                }
                int modeIndex = cmb_SC_laneMode1.SelectedIndex;
                foreach (ComboBox c in sc_laneModeList)
                {
                    c.SelectedIndex = modeIndex;
                    c.Enabled = false;
                }
                cmb_SC_lane1.Enabled = true;
                cmb_SC_laneMode1.Enabled = true;
            }
            else
            {
                EnableLaneControlsSC();
            }
        }

        private void cmb_SC_rightEdgeShoulderMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            MakeData();
        }
        private void cmb_SC_rightEdgeShoulder_SelectedIndexChanged(object sender, EventArgs e)
        {
            MakeData();
        }

        private void cmb_SC_rightEdgeKerb_SelectedIndexChanged(object sender, EventArgs e)
        {
            MakeData();
        }
        private void cmb_SC_rightEdgePavement_SelectedIndexChanged(object sender, EventArgs e)
        {
            MakeData();
        }

        private void cmb_SC_leftEdgePavement_SelectedIndexChanged(object sender, EventArgs e)
        {
            MakeData();
        }

        private void btn_hideViewport_Click(object sender, EventArgs e)
        {
            viewportHidden = !viewportHidden;
            if (viewportHidden) { this.Width = 571; btn_hideViewport.Text = ">"; }
            else { this.Width = 1117; btn_hideViewport.Text = "<"; }
        }

        private void cmb_leftPavWidth_SelectedIndexChanged(object sender, EventArgs e)
        {
            splinePavWidthMode[0] = (ePavWidthMode)cmb_SC_leftEdgePavementWidth.SelectedIndex;
            MakeData();
        }

        private void cmb_rightPavWidth_SelectedIndexChanged(object sender, EventArgs e)
        {
            splinePavWidthMode[1] = (ePavWidthMode)cmb_SC_rightEdgePavementWidth.SelectedIndex;
            MakeData();
        }

        private void cmb_laneWidth_SelectedIndexChanged(object sender, EventArgs e)
        {
            splineLaneWidthMode = (eLaneWidthMode)cmb_laneWidth.SelectedIndex;
            MakeData();
        }

        private void cbx_SC_showCentreMarkings_CheckedChanged(object sender, EventArgs e)
        {
            int leftIndex = cmb_SC_leftEdgeMarking.SelectedIndex;
            int rightIndex = cmb_SC_rightEdgeMarking.SelectedIndex;
            if (cbx_SC_showCentreMarkings.Checked)
            {
                List<string> eList = new List<string>();
                eList.Add("None");
                foreach (lineMetaData l in Profiles.data_edgecentreline)
                {
                    eList.Add(l.LongName());
                }
                InitComboBox(cmb_SC_leftEdgeMarking, eList.ToArray(), leftIndex);
                InitComboBox(cmb_SC_rightEdgeMarking, eList.ToArray(), rightIndex);

            }
            else
            {
                int maxIndex = Profiles.data_edgeline.Count;
                List<string> eList = new List<string>();
                eList.Add("None");
                foreach (lineMetaData l in Profiles.data_edgeline)
                {
                    eList.Add(l.LongName());
                }
                InitComboBox(cmb_SC_leftEdgeMarking, eList.ToArray(), Math.Min(leftIndex, maxIndex));
                InitComboBox(cmb_SC_rightEdgeMarking, eList.ToArray(), Math.Min(rightIndex, maxIndex));
            }

        }
    }
}
