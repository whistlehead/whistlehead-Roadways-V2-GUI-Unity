using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace whistlehead_Roadways_V2_GUI
{
    class Switches
    {
        public static (splineData, string, double) SwitchEdge(object profile, object shoulder, object adjacentLane, int shoulderWidthMode, eLaneWidthMode laneWidthMode, int kerb, string pavement, ePavWidthMode pavWidthMode, double offset, bool mirror, splineData data, string in_name, bool hasPav, bool hasKerb, bool delimit = true)
        {
            double splinePavOffset = 0;

            bool shoulderGetAdj = false;

            if (shoulderWidthMode == (int)eShoulderWidthMode.lan) shoulderWidthMode += (int)laneWidthMode;

            string temp_name = "";
            if (delimit) temp_name += "-";
            int mf = 1;
            if (mirror) mf = -1;
            double m_w_edge = eShoulderW.Get(shoulderWidthMode) * mf;
            double m_w_kerb = Profiles.w_kerb * mf;
            double m_w_gras = Profiles.w_gras * mf;
            double m_w_pav = ePavW.Get(pavWidthMode) * mf;
            double m_o_ped = m_w_pav / 2;

            bool no_prof = false;

            string shoulderTex = shoulder.ToString();
            if (shoulderTex == "Get from adjacent lane") shoulderTex = adjacentLane.ToString(); shoulderGetAdj = true;

            string pavementTex = pavement.ToString();

            if (!mirror)
            {
                temp_name += "S";
                if(!shoulderGetAdj) temp_name += SwitchSurface(shoulderTex);
                temp_name += SwitchShoulderWidthMode(shoulderWidthMode);
                temp_name += "-";
                if (hasKerb)
                {
                    temp_name += "K";
                    temp_name += SwitchKerb(kerb);
                    temp_name += "-";
                }
                if (hasPav)
                {
                    temp_name += "P";
                    temp_name += SwitchSurface(pavementTex);
                    temp_name += "-";
                }
            }

            switch (profile)
            {
                case "Hard edge":
                    data.edges.Add(new sectData(Profiles.data_edge[shoulderWidthMode], shoulderTex, offset, mirror));
                    temp_name += "VH";
                    break;
                case "Soft edge":
                    //data.edges.Add(new sectData(Profiles.data_verge[shoulderWidthMode], offset, mirror));
                    data.edges.Add(new sectData(Profiles.data_edge[shoulderWidthMode], shoulderTex, offset, mirror));
                    data.edges.Add(new sectData(Profiles.data_verge, offset + m_w_edge, mirror));
                    temp_name += "VS";
                    break;
                case "Kerb":
                    data.edges.Add(new sectData(Profiles.data_edge[shoulderWidthMode], shoulderTex, offset, mirror));
                    data.edges.Add(new sectData(Profiles.data_kerbs_double[kerb], offset + m_w_edge, mirror));
                    temp_name += "VK";
                    break;
                case "Kerb with grass slope":
                    data.edges.Add(new sectData(Profiles.data_edge[shoulderWidthMode], shoulderTex, offset, mirror));
                    data.edges.Add(new sectData(Profiles.data_kerbs_single[kerb], offset + m_w_edge, mirror));
                    data.edges.Add(new sectData(Profiles.data_grass_slope, offset + m_w_edge + m_w_kerb, mirror));
                    temp_name += "VKG";
                    break;
                case "Pavement":
                    data.edges.Add(new sectData(Profiles.data_edge[shoulderWidthMode], shoulderTex, offset, mirror));
                    data.edges.Add(new sectData(Profiles.data_kerbs_single[kerb], offset + m_w_edge, mirror));
                    data.edges.Add(new sectData(Profiles.data_pav[(int)pavWidthMode], pavementTex, offset + m_w_edge + m_w_kerb, mirror));
                    splinePavOffset = offset + m_w_edge + m_w_kerb + m_o_ped;
                    temp_name += "VP";
                    break;
                case "Pavement with grass slope":
                    data.edges.Add(new sectData(Profiles.data_edge[shoulderWidthMode], shoulderTex, offset, mirror));
                    data.edges.Add(new sectData(Profiles.data_kerbs_single[kerb], offset + m_w_edge, mirror));
                    data.edges.Add(new sectData(Profiles.data_pav[(int)pavWidthMode], pavementTex, offset + m_w_edge + m_w_kerb, mirror));
                    data.edges.Add(new sectData(Profiles.data_grass_slope, offset + m_w_edge + m_w_kerb + m_w_pav, mirror));
                    splinePavOffset = offset + m_w_edge + m_w_kerb + m_o_ped;
                    temp_name += "VPG";
                    break;
                case "Offset pavement":
                    data.edges.Add(new sectData(Profiles.data_edge[shoulderWidthMode], shoulderTex, offset, mirror));
                    data.edges.Add(new sectData(Profiles.data_kerbs_single[kerb], offset + m_w_edge, mirror));
                    data.edges.Add(new sectData(Profiles.data_grass_pavof, offset + m_w_edge + m_w_kerb, mirror));
                    data.edges.Add(new sectData(Profiles.data_pav[(int)pavWidthMode], pavementTex, offset + m_w_edge + m_w_kerb + m_w_gras, mirror));
                    splinePavOffset = offset + m_w_edge + m_w_kerb + m_w_gras + m_o_ped;
                    temp_name += "VOP";
                    break;
                case "Offset pavement with grass slope":
                    data.edges.Add(new sectData(Profiles.data_edge[shoulderWidthMode], shoulderTex, offset, mirror));
                    data.edges.Add(new sectData(Profiles.data_kerbs_single[kerb], offset + m_w_edge, mirror));
                    data.edges.Add(new sectData(Profiles.data_grass_pavof, offset + m_w_edge + m_w_kerb, mirror));
                    data.edges.Add(new sectData(Profiles.data_pav[(int)pavWidthMode], pavementTex, offset + m_w_edge + m_w_kerb + m_w_gras, mirror));
                    data.edges.Add(new sectData(Profiles.data_grass_slope, offset + m_w_edge + m_w_kerb + m_w_gras + m_w_pav, mirror));
                    splinePavOffset = offset + m_w_edge + m_w_kerb + m_w_gras + m_o_ped;
                    temp_name += "VOPG";
                    break;
                default:
                    temp_name = "";
                    no_prof = true;
                    break;
            }

            if (mirror && !no_prof)
            {
                if (hasPav)
                {
                    temp_name += "-";
                    temp_name += "P";
                    temp_name += SwitchSurface(pavementTex);
                    temp_name += SwitchPavementWidthMode(pavWidthMode);
                }
                if (hasKerb)
                {
                    temp_name += "-";
                    temp_name += "K";
                    temp_name += SwitchKerb(kerb);
                }
                temp_name += "-";
                temp_name += "S";
                if (!shoulderGetAdj) temp_name += SwitchSurface(shoulderTex);
                temp_name += SwitchShoulderWidthMode(shoulderWidthMode);
            }

            //CheckEnablePavOptions();
            return (data, in_name + temp_name, splinePavOffset);
        }

        public static (splineData, string) SwitchLane(object lane, object laneMode, eLaneWidthMode splineLaneWidthMode, double offset, bool mirror, splineData data, string in_name, bool in_delimit = true)
        {
            string temp_name = ""; if (in_delimit) { temp_name += "-"; } temp_name += "L";
            string tex = lane.ToString(); if (laneMode.ToString() == "Worn") tex += " Worn";
            data.lanes.Add(new sectData(Profiles.data_lane[(int)splineLaneWidthMode], tex, offset, mirror));
            double chevron_offset = offset; if (mirror) chevron_offset -= eLaneW.Get(splineLaneWidthMode);
            if (laneMode.ToString() == "Chevrons") data.lines.Add(new sectData(Profiles.data_lane_chevrons[(int)splineLaneWidthMode], chevron_offset, false));
            if (laneMode.ToString() == "Chevrons reversed") data.lines.Add(new sectData(Profiles.data_lane_chevrons_inv[(int)splineLaneWidthMode], chevron_offset, false));
            if (laneMode.ToString() == "Double Chevrons") data.lines.Add(new sectData(Profiles.data_lane_chevrons_both[(int)splineLaneWidthMode], chevron_offset, false));
            if (laneMode.ToString() == "Double Chevrons reversed") data.lines.Add(new sectData(Profiles.data_lane_chevrons_both_inv[(int)splineLaneWidthMode], chevron_offset, false));
            temp_name += SwitchSurface(lane.ToString(), laneMode.ToString());   
            return (data, in_name + temp_name);
        }

        //public static (splineData, string) SwitchEdgeMarking(object selectedItem, double offset, bool mirror, splineData data, string in_name, bool in_delimit = true)
        //{
        //    string temp_name = "";
        //    if (in_delimit) { temp_name += "-"; }
        //    switch (selectedItem)
        //    {
        //        case "Normal (RH: EdgeLine_Norm_100)":
        //            data.lines.Add(new sectData(Profiles.data_edgeline_norm, offset, mirror));
        //            temp_name += "ENo";
        //            break;
        //        case "Junction (RH: EdgeLine_Junc_100)":
        //            data.lines.Add(new sectData(Profiles.data_edgeline_junc, offset, mirror));
        //            temp_name += "EJu";
        //            break;
        //        case "Single yellow (RH: Singleyellow_100)":
        //            data.lines.Add(new sectData(Profiles.data_edgeline_singleyellow, offset, mirror));
        //            temp_name += "ESy";
        //            break;
        //        case "Double yellow (RH: Doubleyellow_100)":
        //            data.lines.Add(new sectData(Profiles.data_edgeline_doubleyellow, offset, mirror));
        //            temp_name += "EDy";
        //            break;
        //        case "Lane Normal (RH: CentreLine_Norm_100)":
        //            data.lines.Add(new sectData(Profiles.data_centreline_norm_std, offset, mirror));
        //            temp_name += "CNo";
        //            break;
        //        case "Lane Normal 2 (RH: CentreLine_Norm2_100)":
        //            data.lines.Add(new sectData(Profiles.data_centreline_norm2_std, offset, mirror));
        //            temp_name += "CNo2";
        //            break;
        //        case "Lane Warning (RH: CentreLine_Warn_100)":
        //            data.lines.Add(new sectData(Profiles.data_centreline_warn_std, offset, mirror));
        //            temp_name += "CWa";
        //            break;
        //        case "Lane Warning 2 (RH: CentreLine_Warn2_100)":
        //            data.lines.Add(new sectData(Profiles.data_centreline_warn2_std, offset, mirror));
        //            temp_name += "CWa2";
        //            break;
        //        default:
        //            temp_name = "";
        //            break;
        //    }
        //    return (data, in_name + temp_name);
        //}

        public static (splineData, string) SwitchEdgeMarking(int selectedIndex, double offset, bool mirror, splineData data, string in_name, bool in_delimit = true)
        {
            string temp_name = "";
            if (selectedIndex - 1 > Profiles.data_edgecentreline.Count || selectedIndex == 0) { return (data, in_name); }
            if (in_delimit) { temp_name += "-"; }
            data.lines.Add(new sectData(Profiles.data_edgecentreline[selectedIndex - 1].Prof(), offset, mirror));
            temp_name += Profiles.data_edgecentreline[selectedIndex - 1].ShortName();
            return (data, in_name + temp_name);
        }

        //public static (splineData, string) SwitchLaneMarking(object selectedItem, double offset, bool mirror, splineData data, string in_name, bool in_delimit = true)
        //{
        //    string temp_name = "";
        //    if (in_delimit) { temp_name += "-"; }
        //    switch (selectedItem)
        //    {
        //        case "Solid (RH: EdgeLine_Norm_100)":
        //            data.lines.Add(new sectData(Profiles.data_edgeline[1].Prof(), offset, mirror));
        //            //data.lines.Add(new sectData(Profiles.data_edgeline_norm, offset, mirror));
        //            temp_name += "ENo";
        //            break;
        //        case "Normal (RH: CentreLine_Norm_100)":
        //            data.lines.Add(new sectData(Profiles.data_centreline_norm_std, offset, mirror));
        //            temp_name += "CNo";
        //            break;
        //        case "Normal 2 (RH: CentreLine_Norm2_100)":
        //            data.lines.Add(new sectData(Profiles.data_centreline_norm2_std, offset, mirror));
        //            temp_name += "CNo2";
        //            break;
        //        case "Warning (RH: CentreLine_Warn_100)":
        //            data.lines.Add(new sectData(Profiles.data_centreline_warn_std, offset, mirror));
        //            temp_name += "CWa";
        //            break;
        //        case "Warning 2 (RH: CentreLine_Warn2_100)":
        //            data.lines.Add(new sectData(Profiles.data_centreline_warn2_std, offset, mirror));
        //            temp_name += "CWa2";
        //            break;
        //        case "Chevrons (1m) (RH: Chevrons_1000)":
        //            data.lines.Add(new sectData(Profiles.data_chevrons_1000, offset, mirror));
        //            temp_name += "CCv";
        //            break;
        //        case "Chevrons reversed (1m) (RH: Chevrons_inv_1000)":
        //            data.lines.Add(new sectData(Profiles.data_chevrons_inv_1000, offset, mirror));
        //            temp_name += "CCvR";
        //            break;
        //        case "Double chevrons (1m) (RH: Chevrons_both_1000)":
        //            data.lines.Add(new sectData(Profiles.data_chevrons_both_1000, offset, mirror));
        //            temp_name += "CCvB";
        //            break;
        //        case "Double chevrons reversed (1m) (RH: Chevrons_both_1000)":
        //            data.lines.Add(new sectData(Profiles.data_chevrons_both_inv_1000, offset, mirror));
        //            temp_name += "CCvBR";
        //            break;
        //        default:
        //            temp_name = "";
        //            break;
        //    }
        //    return (data, in_name + temp_name);
        //}

        public static (splineData, string) SwitchLaneMarking(int selectedIndex, double offset, bool mirror, splineData data, string in_name, bool in_delimit = true)
        {
            string temp_name = "";
            if (selectedIndex - 1 > Profiles.data_centreline.Count || selectedIndex == 0) { return (data, in_name); }
            if (in_delimit) { temp_name += "-"; }
            data.lines.Add(new sectData(Profiles.data_centreline[selectedIndex - 1].Prof(), offset, mirror));
            temp_name += Profiles.data_centreline[selectedIndex - 1].ShortName();
            return (data, in_name + temp_name);
        }

        public static (splineData, string) SwitchCarAI(eLaneWidthMode splineLaneWidthMode, double offset, int dir, splineData data, string out_name)
        {
            if (dir >= 0)
            {
                data.paths.Add(new aiPath(0, offset, Profiles.h_lane, eLanePathW.Get(splineLaneWidthMode), dir));
                out_name += (dir);
            }
            else
            {
                out_name += "X";
            }
            return (data, out_name);
        }

        public static (splineData, string) SwitchPedAI(bool[] hasPav, bool ai, ePavWidthMode[] splinePavWidthMode, double[] offsets, splineData data, string out_name)
        {
            if (ai)
            {
                if (hasPav[0]) data.paths.Add(new aiPath(ePathType.ped, offsets[0], Profiles.h_kerb, ePavPathW.Get(splinePavWidthMode[0]), 2));
                if (hasPav[1]) data.paths.Add(new aiPath(ePathType.ped, offsets[1], Profiles.h_kerb, ePavPathW.Get(splinePavWidthMode[1]), 2));
            }
            
            if (ai && (hasPav[0] || hasPav[1]))
                {
                out_name += " P";
            }
            else if ((hasPav[0] || hasPav[1]))
            {
                out_name += " NP";
            }

            return (data, out_name);
        }

        private static string SwitchSurface(string surface, string mode = "")
        {
            string temp_name = "";
            switch (surface)
            {
                case eTex.asph_norm:
                    temp_name += "ANo";
                    break;
                case eTex.asph_light:
                    temp_name += "ALi";
                    break;
                case eTex.asph_mid:
                    temp_name += "AMi";
                    break;
                case eTex.asph_dark:
                    temp_name += "ADa";
                    break;
                case eTex.treat_red:
                    temp_name += "TRe";
                    break;
                case eTex.treat_yel:
                    temp_name += "TYe";
                    break;
                case eTex.treat_gre:
                    temp_name += "TGr";
                    break;
                case eTex.treat_blu:
                    temp_name += "TBl";
                    break;
                case eTex.pav_cobbl:
                    temp_name += "PCo";
                    break;
                case eTex.pav_herri:
                    temp_name += "PHe";
                    break;
                case eTex.pav_slabs:
                    temp_name += "PSs";
                    break;
                case eTex.pav_slabl:
                    temp_name += "PSl";
                    break;
                case eTex.gravel:
                    temp_name += "Gra";
                    break;
                default:
                    temp_name = "";
                    break;
            }

            if (temp_name != "" && mode != "")
            {
                switch (mode)
                {
                    case "Worn":
                        temp_name += "W";
                        break;
                    case "Chevrons":
                        temp_name += "C";
                        break;
                    case "Chevrons reversed":
                        temp_name += "CR";
                        break;
                    case "Double Chevrons":
                        temp_name += "DC";
                        break;
                    case "Double Chevrons reversed":
                        temp_name += "DCR";
                        break;
                    default:
                        break;
                }
            }
            return temp_name;
        }

        private static string SwitchKerb(int kerb)
        {
            // yes, I know this is hard coded and horrible and requires knowledge of which index links to which profile
            string temp_name = "";
            switch (kerb)
            {
                case 0:
                    temp_name += "No";
                    break;
                case 1:
                    temp_name += "Sl";
                    break;
                default:
                    break;
            }
            return temp_name;
        }

        private static string SwitchShoulderWidthMode(int mode)
        {
            // this one too
            string temp_name = "";
            switch (mode)
            {
                case 1:
                    temp_name += "N";
                    break;
                case 2:
                    temp_name += "M";
                    break;
                case 3:
                    temp_name += "W";
                    break;
                case 4:
                case 5:
                    temp_name += "L";
                    break;
                default:
                    break;
            }
            return temp_name;
        }

        private static string SwitchPavementWidthMode(ePavWidthMode mode)
        {
            // not this one though
            string temp_name = "";
            switch (mode)
            {
                case ePavWidthMode.nar:
                    temp_name += "N";
                    break;
                case ePavWidthMode.wid:
                    temp_name += "W";
                    break;
                case ePavWidthMode.exw:
                    temp_name += "E";
                    break;
                default:
                    break;
            }
            return temp_name;
        }
    }
}
